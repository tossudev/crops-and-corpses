using Godot;
using System;
using System.Collections.Generic;

public partial class StateMachine : Node
{
	[Export]
    public States initial_state;

    private States current_state;
    private Dictionary<string, States> states = new Dictionary<string, States>();

    public override void _Ready()
    {
        foreach (Node child in GetChildren())
        {
            if (child is States state)
            {
                states[state.Name.ToString().ToLower()] = state;
                state.Transitioned += OnChildTransitioned;
            }
        }

        if (initial_state != null)
        {
            initial_state.Enter();
            current_state = initial_state;
        }
    }

    public override void _Process(double delta)
    {
        if (current_state != null)
        {
            current_state.Update(delta);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (current_state != null)
        {
            current_state.Physics_Update(delta);
        }
    }

    private void OnChildTransitioned(string newStateName)
    {
        if (current_state == null || newStateName == null)
        {
            return;
        }

        string newStateNameLower = newStateName.ToLower();
        if (!states.TryGetValue(newStateNameLower, out States newState))
        {
            return;
        }

        current_state.Exit();
        newState.Enter();
        current_state = newState;

        GD.Print("Current state: " + current_state.Name);
    }
}
