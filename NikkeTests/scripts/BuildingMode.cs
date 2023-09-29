using Godot;
using System;
using System.Diagnostics;

public partial class BuildingMode : Node2D
{
    public int collisions;

    public BuildingMenu buildingMenu;

    public override void _Process(double delta)
    {
        SnapBuildingToGrid();

        if (Input.IsActionJustPressed("Click"))
        {
            if (collisions > 0)
            {
                Debug.WriteLine("Someting is colliding with the building");
                return;
            }

            buildingMenu.Build();
        }
        else if (Input.IsActionPressed("ui_cancel"))
        {
            QueueFree();
            Input.MouseMode = Input.MouseModeEnum.Visible;
            buildingMenu.EnableBuildButton();
        }

        if (collisions > 0)
        {
            Modulate = new Color(3, 1, 1, 1);
        }
        else
        {
            Modulate = new Color(1, 1, 1, 1);
        }
    }

    private void SnapBuildingToGrid()
    {
        Vector2 _mousePosition = GetGlobalMousePosition();

        float _snapX = _mousePosition.X % 64;
        float _snapY = _mousePosition.Y % 64;

        if (_snapX >= 32)
        {
            _snapX = -(64 - _snapX);
        }
        else if (_snapX <= -32)
        {
            _snapX = 64 + _snapX;
        }

        if (_snapY >= 32)
        {
            _snapY = -(64 - _snapY);
        }
        else if (_snapY <= -32)
        {
            _snapY = 64 + _snapY;
        }

        Vector2 _snapLocation = new Vector2(_mousePosition.X - _snapX, _mousePosition.Y - _snapY);

        GlobalPosition = _snapLocation;
    }

    private void _on_area_2d_area_entered(Area2D area) 
	{
        collisions++;
    }

    private void _on_area_2d_area_exited(Area2D area)
    {
        collisions--;
    }
}
