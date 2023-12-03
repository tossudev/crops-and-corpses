using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.IO;
using System.Xml.Linq;

public partial class BuildingMenu : Control
{
    public static BuildingMenu buildMenu;
    
	Building _farmPlot, _house, _archerTower, _well, _largeHouse;
	Building _currentBuilding;

    List<Building> _buildingPrefabs;

    Node2D _ghostBuilding;

	Node2D _buildings;

    Node2D _fences;

    BuildingDemolishMenu _demolishMenu;
    const string BUILDING_DEMOLISH_MENU_NODENAME = "%BuildingDemolishMenu";

    [Export]
    ScrollContainer _buildMenu;
    [Export]
    Control _buildMenuControl;
    [Export]
    VBoxContainer _vBoxContainer;
    [Export]
    Label _notEnoughResourcesLabel;

    [Export]
    PackedScene _farmPlotScene, _farmPlotGhostScene, _houseScene, _houseGhostScene, _archerTowerScene, _archerTowerGhostScene, _wellScene, _wellGhostScene, _largeHouseScene, _largeHouseGhostScene;
    [Export]
    Texture2D _farmPlotIcon, _houseIcon, _archerTowerIcon, _wellIcon, _largeHouseIcon;

    CharacterBody2D _player;

    public Item log;
    public Item copper;

    public string savePath;
    const string FILE_NAME = "buildings.txt";

    public override void _Ready()
    {
        if (!SceneManager.IsCurrentScene(this, Scene.Town))
        {
            QueueFree();
            return;
        }

        buildMenu?.QueueFree();
        buildMenu = this;

        savePath = ProjectSettings.GlobalizePath("user://saves/");

        _player = GetParent().GetParent() as CharacterBody2D;

        _buildings = GetNode("/root/Town/Buildings/SaveableBuildings") as Node2D;

        _fences = GetNode("/root/Town/Fences") as Node2D;

        _buildingPrefabs = new List<Building>();

        _farmPlot = new Building(_farmPlotScene, _farmPlotGhostScene, 1, "Farm Plot", _farmPlotIcon);
        _buildingPrefabs.Add(_farmPlot);

        _house = new Building(_houseScene, _houseGhostScene, 4, "House", _houseIcon);
        _buildingPrefabs.Add(_house);

        _largeHouse = new Building(_largeHouseScene, _largeHouseGhostScene, 6, "Large House", _largeHouseIcon);
        _buildingPrefabs.Add(_largeHouse);

        _well = new Building(_wellScene, _wellGhostScene, 2, "Well", _wellIcon);
        _buildingPrefabs.Add(_well);

        _archerTower = new Building(_archerTowerScene, _archerTowerGhostScene, 6, "Archer Tower", _archerTowerIcon);
        _buildingPrefabs.Add(_archerTower);

        _notEnoughResourcesLabel.AddThemeFontSizeOverride("font_size", 32);
        
        CreateBuildMenu();

        LoadBuildings();
    }

    public override void _ExitTree()
    {
        if (buildMenu != null && buildMenu == this)
        {
            buildMenu = null;
        }
    }

    public override void _Input(InputEvent @event) {
        if (@event.IsActionPressed("open_build_menu")) {
            if(_buildMenu.Visible == false)
            {
                OpenBuildMenu();
            }
            else
            {
                CloseBuildMenu();
            }
        }
		
        if (@event.IsActionPressed("ui_cancel")) {
            CloseBuildMenu();
        }
    }

    private void OpenBuildMenu()
    {
        _player.SetPhysicsProcess(false);
        _player.SetProcessUnhandledInput(false);
        _buildMenu.Show();

        SetPriceLabelColor();
    }

    private void SetPriceLabelColor()
    {
        foreach (Button button in _vBoxContainer.GetChildren())
        {
            Label label;

            if (button.GetChildCount() > 0)
            {
                label = button.GetChild(0).GetChild(0) as Label;
            }
            else
            {
                return;
            }

            int price = Int32.Parse(label.Text);

            if (!PlayerInventoryData.ExistsInInventory(log.ID, price))
            {
                label.SelfModulate = Colors.Red;
            }
            else
            {
                label.SelfModulate = Colors.White;
            }
        }
    }

    public void CloseBuildMenu()
    {
        _player.SetPhysicsProcess(true);
        _player.SetProcessUnhandledInput(true);
        _buildMenu.Hide();
        _notEnoughResourcesLabel.Visible = false;
    }

