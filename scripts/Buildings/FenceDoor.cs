using Godot;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

public partial class FenceDoor : Node2D
{
    [Export]
    Node2D _doorsOpened, _doorsClosed;

    [Export]
    CollisionShape2D _doorsCollisionOpen1, _doorsCollisionOpen2, _doorsCollision;


    void OnFenceDoorInput(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is not InputEventMouseButton { Pressed: true } mouseEvent) return;

        if (mouseEvent.ButtonIndex == MouseButton.Left)
        {
            if(_doorsCollision.Disabled) 
            {
                DoorsClose();
            }
            else
            {
                DoorsOpen();
            }
        }
    }

    public void DoorsOpen()
    {
        _doorsOpened.Show();
        _doorsCollisionOpen2.Disabled = false;
        _doorsCollisionOpen1.Disabled = false;

        _doorsClosed.Hide();
        _doorsCollision.Disabled = true;
    }

    public void DoorsClose()
    {
        _doorsCollisionOpen2.Disabled = true;
        _doorsCollisionOpen1.Disabled = true;
        _doorsOpened.Hide();

        _doorsCollision.Disabled = false;
        _doorsClosed.Show();
    }
}
