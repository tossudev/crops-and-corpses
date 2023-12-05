using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot.Collections;

[GlobalClass]
public partial class TownManager : Node2D
{
	const string DEFAULT_STATS_PATH = "res://assets/resources/town_stats_upgrades/town_starting_stats.tres";
	
	public const int ONE_SECOND_IN_TICKS = 60;

	static int _globalPhysicsTicks;
	public static int globalPhysicsTicks => _globalPhysicsTicks;

	public static RawTownStats currentTownStats => SaveData.townHallStats;

	static Vector2 _townHallPosition;
	public static Vector2 townHallPosition => _townHallPosition;

	
	const string TOWN_STREET_SIGN_GROUP = "StreetSign";

	static PlayerTravel _townPlayerTravel;
	public static PlayerTravel GetTownPlayerTravel (Node caller)
	{
		return _townPlayerTravel ??= (PlayerTravel) caller.GetTree().GetFirstNodeInGroup(TOWN_STREET_SIGN_GROUP);;
	}

	public static void SetTownHallPosition(Vector2 position)
	{
		_townHallPosition = position;
	}

	public static async void ReadTownDataFromFile(Dictionary saveData, bool sync = true)
	{
		if (await RawTownStats.AssignStatsDataFromDictionary(saveData, sync)) return;

		TownStats townStats = (TownStats) FileLoader.LoadCustomResource(DEFAULT_STATS_PATH);
		
		SaveData.townHallStats = townStats?.TownStatsAsRaw();
		if (sync) await SaveData.SyncTownStats();
	}

	public static bool EveryXSecond(int second)
	{
		return globalPhysicsTicks % (ONE_SECOND_IN_TICKS * second) == 0;
	}
	
	public static void GainExp(ExpGain amount)
	{
		SaveData.townHallStats.totalExperience += (int) amount;

		int currentTownHallLevel = SaveData.townHallStats.townHallLevel;
		switch (SaveData.townHallStats.totalExperience)
		{
			case > (int) TownHallLevelExpRequirement.LEVEL_5 when currentTownHallLevel < 5:
				// best unlocks
				ApplyUnlock(TownUnlock.POP_CAP_LEVEL_5);

				SaveData.townHallStats.townHallLevel = 5;
				break;
			
			case > (int) TownHallLevelExpRequirement.LEVEL_4 when currentTownHallLevel < 4:
				// very nice unlocks
				ApplyUnlock(TownUnlock.POP_CAP_LEVEL_4);

				SaveData.townHallStats.townHallLevel = 4;
				break;
			
			case > (int) TownHallLevelExpRequirement.LEVEL_3 when currentTownHallLevel < 3:
				// good unlocks
				ApplyUnlock(TownUnlock.POP_CAP_LEVEL_3);

				SaveData.townHallStats.townHallLevel = 3;
				break;
			
			case > (int) TownHallLevelExpRequirement.LEVEL_2 when currentTownHallLevel < 2:
				// decent unlocks
				ApplyUnlock(TownUnlock.POP_CAP_LEVEL_2);

				SaveData.townHallStats.townHallLevel = 2;
				break;
			
			case > (int) TownHallLevelExpRequirement.LEVEL_1 when currentTownHallLevel < 1:
				
				ApplyUnlock(TownUnlock.POP_CAP_LEVEL_1);
				
				SaveData.townHallStats.townHallLevel = 1;
				break;
		}
		
		TownHallStatsPanel._thStatsPanelInstance?.UpdateExpBar();
	}
    
	public static int GetLevelRequiredExp(bool nextLevel)
	{
		var requiredExpLastLevel = 0;
		
		foreach (var requirement in Enum.GetValues<TownHallLevelExpRequirement>())
		{
			var requiredExp = (int) requirement;

			if (currentTownStats.totalExperience < requiredExp)
			{
				return nextLevel ? requiredExp : requiredExpLastLevel;
			}

			requiredExpLastLevel = requiredExp;
		}

		return (int) Enum.GetValues<TownHallLevelExpRequirement>().Last();
	}
	