    private void CreateBuildMenu()
    {
        log = ItemData.GetItemById(0);
        copper = ItemData.GetItemById(2);
        
        //_buildMenuControl.CustomMinimumSize = new Vector2(600, 300);
        _vBoxContainer.CustomMinimumSize = new Vector2(600, 400);

        foreach (Building _building in _buildingPrefabs)
        {
            Button _button = new Button();
            _vBoxContainer.AddChild(_button);
            _button.CustomMinimumSize = new Vector2(400, 100);
            _button.Text = _building.name + "         ";
            _button.Icon = _building.icon;
            _button.ExpandIcon = true;
            _button.AddThemeFontSizeOverride("font_size", 32);
            _button.ButtonUp += () => OnButtonUp(_building);


            Sprite2D _sprite = new Sprite2D();
            _button.AddChild(_sprite);
            _sprite.Texture = log.IconTexture;
            _sprite.Scale = new Vector2(0.4f, 0.4f);
            _sprite.Position = new Vector2(540, 50);

            Label _label = new Label();
            _sprite.AddChild(_label);
            _label.Text = _building.price.ToString();
            _label.HorizontalAlignment = HorizontalAlignment.Right;
            _label.VerticalAlignment = VerticalAlignment.Bottom;
            _label.AnchorsPreset = 8;
            _label.Size = new Vector2(130, 130);
            _label.AddThemeFontSizeOverride("font_size", 80);
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
        _saveButton.ButtonUp += () => SaveBuildings();

        Button _loadButton = new Button();
        _hBoxContainer.AddChild(_loadButton);
        _loadButton.Text = "Load";
        _loadButton.AddThemeFontSizeOverride("font_size", 40);
        _loadButton.ButtonUp += () => LoadBuildings();

        _buildMenuControl.CustomMinimumSize = new Vector2(_buildMenuControl.CustomMinimumSize.X, _vBoxContainer.GetMinimumSize().Y + 20);
    }

    private JsonArray GetBuildings()
    {
        JsonArray _savedBuildings = new JsonArray();

        GlobalTime globaltime = GetNode<GlobalTime>("/root/GlobalTime");
        float time = globaltime.GetTime();

        JsonObject jsonObjTime = new JsonObject
        {
            { "name", "globalTime" },
            { "time", time }
        };

        _savedBuildings.Add(jsonObjTime);

        foreach (Node2D node in _buildings.GetChildren())
        {
            string name = "null";
            int buildingHealth = 100;

            if (node.IsInGroup("House"))
            {
                name = "House";

                BuildingHealth healthscript = node.GetNode("BuildingHealth") as BuildingHealth;
                buildingHealth = healthscript.buildingHealth;
            }
            else if (node.IsInGroup("LargeHouse"))
            {
                name = "LargeHouse";

                BuildingHealth healthscript = node.GetNode("BuildingHealth") as BuildingHealth;
                buildingHealth = healthscript.buildingHealth;
            }
            else if (node.IsInGroup("FarmPlot"))
            {
                name = "FarmPlot";
                string seedName = "null";
                double growthTime = 0;
                bool isGrowing = false;
                bool isTendedTo = false;
                bool isDead = false;

                if (node.FindChild("plant_slot").GetChildCount() > 0)
                {
                    //plantName = node.FindChild("plant_slot").GetChild(0).Name;

                    Plant plant = node.FindChild("plant_slot").GetChild(0) as Plant;
                    growthTime = plant.currentGrowthTime;

                    seedName = plant.seedName;
                    isGrowing = plant.growthStarted;
                    isTendedTo = plant.isTendedTo;
                    if(plant.GetGrowthState() == GrowthState.IsDead)
                    {
                        isDead = true;
                    }
                }

                JsonObject jsonObjPlant = new JsonObject
                {
                    { "name", name },
                    { "x", Mathf.RoundToInt(node.Position.X) },
                    { "y", Mathf.RoundToInt(node.Position.Y) },
                    { "plant",  seedName},
                    { "growthTime",  growthTime},
                    { "isGrowing",  isGrowing},
                    { "isTendedTo",  isTendedTo},
                    { "isDead",  isDead}
                };

                _savedBuildings.Add(jsonObjPlant);

                continue;
            } 
            else if (node.IsInGroup("ArcherTower"))
            {
                name = "ArcherTower";

                BuildingHealth healthscript = node.GetNode("BuildingHealth") as BuildingHealth;
                buildingHealth = healthscript.buildingHealth;
            }
            else if (node.IsInGroup("Well"))
            {
                name = "Well";
            }
            
            JsonObject jsonObj = new JsonObject
            {
                { "name", name },
                { "x", Mathf.RoundToInt(node.Position.X) },
                { "y", Mathf.RoundToInt(node.Position.Y) },
                { "health", buildingHealth }
            };

            _savedBuildings.Add(jsonObj);
        }

        int fenceHealth = 100;
        int i = 0;

        foreach (Node2D fence in _fences.GetChild(0).GetChildren())
        {
            BuildingHealth healthscript = fence.GetNode("%BuildingHealth") as BuildingHealth;
            fenceHealth = healthscript.buildingHealth;


            JsonObject fenceObj = new JsonObject
            {
                { "index", i },
                { "health", fenceHealth }
            };

            i++;
            _savedBuildings.Add(fenceObj);
        }



        return _savedBuildings;
    }

    public void SaveBuildings()
    {
        try
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            string path = Path.Join(savePath, FILE_NAME);
            File.WriteAllText(path, GetBuildings().ToString());
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private void LoadBuildings()
    {
        string path = Path.Join(savePath, FILE_NAME);

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
        float time = 0;

        foreach (Node2D node in _buildings.GetChildren())
        {
            node.QueueFree();
        }

        foreach (JsonObject jsonObject in loadedBuildings)
        {
            if (jsonObject["name"] == null)
            {
                break;
            }

            if (jsonObject["name"].ToString() == "globalTime")
            {
                time = (float)jsonObject["time"];
                continue;
            }

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

            if(_currentBuilding.name == "House" || _currentBuilding.name == "Large House" || _currentBuilding.name == "Archer Tower")
            {
                _demolishMenu = _buildingScene.GetNode<BuildingDemolishMenu>(BUILDING_DEMOLISH_MENU_NODENAME);
                _demolishMenu.buildingName = _currentBuilding.name;
                if(_demolishMenu.buildingNameLabel != null)
                {
                    _demolishMenu.SetBuildingName();
                }
            }

            if (jsonObject["name"].ToString() == "House" || jsonObject["name"].ToString() == "LargeHouse" || jsonObject["name"].ToString() == "ArcherTower")
            {
                BuildingHealth healthscript = _buildingScene.GetNode("BuildingHealth") as BuildingHealth;
                healthscript.loadedHealth = (int)jsonObject["health"];
            }

            if (jsonObject["name"].ToString() == "FarmPlot")
            {
                if(jsonObject["plant"].ToString() != "null")
                {
                    bool isGrowing = (bool)jsonObject["isGrowing"];
                    bool isTendedTo = (bool)jsonObject["isTendedTo"];
                    bool isDead = (bool)jsonObject["isDead"];

                    Plant(jsonObject["plant"].ToString(), double.Parse(jsonObject["growthTime"].ToString()), time, _buildingScene, isGrowing, isTendedTo, isDead);
                }
            }
        }
    }

    public void Plant(string seedName, double growthTime, float globalTime, Node2D farmPlot, bool isGrowing, bool isTendedTo, bool isDead)
    {
        FieldHandler fieldHandler = farmPlot as FieldHandler;
       fieldHandler.LoadPlant(seedName, growthTime, globalTime, isGrowing, isTendedTo, isDead); 
    }

    private void OnButtonUp(Building building)
    {
        if (!PlayerInventoryData.ExistsInInventory(log.ID, building.price))
        {
            _notEnoughResourcesLabel.Visible = true;
            return;
        }

        _currentBuilding = building;
        BuildingMode();
    }

    public async void Build()
    {
        RawInventoryItem logs = new RawInventoryItem(log.ID, log.Name, _currentBuilding.price, log.StackSize);

        if (await PlayerInventoryController.RemoveItemFromInventory(logs) == false)
        {
            GD.Print("Not enough logs!");
            return;
        }

        Node2D _buildingScene = _currentBuilding.scene.Instantiate() as Node2D;
        _buildingScene.Position = _ghostBuilding.Position;
        _buildings.AddChild(_buildingScene);

        if (_currentBuilding.name == "House" || _currentBuilding.name == "Large House" || _currentBuilding.name == "Archer Tower")
        {
            _demolishMenu = _buildingScene.GetNode<BuildingDemolishMenu>(BUILDING_DEMOLISH_MENU_NODENAME);
            _demolishMenu.buildingName = _currentBuilding.name;
            if (_demolishMenu.buildingNameLabel != null)
            {
                _demolishMenu.SetBuildingName();
            }
        }
    }

    private void BuildingMode()
	{
        _buildMenu.Hide();
        _notEnoughResourcesLabel.Visible = false;

        _ghostBuilding = _currentBuilding.buildingModeScene.Instantiate() as Node2D;

        BuildingMode _buildingMode;
        _buildingMode = _ghostBuilding as BuildingMode;
        _buildingMode.buildingMenu = this;
        _buildingMode.buildingPriceLogs = _currentBuilding.price;

        _buildings.AddChild(_ghostBuilding);
	}
}
