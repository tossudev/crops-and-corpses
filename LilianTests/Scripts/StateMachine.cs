using Godot;
using System;
using System.Collections.Generic;

public partial class StateMachine : Node
{
	[Export] public States initial_state;

    private States _current_state;
    private Dictionary<string, States> _states = new Dictionary<string, States>();

    public override void _Ready()
    {
        foreach (Node child in GetChildren())
        {
            if (child is States state)
            {
                _states[state.Name.ToString().ToLower()] = state;
                state.Transitioned += OnChildTransitioned;
            }
        }

        if (initial_state != null)
        {
            initial_state.Enter();
            _current_state = initial_state;
        }
    }

    public override void _Process(double delta)
    {
        if (_current_state != null)
        {
            _current_state.Update(delta);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_current_state != null)
        {
            _current_state.Physics_Update(delta);
        }
    }

    private void OnChildTransitioned(string newStateName)
    {
        if (_current_state == null || newStateName == null)
        {
            return;
        }

        string newStateNameLower = newStateName.ToLower();
        if (!_states.TryGetValue(newStateNameLower, out States newState))
        {
            return;
        }

        _current_state.Exit();
        newState.Enter();
        _current_state = newState;

        //GD.Print("Current state: " + _current_state.Name);
    }
}
