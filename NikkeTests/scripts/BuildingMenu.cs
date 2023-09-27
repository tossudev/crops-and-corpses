using Godot;
using System;
using System.Diagnostics;

public partial class BuildingMenu : ScrollContainer
{
	Building _townHall, _farmPlot, _house;

	Building _currentBuilding;
	Node2D _buildingScene;

	bool _isPlacingBuilding;

	int _resources;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Hide();

		_townHall = new Building(ResourceLoader.Load<PackedScene>("res://NikkeTests/scenes/town_hall.tscn"), 100);
		_farmPlot = new Building(ResourceLoader.Load<PackedScene>("res://NikkeTests/scenes/farm_plot.tscn"), 10);
		_house = new Building(ResourceLoader.Load<PackedScene>("res://NikkeTests/scenes/house.tscn"), 40);

		_resources = 200;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(_isPlacingBuilding) 
		{
            SnapBuildingToGrid();

			if (Input.IsActionPressed("Click"))
			{
				_isPlacingBuilding = false;
                _buildingScene.Modulate = new Color(1, 1, 1, 1);

				_resources -= _currentBuilding.price;

				if(_currentBuilding == _townHall) 
				{
                    Button _townHallButton;
                    _townHallButton = GetNode("Control/VBoxContainer/town_hall") as Button;
					_townHallButton.Disabled = true;
                }
			}
			else if (Input.IsActionPressed("ui_cancel"))
			{
                _isPlacingBuilding = false;
                _buildingScene.QueueFree();
            }
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

        _buildingScene.GlobalPosition = _snapLocation;
    }

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.ButtonIndex == 0 && _isPlacingBuilding)
		{
			
		}
	}

	private void _on_build_button_toggled(bool isToggledOn)
	{
		if (isToggledOn)
		{
			Show();
		}
		else 
		{
			Hide();
            Button _buildButton = GetNode("../build_button") as Button;
			_buildButton.ReleaseFocus();
        }
	}

	private void _on_town_hall_pressed()
	{
		_currentBuilding = _townHall;
        InstantiateBuilding();
	}

	private void _on_farm_pressed()
	{
        _currentBuilding = _farmPlot;
        InstantiateBuilding();
	}


	private void _on_house_pressed()
	{
        _currentBuilding = _house;
        InstantiateBuilding();
	}

	private void InstantiateBuilding()
	{
        if (_resources < _currentBuilding.price)
        {
			Debug.WriteLine("You don't have enough resources");
			return;
        }

        _buildingScene = _currentBuilding.scene.Instantiate() as Node2D;

		Node2D _buildings = GetNode("../buildings") as Node2D;
		_buildings.AddChild(_buildingScene);

        _buildingScene.Modulate = new Color(1, 1, 1, 0.3f);

		Button _buildButton = GetNode("../build_button") as Button;
		_buildButton.ButtonPressed = false;

		_isPlacingBuilding = true;
	}
}