	public static void ApplyUnlock(TownUnlock unlock)
	{
		if (SaveData.appliedUnlocks.Contains(unlock)) return;
		
		switch (unlock)
		{
			case TownUnlock.POP_CAP_LEVEL_1:
				SaveData.townHallStats.populationCap = 5;
				break;
			
			case TownUnlock.POP_CAP_LEVEL_2:
				SaveData.townHallStats.populationCap = 10;
				break;
			
			case TownUnlock.POP_CAP_LEVEL_3:
				SaveData.townHallStats.populationCap = 25;
				break;
			
			case TownUnlock.POP_CAP_LEVEL_4:
				SaveData.townHallStats.populationCap = 50;
				break;
			
			case TownUnlock.POP_CAP_LEVEL_5:
				SaveData.townHallStats.populationCap = 100;
				break;
			
			case TownUnlock.DIY_BRIDGE_UNLOCK:
				SaveData.townHallStats.isDIYBridgeBuilt = true;
				break;
			
			case TownUnlock.RUINS_UNLOCK:
				SaveData.townHallStats.isRuinsUnlocked = true;
				break;
			
			case TownUnlock.MINESHAFT_UNLOCK:
				SaveData.townHallStats.isMineshaftUnlocked = true;
				break;
			
			case TownUnlock.STALAGMITE_UNLOCK:
				SaveData.townHallStats.isCaveStalagmiteMined = true;
				break;
			
			default:
				throw new ArgumentOutOfRangeException();
		}
		
		SaveData.appliedUnlocks.Add(unlock);
		TownHallStatsPanel._thStatsPanelInstance.UpdateAllStats();
		Task sync = SaveData.SyncTownStats();
	}
	
	public static void ApplyUpgrade(TownUpgrade upgrade)
	{
		if (upgrade == null)
		{
			GD.PushError("Upgrade was null");
			return;
		}
		
		if (SaveData.appliedUpgrades.Any(townUpgrade => townUpgrade.id == upgrade.id)) return;
		
		switch (upgrade)
		{
			case FarmerUpgrade farmerUpgrade:
				
				SaveData.townHallStats.farmerWalkSpeed += farmerUpgrade.farmerWalkSpeed;
				break;
			
			
			case HousingUpgrade housingUpgrade:
				
				SaveData.townHallStats.houseHP += housingUpgrade.houseHP;
				break;

			
			case SoldierUpgrade soldierUpgrade:

				SaveData.townHallStats.soldierAccuracy += soldierUpgrade.soldierAccuracy;
				SaveData.townHallStats.soldierAttackSpeed += soldierUpgrade.soldierAttackSpeed;
				break;
			
			case StorageUpgrade storageUpgrade:
				
				break;
			case WallUpgrade wallUpgrade:

				SaveData.townHallStats.wallHP = wallUpgrade.wallHP;
				break;
		}
		
		SaveData.appliedUpgrades.Add(upgrade);
		TownHallStatsPanel._thStatsPanelInstance.UpdateAllStats();

		Task sync = SaveData.SyncTownStats();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		_globalPhysicsTicks++;
	}

	public static Dictionary GetUnlockDictionary(List<TownUnlock> unlocks)
	{
		Dictionary unlockDict = new();

		foreach (var unlock in unlocks)
		{
			unlockDict.Add((int) unlock, unlock.ToString());
		}

		return unlockDict;
	}
}

public enum TownHallLevelExpRequirement
{
	LEVEL_1 = 500,
	LEVEL_2 = 1500,
	LEVEL_3 = 3000,
	LEVEL_4 = 5000,
	LEVEL_5 = 10000
}

public enum TownUnlock
{
	POP_CAP_LEVEL_1,
	POP_CAP_LEVEL_2,
	POP_CAP_LEVEL_3,
	POP_CAP_LEVEL_4,
	POP_CAP_LEVEL_5,
	DIY_BRIDGE_UNLOCK,
	RUINS_UNLOCK,
	MINESHAFT_UNLOCK,
	STALAGMITE_UNLOCK
}

public enum AutosaveIntervalSeconds
{
	VILLAGER_POSITION_INTERVAL = 30
}