using Godot;
using System;
using System.Threading.Tasks;

public partial class TownHallMenu : Control
{

    Panel _mainPanel;
	const string MAIN_PANEL_NODENAME = "%MainPanel";
	
	Panel _upgradePanel;
	const string UPGRADE_PANEL_NODENAME = "%UpgradePanel";

	Panel _occupationsPanel;
	const string OCCUPATIONS_PANEL_NODENAME = "%OccupationsPanel";

	Panel _storagePanel;
	const string STORAGE_PANEL_NODENAME = "%StoragePanel";
	
	GridContainer _upgradeGridContainer;
	const string UPGRADE_GRID_NODENAME = "%UpgradeGridContainer";
	
	// Main panel buttons
	
	Button _upgradesButton;
	const string UPGRADES_BUTTON_NODENAME = "%UpgradesButton";
	
	Button _occupationsButton;
	const string OCCUPATIONS_BUTTON_NODENAME = "%OccupationsButton";
	
	Button _storageButton;
	const string STORAGE_BUTTON_NODENAME = "%StorageButton";

	public VillagerResidence _villagerResidence;
	const string VILLAGER_RESIDENCE_NODENAME = "%VillagerGrid";

	
	public override void _Ready()
	{
		_mainPanel = GetNode<Panel>(MAIN_PANEL_NODENAME);
		_upgradePanel = GetNode<Panel>(UPGRADE_PANEL_NODENAME);
		_occupationsPanel = GetNode<Panel>(OCCUPATIONS_PANEL_NODENAME);
		_storagePanel = GetNode<Panel>(STORAGE_PANEL_NODENAME);
		
		_upgradeGridContainer = GetNode<GridContainer>(UPGRADE_GRID_NODENAME);
		_villagerResidence = GetNode<VillagerResidence>(VILLAGER_RESIDENCE_NODENAME);
		InitUpgradeList();

		// Main Panel mappings
		_upgradesButton = GetNode<Button>(UPGRADES_BUTTON_NODENAME);
		_upgradesButton.Pressed += OpenUpgradePanel;
		
		_occupationsButton = GetNode<Button>(OCCUPATIONS_BUTTON_NODENAME);
		_occupationsButton.Pressed += OpenOccupationsPanel;

		_storageButton = GetNode<Button>(STORAGE_BUTTON_NODENAME);
		_storageButton.Pressed += OpenStoragePanel;

		
		CloseAllPanels();
	}

	void CloseAllPanels()
	{
		_mainPanel.Visible = false;
		_upgradePanel.Visible = false;
		_occupationsPanel.Visible = false;
		_storagePanel.Visible = false;
	}
	
	// Main panel functions

	void OpenMainPanel()
	{
		_mainPanel.Visible = true;
		_upgradePanel.Visible = false;
		_occupationsPanel.Visible = false;
		_storagePanel.Visible = false;
	}

	
	
    void OpenUpgradePanel()
	{
		_mainPanel.Visible = false;
		_upgradePanel.Visible = true;
		_occupationsPanel.Visible = false;
		_storagePanel.Visible = false;
	}
	
    void OpenOccupationsPanel()
	{
		_mainPanel.Visible = false;
		_upgradePanel.Visible = false;
		_occupationsPanel.Visible = true;
		_storagePanel.Visible = false;
	}
	
    void OpenStoragePanel()
	{
		_mainPanel.Visible = false;
		_upgradePanel.Visible = false;
		_occupationsPanel.Visible = false;
		_storagePanel.Visible = true;
	}

    void OnTownHallInputEvent(Node viewport, InputEvent @event, int shapeIdx)
    {
	    if (@event is not InputEventMouseButton { Pressed: true } mouseEvent) return;
		
	    if (mouseEvent.ButtonIndex == MouseButton.Left)
	    {
		    OpenMainPanel();
	    }
    }

    public override void _Input(InputEvent @event)
    {

	    if (@event.IsActionPressed("close_townhall_menu"))
	    {
		    CloseAllPanels();
	    }
    }


    bool _upgradesInitiated;
	PathContainer _upgradeDirectories;
	const string UPGRADE_PATH_CONTAINER_PATH = "res://assets/resources/town_stats_upgrades/upgrade_path_container.tres";
	const string UPGRADE_SCENE_PATH = "res://scenes/ui/town_ui/town_upgrade_button.tscn";
    async void InitUpgradeList()
	{
		await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete);
		_LoadUpgradesFromEachPath((PathContainer) ResourceLoader.Load(UPGRADE_PATH_CONTAINER_PATH));
	}
	
	void _LoadUpgradesFromEachPath(PathContainer container)
	{
		foreach (var node in _upgradeGridContainer.GetChildren())
		{
			node.QueueFree();
		}
		
		foreach (var folderPathKeeper in container.paths)
		{
			_LoadUpgradesFromPath(folderPathKeeper.GetFolderPath());
		}
	}
    
    void _LoadUpgradesFromPath(string path) {

		using var dir = DirAccess.Open(path);
		// Open item directory
		if (dir != null) {
			dir.ListDirBegin();
			string fileName = dir.GetNext();

			// Add all items from directory to resource array
			while (fileName != "") {
				string filePath = path + fileName;
				var resource = ResourceLoader.Load(filePath);

				if (resource is TownUpgrade upgrade)
				{
					TownUpgradeButton upgradeButton = 
						(TownUpgradeButton) GD.Load<PackedScene>(UPGRADE_SCENE_PATH).Instantiate<Control>();
					
					upgradeButton.InitButton(upgrade);
					
					_upgradeGridContainer.AddChild(upgradeButton);
				}
                
				fileName = dir.GetNext();
			}
		}
	}
}
