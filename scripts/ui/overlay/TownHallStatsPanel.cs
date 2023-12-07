using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

public partial class TownHallStatsPanel : Control
{

	public static TownHallStatsPanel _thStatsPanelInstance;
	
	ProgressBar _expBar;
	const string EXP_BAR_NODENAME = "%ExpProgressBar";
	
	Label _levelLabel;
	const string LEVEL_LABEL_NODENAME = "%LevelLabel";
	
	HBoxContainer _statContainersVBoxContainer;
	const string STAT_CONTAINER_NODENAME = "%StatContainers";

	List<TownStatUiContainer> _townStatUiContainers = new ();
	PackedScene _statContainerPrefab;
	
	public override void _Ready()
	{
		_thStatsPanelInstance?.QueueFree();
		_thStatsPanelInstance = this;
		
		Initialize();

		Timer updateTimer = new Timer()
		{
			Autostart = true,
			OneShot = false,
			WaitTime = 6f
		};

		updateTimer.Timeout += UpdateAllStats;
		AddChild(updateTimer);
		
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_thStatsPanelInstance = null;
	}

	async void Initialize()
	{
		await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete);

		_statContainerPrefab = GD.Load<PackedScene>("res://scenes/ui/town_ui/town_stat_ui_container.tscn");
		
		_expBar = GetNode<ProgressBar>(EXP_BAR_NODENAME);
		_levelLabel = GetNode<Label>(LEVEL_LABEL_NODENAME);
		UpdateExpBar();
		
		_statContainersVBoxContainer = GetNode<HBoxContainer>(STAT_CONTAINER_NODENAME);

		foreach (var child in _statContainersVBoxContainer.GetChildren())
		{
			child.Free();
		}

		AddStatContainer(TownStatType.SILO_CAP);
		AddStatContainer(TownStatType.POPULATION_CAP);
		AddStatContainer(TownStatType.HOUSING);
		AddStatContainer(TownStatType.BROKEN_BUILDINGS);
	}

	void AddStatContainer(TownStatType type)
	{
		TownStatUiContainer container = (TownStatUiContainer) _statContainerPrefab.Instantiate();
		_townStatUiContainers.Add(container);
		_statContainersVBoxContainer.AddChild(container);
		
		Color modulateColor = type == TownStatType.BROKEN_BUILDINGS
			? Color.Color8(255, 255, 255, 150)
			: Color.Color8(255, 255, 255, 255);
		
		container.SetContainerTypeAndModulation(type, modulateColor);

		
		
		UpdateStat(type);
	}


	public void UpdateExpBar()
	{
		float previousLevelExp = TownManager.GetLevelRequiredExp(false);
		float nextLvlExp = TownManager.GetLevelRequiredExp(true);
		float totalExp = TownManager.currentTownStats.totalExperience;
		
		_expBar.Value = Mathf.Lerp(0, 100, (totalExp - previousLevelExp) / (nextLvlExp - previousLevelExp));

		_levelLabel.Text = TownManager.currentTownStats.townHallLevel.ToString();
	}

	public void UpdateAllStats()
	{
		foreach (var type in Enum.GetValues<TownStatType>())
		{
			UpdateStat(type);
		}
	}


	public async void UpdateStat(TownStatType type)
	{
        await TaskExtensions.SuspendWhile(() => 
	        VillagerManager.villagerManagerInstance == null || TownManager.currentTownStats == null, 150);	
		
        var currentTownStats = TownManager.currentTownStats;

        
        string statText = type switch
		{
			TownStatType.HOUSING => $"{currentTownStats.providedHomes}/{currentTownStats.populationCap}",
			
			TownStatType.POPULATION_CAP => $"{SaveData.allVillagerRawData.Count(data => data.isTownPopulation)}" +
			                               $"/{currentTownStats.populationCap}",
			
			TownStatType.SILO_CAP => $"{SaveData.townStorageItems.Count(s => s is not null)}" +
			                         $"/{StorageData.TOWN_STORAGE_SIZE}",
			
			TownStatType.BROKEN_BUILDINGS => $"{VillagerManager.villagerManagerInstance.GetAllBrokenBuildings().Count}" +
			                                 $"/{VillagerManager.villagerManagerInstance.allBuildings.Count}",
			
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};
        
        
		_townStatUiContainers
			?.Find(container => container.townStatType == type)
			.UpdateContainer(statText);
	}
}
