using System.Collections.Generic;
using Godot;
using Godot.Collections;

[System.Serializable]
public partial class RawSaveData : GodotObject
{
    public RawTownStats townStats = new ();
    public List<TownUpgrade> appliedUpgrades = new ();
    public List<TownUnlock> appliedUnlocks = new ();
    public List<VillagerRawData> allVillagers = new ();
    public List<RawInventoryItem> inventoryItems = new ();
    public Array<RawInventoryItem> organizedInventoryItems = new ();
    public Dictionary playerInfo = new ();
    public Dictionary sceneInfo = new ();

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



        // All items in inventory
        fullDictionary.Add(SaveData.INVENTORY_ITEMS_KEY, RawInventoryItem.GetAllItemsDict(inventoryItems));

        // Organized inventory
        fullDictionary.Add(
            SaveData.ORGANIZED_INVENTORY_ITEMS_KEY, RawInventoryItem.GetOrganizedItemsArray(organizedInventoryItems));

        // Player info
        fullDictionary.Add(SaveData.PLAYER_INFO_KEY, PlayerInfo.GetDictionary());

        // Scene info
        fullDictionary.Add(SaveData.SCENE_INFO_KEY, SceneInfo.GetDictionary());


        return fullDictionary;
    }
}