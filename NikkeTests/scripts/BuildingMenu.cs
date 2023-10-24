using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.IO;

public partial class BuildingMenu : Control
{
	Building _farmPlot, _house, _archerTower;
	Building _currentBuilding;

    List<Building> _buildingPrefabs;

    Node2D _ghostBuilding;

    [Export]
	Node2D _buildings;
    [Export]
    Button _buildButton;
    [Export]
    ScrollContainer _buildMenu;
    [Export]
    Control _buildMenuControl;
    [Export]
    VBoxContainer _vBoxContainer;

    [Export]
    PackedScene _farmPlotScene, _farmPlotGhostScene, _houseScene, _houseGhostScene, _archerTowerScene, _archerTowerGhostScene;
    [Export]
    Texture2D _farmPlotIcon, _houseIcon, _archerTowerIcon;

    [Export]
    CharacterBody2D _player;

	int _resources;

    string _savePath, _fileName;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _savePath = ProjectSettings.GlobalizePath("user://saves/");
        _fileName = "buildings.txt";

        _buildingPrefabs = new List<Building>();

        _farmPlot = new Building(_farmPlotScene, _farmPlotGhostScene, 10, "Farm Plot", _farmPlotIcon);
        _buildingPrefabs.Add(_farmPlot);

        _house = new Building(_houseScene, _houseGhostScene, 40, "House", _houseIcon);
        _buildingPrefabs.Add(_house);

        _archerTower = new Building(_archerTowerScene, _archerTowerGhostScene, 60, "Archer Tower", _archerTowerIcon);
        _buildingPrefabs.Add(_archerTower);

        _resources = 500;

        CreateBuildMenu();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            //_player.SetPhysicsProcess(true);
            //_buildMenu.Hide();
            //_buildButton.ReleaseFocus();
            _buildButton.ButtonPressed = false;
        }
    }

    private void CreateBuildMenu()
    {
        _buildMenuControl.CustomMinimumSize = new Vector2(300, 200);
        _vBoxContainer.CustomMinimumSize = new Vector2(300, 400);

        foreach (Building _building in _buildingPrefabs)
        {
            Button _button = new Button();
            _vBoxContainer.AddChild(_button);
            _button.Text = _building.name + "\n" + "Price: " + _building.price;
            _button.Icon = _building.icon;
            _button.ExpandIcon = true;
            _button.AddThemeFontSizeOverride("font_size", 32);
            _button.ButtonUp += () => OnButtonUp(_building);
        }

        for (int i = 0; i < 3; i++)        
        {
            Button _button = new Button();
            _vBoxContainer.AddChild(_button);
            _button.Text = "Building";
            _button.AddThemeFontSizeOverride("font_size", 32);
            _button.Disabled = true;
        }

        HBoxContainer _hBoxContainer = new HBoxContainer();
        _hBoxContainer.Alignment = BoxContainer.AlignmentMode.Center;
        _vBoxContainer.AddChild(_hBoxContainer);

        Button _saveButton = new Button();
        _hBoxContainer.AddChild(_saveButton);
        _saveButton.Text = "Save";
        _saveButton.AddThemeFontSizeOverride("font_size", 40);
        _saveButton.ButtonUp += () => SaveBuildings(_savePath, _fileName);

        Button _loadButton = new Button();
        _hBoxContainer.AddChild(_loadButton);
        _loadButton.Text = "Load";
        _loadButton.AddThemeFontSizeOverride("font_size", 40);
        _loadButton.ButtonUp += () => LoadBuildings(_savePath, _fileName);

        _buildMenuControl.CustomMinimumSize = new Vector2(300, _vBoxContainer.GetMinimumSize().Y + 20);
    }

    public JsonArray GetBuildings()
    {
        JsonArray _savedBuildings = new JsonArray();

        foreach (Node2D node in _buildings.GetChildren())
        {
            string name = "null";

            if (node.IsInGroup("House"))
            {
                name = "House";
            }
            else if (node.IsInGroup("FarmPlot"))
            {
                name = "FarmPlot";
            } 
            else if (node.IsInGroup("ArcherTower"))
            {
                name = "ArcherTower";
            }

            JsonObject jsonObj = new JsonObject
        {
            { "name", name },
            { "x", node.Position.X },
            { "y", node.Position.Y }
        };

            _savedBuildings.Add(jsonObj);
        }

        return _savedBuildings;
    }

    private void SaveBuildings(string path, string fileName)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Join(path, fileName);
            File.WriteAllText(path, GetBuildings().ToString());
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private void LoadBuildings(string path, string fileName)
    {
        path = Path.Join(path, fileName);

        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            JsonArray _loadedBuildings = (JsonArray)JsonArray.Parse(File.ReadAllText(path));
            InstantiateBuildings(_loadedBuildings);
        }
        catch (Exception ex) 
        {
            Debug.WriteLine(ex);
        }
    }

    public void InstantiateBuildings(JsonArray loadedBuildings)
    {
        foreach (Node2D node in _buildings.GetChildren())
        {
            node.QueueFree();
        }

        foreach (JsonObject jsonObject in loadedBuildings)
        {
            if(jsonObject["name"].ToString() == "null")
            {
                continue;
            }

            foreach(Building building in _buildingPrefabs) 
            {
                if (building.name.Replace(" ", "") == jsonObject["name"].ToString())
                {
                    _currentBuilding = building;
                }
            }

            int x = Int32.Parse(jsonObject["x"].ToString());
            int y = Int32.Parse(jsonObject["y"].ToString());

            Node2D _buildingScene = _currentBuilding.scene.Instantiate() as Node2D;           
            _buildingScene.Position = new Vector2(x, y);
            _buildings.AddChild(_buildingScene);
        }
    }

    private void OnButtonUp(Building building)
    {
        _currentBuilding = building;
        BuildingMode();
    }

    public void Build()
    {
        if (_resources < _currentBuilding.price)
        {
            Debug.WriteLine("You don't have enough resources");
            return;
        }

        Node2D _buildingScene = _currentBuilding.scene.Instantiate() as Node2D;
        _buildingScene.Position = _ghostBuilding.Position;
        _buildings.AddChild(_buildingScene);

        _resources -= _currentBuilding.price;
    }

	private void _on_build_button_toggled(bool isToggledOn)
	{
        if (isToggledOn)
		{
            _player.SetPhysicsProcess(false);
            _player.SetProcessUnhandledInput(false);
			_buildMenu.Show();
		}
		else 
		{
            _player.SetPhysicsProcess(true);
            _player.SetProcessUnhandledInput(true);
            _buildMenu.Hide();
			_buildButton.ReleaseFocus();
        }
	}

    public void EnableBuildButton()
    {
        _buildButton.Disabled = false;
    }

    private void BuildingMode()
	{
        _buildButton.Disabled = true;
        _buildMenu.Hide();
        //_buildButton.ButtonPressed = false;
        //_player.SetPhysicsProcess(false);

        //Input.MouseMode = Input.MouseModeEnum.Hidden;      

        _ghostBuilding = _currentBuilding.buildingModeScene.Instantiate() as Node2D;

        BuildingMode _buildingMode;
        _buildingMode = _ghostBuilding as BuildingMode;
        _buildingMode.buildingMenu = this;

        _buildings.AddChild(_ghostBuilding);
	}
}
