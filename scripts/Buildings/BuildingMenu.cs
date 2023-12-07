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

    static bool _savingInProgress;
    
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
    public Item stone;
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

       

        _farmPlot = new Building(_farmPlotScene, _farmPlotGhostScene, 1, 1, 0, ExpGain.VERY_SMALL,  "Farm Plot", _farmPlotIcon);
        _buildingPrefabs.Add(_farmPlot);

        _house = new Building(_houseScene, _houseGhostScene, 4, 2, 2, ExpGain.SMALL,  "House", _houseIcon);
        _buildingPrefabs.Add(_house);

        _largeHouse = new Building(_largeHouseScene, _largeHouseGhostScene, 6, 3,3, ExpGain.MEDIUM,  "Large House", _largeHouseIcon);
        _buildingPrefabs.Add(_largeHouse);

        _well = new Building(_wellScene, _wellGhostScene, 1, 4,0, ExpGain.SMALL,  "Well", _wellIcon);
        _buildingPrefabs.Add(_well);

        _archerTower = new Building(_archerTowerScene, _archerTowerGhostScene, 6, 6,6, ExpGain.BIG, "Archer Tower", _archerTowerIcon);
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
        _player.SetProcessUnhandledInput(false);
        _buildMenu.Show();

        SetPriceLabelColor(0, log.ID);
        SetPriceLabelColor(1, stone.ID);
        SetPriceLabelColor(2, copper.ID);
    }

    private void SetPriceLabelColor(int child, int itemId)
    {
        foreach (Button button in _vBoxContainer.GetChildren())
        {
            Label label;

            if (button.GetChildCount() > child)
            {
                label = button.GetChild(child).GetChild(0) as Label;
            }
            else
            {
                return;
            }

            int price = Int32.Parse(label.Text);

            if (!StorageData.ExistsInInventoryOrHotbar(itemId, price))
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
        _player.SetProcessUnhandledInput(true);
        _buildMenu.Hide();
        _notEnoughResourcesLabel.Visible = false;
    }

    private void CreateBuildMenu()
    {
        log = ItemData.GetItemById(0);
        stone = ItemData.GetItemById(5);
        copper = ItemData.GetItemById(2);
        
        _vBoxContainer.CustomMinimumSize = new Vector2(600, 400);

        foreach (Building _building in _buildingPrefabs)
        {
            Button _button = new Button();
            _vBoxContainer.AddChild(_button);
            _button.CustomMinimumSize = new Vector2(400, 100);
            _button.Alignment = HorizontalAlignment.Left;
            _button.Text = " " + _building.name;
            _button.Icon = _building.icon;
            _button.ExpandIcon = true;
            _button.AddThemeFontSizeOverride("font_size", 32);
            _button.ButtonUp += () => OnButtonUp(_building);

            CreatePriceIcons(_button, new Vector2(350, 50), new Vector2(0.4f, 0.4f), new Vector2(1,1), log.IconTexture, _building.priceLogs);
            CreatePriceIcons(_button, new Vector2(450, 50), new Vector2(0.3f, 0.3f), new Vector2(1.25f, 1.25f), stone.IconTexture, _building.priceStone);
            CreatePriceIcons(_button, new Vector2(540, 50), new Vector2(0.3f, 0.3f), new Vector2(1.25f, 1.25f),copper.IconTexture, _building.priceCopper);
        }

        _buildMenuControl.CustomMinimumSize = new Vector2(_buildMenuControl.CustomMinimumSize.X, _vBoxContainer.GetMinimumSize().Y + 20);
    }

    public void CreatePriceIcons(Button button, Vector2 position, Vector2 spriteScale, Vector2 labelScale, Texture2D texture, int price)
    {
        Sprite2D _sprite = new Sprite2D();
        button.AddChild(_sprite);
        _sprite.Texture = texture;
        _sprite.Scale = spriteScale;
        _sprite.Position = position;

        Label _label = new Label();
        _sprite.AddChild(_label);
        _label.Text = price.ToString();
        _label.HorizontalAlignment = HorizontalAlignment.Right;
        _label.VerticalAlignment = VerticalAlignment.Bottom;
        _label.AnchorsPreset = 8;
        _label.Size = new Vector2(130, 130);
        _label.Scale = labelScale;
        _label.AddThemeFontSizeOverride("font_size", 80);
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

    public async void SaveBuildings()
    {
        await TaskExtensions.SuspendWhile(() => _savingInProgress);

        try
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            string path = Path.Join(savePath, FILE_NAME);

            _savingInProgress = true;
            await File.WriteAllTextAsync(path, GetBuildings().ToString());
            _savingInProgress = false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    async void LoadBuildings()
    {
        await TaskExtensions.SuspendWhile(() => _savingInProgress);
        
        string path = Path.Join(savePath, FILE_NAME);

        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            JsonArray _loadedBuildings = (JsonArray)JsonArray.Parse(await File.ReadAllTextAsync(path));
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

            _demolishMenu = _buildingScene.GetNode<BuildingDemolishMenu>(BUILDING_DEMOLISH_MENU_NODENAME);
            _demolishMenu.buildingName = _currentBuilding.name;
            if(_demolishMenu.buildingNameLabel != null)
            {
               _demolishMenu.SetBuildingName();
            }

            if (jsonObject["name"].ToString() == "House" || jsonObject["name"].ToString() == "LargeHouse" || jsonObject["name"].ToString() == "ArcherTower")
            {
                BuildingHealth healthscript = _buildingScene.GetNode("BuildingHealth") as BuildingHealth;
                healthscript.isLoaded = true;
                healthscript.loadedHealth = (int)jsonObject["health"];
                healthscript.LoadBuildingHealth((int)jsonObject["health"]);
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
        if (!StorageData.ExistsInInventoryOrHotbar(log.ID, building.priceLogs) || 
            !StorageData.ExistsInInventoryOrHotbar(stone.ID, building.priceStone) || 
            !StorageData.ExistsInInventoryOrHotbar(copper.ID, building.priceCopper))
        {
            _notEnoughResourcesLabel.Visible = true;
            return;
        }

        _currentBuilding = building;
        BuildingMode();
    }

    public async void Build()
    {
        RawInventoryItem logsRaw = new RawInventoryItem(log.ID, log.Name, _currentBuilding.priceLogs, log.StackSize);
        RawInventoryItem stoneRaw = new RawInventoryItem(stone.ID, stone.Name, _currentBuilding.priceStone, stone.StackSize);
        RawInventoryItem copperRaw = new RawInventoryItem(copper.ID, copper.Name, _currentBuilding.priceCopper, copper.StackSize);

        if (await PlayerInventoryController.RemoveItemFromInventory(logsRaw) == true)
        {
            if (await PlayerInventoryController.RemoveItemFromInventory(stoneRaw) == true)
            {
                if (await PlayerInventoryController.RemoveItemFromInventory(copperRaw) == false)
                {
                    GD.Print("Not enough stone!");
                    return;
                }
            }
        }

        Node2D _buildingScene = _currentBuilding.scene.Instantiate() as Node2D;
        _buildingScene.Position = _ghostBuilding.Position;
        _buildings.AddChild(_buildingScene);

        TownManager.GainExp(_currentBuilding.buildingExp);

        _demolishMenu = _buildingScene.GetNode<BuildingDemolishMenu>(BUILDING_DEMOLISH_MENU_NODENAME);
        _demolishMenu.buildingName = _currentBuilding.name;
        if (_demolishMenu.buildingNameLabel != null)
        {
            _demolishMenu.SetBuildingName();
        }

        if (_currentBuilding.name == "House" || _currentBuilding.name == "LargeHouse" || _currentBuilding.name == "ArcherTower")
        {
            HealthComponent _healthComponent = _buildingScene.GetNode("%HealthComponent") as HealthComponent;
            _healthComponent.SetHealth(_healthComponent.GetMaxHealth());
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
        _buildingMode.buildingPriceLogs = _currentBuilding.priceLogs;
        _buildingMode.buildingPriceStone = _currentBuilding.priceStone;
        _buildingMode.buildingPriceCopper = _currentBuilding.priceCopper;

        _buildings.AddChild(_ghostBuilding);
	}

    public void UpdateBuildingMaxHealth(BuildingType buildingType)
    {
        if(buildingType == BuildingType.House)
        {
            foreach(Node2D building in _buildings.GetChildren())
            {
                if (!building.IsInGroup("House") && !building.IsInGroup("LargeHouse"))
                {
                    return;
                }

                HealthComponent healtComponent = building.GetNode<HealthComponent>("%HealthComponent");
                int lostHealth = healtComponent.GetMaxHealth() - healtComponent.GetHealth();

                if (healtComponent.GetParent().IsInGroup("House"))
                {
                    healtComponent.SetMaxHealth(100 + SaveData.townHallStats.houseHP);
                }
                else if (healtComponent.GetParent().IsInGroup("LargeHouse"))
                {
                    healtComponent.SetMaxHealth(150 + SaveData.townHallStats.houseHP);
                }
                healtComponent.SetHealth(healtComponent.GetMaxHealth() - lostHealth);
            }
        }    
        else if (buildingType == BuildingType.Fence)
        {
            foreach (Node2D fence in _fences.GetChild(0).GetChildren())
            {
                HealthComponent healtComponent = fence.GetNode<HealthComponent>("%HealthComponent");

                int lostHealth = healtComponent.GetMaxHealth() - healtComponent.GetHealth();
                healtComponent.SetMaxHealth(100 + SaveData.townHallStats.wallHP);
                healtComponent.SetHealth(healtComponent.GetMaxHealth() - lostHealth);
            }
        }
    }
}
