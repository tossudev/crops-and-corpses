using Godot;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

public partial class FenceDoor : Node2D
{
    [Export]
    Node2D _doorsTopdown, _doors;

    [Export]
    CollisionShape2D _doorsTopdownCollisionLeft, _doorsTopdownCollisionRight, _doorsCollision;


    void OnFenceDoorInput(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is not InputEventMouseButton { Pressed: true } mouseEvent) return;

        if (mouseEvent.ButtonIndex == MouseButton.Left)
        {
            if(_doorsCollision.Disabled) 
            {
                Doors();
            }
            else
            {
                DoorsTopdown();
            }
        }
    }

    public void DoorsTopdown()
    {
        _doorsTopdown.Show();
        _doorsTopdownCollisionRight.Disabled = false;
        _doorsTopdownCollisionLeft.Disabled = false;

        _doors.Hide();
        _doorsCollision.Disabled = true;
    }

    public void Doors()
    {
        _doorsTopdownCollisionRight.Disabled = true;
        _doorsTopdownCollisionLeft.Disabled = true;
        _doorsTopdown.Hide();

        _doorsCollision.Disabled = false;
        _doors.Show();
    }
}
