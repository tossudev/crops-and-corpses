using Godot;
using System;
using System.Diagnostics;

public partial class BuildingMenu : ScrollContainer
{
	Building _farmPlot, _house;
	Building _currentBuilding;

    Node2D _ghostBuilding;
	Node2D _buildings;

    Button _buildButton;

	int _resources;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_farmPlot = new Building(ResourceLoader.Load<PackedScene>("res://NikkeTests/scenes/farm_plot.tscn"), ResourceLoader.Load<PackedScene>("res://NikkeTests/scenes/farm_plot_build_mode.tscn"), 10);
		_house = new Building(ResourceLoader.Load<PackedScene>("res://NikkeTests/scenes/house.tscn"), ResourceLoader.Load<PackedScene>("res://NikkeTests/scenes/house_build_mode.tscn"),  40);

        _buildings = GetNode("../buildings") as Node2D;
        _buildButton = GetNode("../build_button") as Button;

        _resources = 500;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionPressed("ui_cancel"))
        {
            Hide();
            _buildButton.ReleaseFocus();
            _buildButton.ButtonPressed = false;
        }
    }

    public void Build()
    {
        if (_resources < _currentBuilding.price)
        {
            Debug.WriteLine("You don't have enough resources");
            return;
        }

        Node2D _buildingScene = _currentBuilding.scene.Instantiate() as Node2D;
        _buildings.AddChild(_buildingScene as Node2D);
        _buildingScene.Position = _ghostBuilding.Position;

        _resources -= _currentBuilding.price;
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
			_buildButton.ReleaseFocus();
        }
	}

	private void _on_farm_pressed()
	{
        _currentBuilding = _farmPlot;
        BuildingMode();
	}


	private void _on_house_pressed()
	{
        _currentBuilding = _house;
        BuildingMode();
	}

    public void EnableBuildButton()
    {
        _buildButton.Disabled = false;
    }

    private void BuildingMode()
	{
        _buildButton.Disabled = true;
        _buildButton.ButtonPressed = false;

        Input.MouseMode = Input.MouseModeEnum.Hidden;

        _ghostBuilding = _currentBuilding.buildingModeScene.Instantiate() as Node2D;

        BuildingMode _buildingMode;
        _buildingMode = _ghostBuilding as BuildingMode;
        _buildingMode.buildingMenu = this;

        _buildings.AddChild(_ghostBuilding);
	}
}
