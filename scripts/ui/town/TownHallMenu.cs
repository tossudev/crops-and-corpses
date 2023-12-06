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
	const string CLOSE_BUTTON_NODENAME = "%CloseButton";

	Button _closeMainPanelButton;
	const string MAIN_PANEL_CLOSE_BUTTON_CONTAINER_NODENAME = "%MainCloseButtonContainer";
	
	Button _upgradesButton;
	const string UPGRADES_BUTTON_NODENAME = "%UpgradesButton";
	Button _closeUpgradesPanelButton;
	const string UPGRADES_PANEL_CLOSE_BUTTON_CONTAINER_NODENAME = "%UpgradesCloseButtonContainer";
	
	
	Button _occupationsButton;
	const string OCCUPATIONS_BUTTON_NODENAME = "%OccupationsButton";
	Button _closeOccupationsPanelButton;
	const string OCCUPATIONS_PANEL_CLOSE_BUTTON_CONTAINER_NODENAME = "%OccupationsCloseButtonContainer";
	
	Button _storageButton;
	const string STORAGE_BUTTON_NODENAME = "%StorageButton";
	Button _closeStoragePanelButton;
	const string STORAGE_PANEL_CLOSE_BUTTON_CONTAINER_NODENAME = "%StorageCloseButtonContainer";
	
	

	public VillagerResidence villagerResidence;

	public static TownHallMenu menuInstance;
	
	public override void _Ready()
	{
		if (menuInstance != null)
		{
			if (!menuInstance.IsQueuedForDeletion())
			{
				menuInstance.QueueFree();
			}
		}
		
		menuInstance = this;
		
		_mainPanel = GetNode<Panel>(MAIN_PANEL_NODENAME);
		_upgradePanel = GetNode<Panel>(UPGRADE_PANEL_NODENAME);
		_occupationsPanel = GetNode<Panel>(OCCUPATIONS_PANEL_NODENAME);
		_storagePanel = GetNode<Panel>(STORAGE_PANEL_NODENAME);
		
		_upgradeGridContainer = GetNode<GridContainer>(UPGRADE_GRID_NODENAME);
		InitUpgradeList();

		// Main Panel mappings
		_closeMainPanelButton = GetCloseButton(MAIN_PANEL_CLOSE_BUTTON_CONTAINER_NODENAME);
		_closeMainPanelButton.Pressed += CloseAllPanels;
		
		_upgradesButton = GetNode<Button>(UPGRADES_BUTTON_NODENAME);
		_upgradesButton.Pressed += OpenUpgradePanel;
		_closeUpgradesPanelButton = GetCloseButton(UPGRADES_PANEL_CLOSE_BUTTON_CONTAINER_NODENAME);
		_closeUpgradesPanelButton.Pressed += OpenMainPanel;
		
		
		_occupationsButton = GetNode<Button>(OCCUPATIONS_BUTTON_NODENAME);
		_occupationsButton.Pressed += OpenOccupationsPanel;
		_closeOccupationsPanelButton = GetCloseButton(OCCUPATIONS_PANEL_CLOSE_BUTTON_CONTAINER_NODENAME);
		_closeOccupationsPanelButton.Pressed += OpenMainPanel;
		
		
		_storageButton = GetNode<Button>(STORAGE_BUTTON_NODENAME);
		_storageButton.Pressed += OpenStoragePanel;
		_closeStoragePanelButton = GetCloseButton(STORAGE_PANEL_CLOSE_BUTTON_CONTAINER_NODENAME);
		_closeStoragePanelButton.Pressed += OpenMainPanel;
		
		CloseAllPanels();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		menuInstance = null;
	}

	Button GetCloseButton(string containerNodePath)
	{
		return GetNodeOrNull(containerNodePath)?.GetNode<Button>(CLOSE_BUTTON_NODENAME);
	}
	
	void CloseAllPanels()
	{
		_mainPanel.Visible = false;
		_upgradePanel.Visible = false;
		_occupationsPanel.Visible = false;
		_storagePanel.Visible = false;
	}
	
	// Main panel functions

	public void OpenMainPanel()
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

    public override void _Input(InputEvent @event)
    {

	    if (@event.IsActionPressed("close_townhall_menu"))
	    {
		    CloseAllPanels();
	    }
    }


    bool _upgradesInitiated;
	const string UPGRADE_DIRECTORY_PATH = "res://assets/resources/town_stats_upgrades/upgrade_path_container.tres";
	const string UPGRADE_SCENE_PATH = "res://scenes/ui/town_ui/town_upgrade_button.tscn";
    async void InitUpgradeList()
	{
		await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete);
		_LoadUpgradesFromEachPath();
	}
	
	void _LoadUpgradesFromEachPath()
	{
		foreach (var node in _upgradeGridContainer.GetChildren())
		{
			node.QueueFree();
		}
		
		foreach (var resource in FileLoader._LoadResourcesFromEachPath(UPGRADE_DIRECTORY_PATH))
		{
			if (resource is not TownUpgrade upgrade) continue;
			
			TownUpgradeButton upgradeButton = 
				(TownUpgradeButton) GD.Load<PackedScene>(UPGRADE_SCENE_PATH).Instantiate<Control>();
					
			upgradeButton.InitButton(upgrade);
					
			_upgradeGridContainer.AddChild(upgradeButton);
		}
	}
}
