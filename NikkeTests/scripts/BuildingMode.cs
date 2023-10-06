using Godot;
using System;
using System.Diagnostics;

public partial class BuildingMode : Node2D
{
    public int collisions;

    public BuildingMenu buildingMenu;

    int _tileSize;

    public override void _Ready()
    {
        _tileSize = 128;
    }

    public override void _PhysicsProcess(double delta)
    {
        SnapBuildingToGrid();
    }

    public override async void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Click"))
        {
            await ToSignal(GetTree(), "physics_frame");

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
            //Input.MouseMode = Input.MouseModeEnum.Visible;
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

        float _snapX = _mousePosition.X % _tileSize;
        float _snapY = _mousePosition.Y % _tileSize;

        if (_snapX >= _tileSize/2)
        {
            _snapX = -(_tileSize - _snapX);
        }
        else if (_snapX <= -_tileSize / 2)
        {
            _snapX = _tileSize + _snapX;
        }

        if (_snapY >= _tileSize / 2)
        {
            _snapY = -(_tileSize - _snapY);
        }
        else if (_snapY <= -_tileSize / 2)
        {
            _snapY = _tileSize + _snapY;
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

    private void _on_area_2d_body_entered(Node2D body)
    {
        collisions++;
    }

    private void _on_area_2d_body_exited(Node2D body)
    {
        collisions--;
    }
}
