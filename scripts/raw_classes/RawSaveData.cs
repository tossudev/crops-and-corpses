using System.Collections.Generic;
using Godot;
using Godot.Collections;

[System.Serializable]
public partial class RawSaveData : GodotObject
{
    public RawTownStats townStats = new();
    public List<TownUpgrade> appliedUpgrades = new();
    public List<TownUnlock> appliedUnlocks = new();
    public List<VillagerRawData> allVillagers = new();
    public Array<RawInventoryItem> organizedInventoryItems = new();
    public Array<RawInventoryItem> playerHotbarItems = new();
    public Array<RawInventoryItem> townStorageItems = new();
    public Dictionary playerInfo = new();

    public RawSaveData()
    {
    }

    public RawSaveData(RawTownStats townStats,
        List<TownUpgrade> appliedUpgrades,
        List<TownUnlock> appliedUnlocks,
        List<VillagerRawData> allVillagers,
        Array<RawInventoryItem> organizedInventoryItems,
        Array<RawInventoryItem> playerHotbarItems,
        Array<RawInventoryItem> townStorageItems,
        Dictionary playerInfo)
    {
        this.townStats = townStats;
        this.appliedUpgrades = appliedUpgrades;
        this.appliedUnlocks = appliedUnlocks;
        this.allVillagers = allVillagers;
        this.organizedInventoryItems = organizedInventoryItems;
        this.playerHotbarItems = playerHotbarItems;
        this.townStorageItems = townStorageItems;
        this.playerInfo = playerInfo;
    }

    public Dictionary GetFullDataDictionary()
    {
        Dictionary fullDictionary = new();

        // Town stats
        fullDictionary.Add(SaveData.TOWN_STATS_KEY, RawTownStats.GetDictionary(townStats));

        // Applied upgrades
        fullDictionary.Add(SaveData.APPLIED_TOWN_UPGRADES_KEY, TownUpgrade.GetDictionary(appliedUpgrades));

        // Applied unlocks
        fullDictionary.Add(SaveData.APPLIED_TOWN_UNLOCKS_KEY, TownManager.GetUnlockDictionary(appliedUnlocks));

        // Villager Data
        fullDictionary.Add(SaveData.VILLAGER_DATA_KEY, VillagerRawData.GetDictionary(allVillagers));



        // Town Storage Items
        fullDictionary.Add(SaveData.TOWN_STORAGE_ITEMS_KEY, RawInventoryItem.GetOrganizedItemsArray(townStorageItems));

        // Organized inventory
        fullDictionary.Add(
            SaveData.ORGANIZED_INVENTORY_ITEMS_KEY, RawInventoryItem.GetOrganizedItemsArray(organizedInventoryItems));

        // Hotbar
        fullDictionary.Add(SaveData.HOTBAR_ITEMS_KEY, RawInventoryItem.GetOrganizedItemsArray(playerHotbarItems));

        // Player info
        fullDictionary.Add(SaveData.PLAYER_INFO_KEY, PlayerInfo.GetDictionary());


        return fullDictionary;
    }
}