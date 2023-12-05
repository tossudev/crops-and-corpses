using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

[GlobalClass]
public partial class SaveData : Node
{
    const string SAVEFOLDERNAME = "saves";
    const string SAVEFILENAME = "PlayerData.txt";
    static string directoryPath = "";
    static string fullPath = "";

    public const string TOWN_STATS_KEY = "townStats";
    public const string APPLIED_TOWN_UPGRADES_KEY = "appliedTownUpgrades";
    public const string APPLIED_TOWN_UNLOCKS_KEY = "appliedTownUnlocks";
    public const string VILLAGER_DATA_KEY = "allVillagers";
    public const string ORGANIZED_INVENTORY_ITEMS_KEY = "organizedInventoryItems";
    public const string HOTBAR_ITEMS_KEY = "playerHotbarItems";
    public const string TOWN_STORAGE_ITEMS_KEY = "townStorageItems";
    public const string PLAYER_INFO_KEY = "playerInfo";
    public const string SCENE_INFO_KEY = "sceneInfo";

    //---------Modifiable at runtime----------------------

    public static RawTownStats townHallStats = new();
    public static List<TownUpgrade> appliedUpgrades = new();
    public static List<TownUnlock> appliedUnlocks = new();
    public static List<VillagerRawData> allVillagerData = new();
    public static Array<RawInventoryItem> organizedPlayerInventory = new();
    public static Array<RawInventoryItem> playerHotbarItems = new();
    public static Array<RawInventoryItem> townStorageItems = new();
    public static Dictionary playerInfo = new();

    public static bool savingInProgress = false;
    public static bool firstLoadComplete = false;
    public static bool inventorySyncInProgress = false;
    //---------/Modifiable at runtime----------------------

    public override void _Ready()
    {
        base._Ready();
        directoryPath = ProjectSettings.GlobalizePath($"user://{SAVEFOLDERNAME}");
        fullPath = directoryPath.PathJoin(SAVEFILENAME);

        ItemData.InitiateItemData();
        WeaponData.InitiateWeaponData();
        LoadSaveDataIntoMemory();
        StorageData.AddDefaultResourcesToInventoryIfEmpty();
    }



    static async Task Save()
    {
        savingInProgress = true;
        if (string.IsNullOrEmpty(directoryPath))
        {
            GD.PrintErr("Save path not set, can't save game!");
            savingInProgress = false;
            return;
        }

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var rawSaveData = new RawSaveData(townHallStats,
            appliedUpgrades,
            appliedUnlocks,
            allVillagerData,
            organizedPlayerInventory,
            playerHotbarItems,
            townStorageItems,
            playerInfo);

        Dictionary saveDictionary = rawSaveData.GetFullDataDictionary();

        string json = Json.Stringify(saveDictionary, "\t");


        try
        {
            await File.WriteAllTextAsync(fullPath, json);
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
        }

        savingInProgress = false;
    }

    public static async Task<Dictionary> LoadData()
    {
        if (!File.Exists(fullPath)) return null;

        await TaskExtensions.SuspendWhile(() => savingInProgress);

        string data = "";

        try
        {
            data = await File.ReadAllTextAsync(fullPath);
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
        }

        Json loadedJson = new();

        Error error = loadedJson.Parse(data);

        if (error == Error.Ok) return (Dictionary)loadedJson.Data;

        GD.PrintErr(error, ": SaveData Load");
        return null;
    }

    static async void LoadSaveDataIntoMemory()
    {
        Dictionary saveData = await LoadData();

        // Town stats
        TownManager.ReadTownDataFromFile(saveData, false);

        // Storage related Data
        await RawInventoryItem.ReadStorageDataFromFile(saveData);

        // Villager Data
        await VillagerRawData.ReadVillagerDataFromFile(saveData);

        // Player Info
        PlayerInfo.LoadPlayerInfo(saveData);

        firstLoadComplete = true;
    }

    public static async void SyncAll()
    {
        await SyncInventory();
    }

    public static async Task SyncInventory()
    {
        await Save();
    }

    public static async Task SyncTownStats()
    {
        await Save();
    }

    public static async Task SyncVillagers()
    {
        await Save();
    }
}