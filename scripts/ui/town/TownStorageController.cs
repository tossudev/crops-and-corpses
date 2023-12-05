using Godot;
using System;
using System.Threading.Tasks;

public partial class TownStorageController : Node
{

    static GridContainer _storageGrid;
    const string STORAGE_GRID_NODENAME = "%StorageGrid";

    public override void _Ready()
    {
        base._Ready();

        _storageGrid = GetNode<GridContainer>(STORAGE_GRID_NODENAME);
        
        InitializeGrid();
    }

    async void InitializeGrid()
    {
        await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete, 150);

        
        StorageController.InitializeItemGridContainer(
            _storageGrid,
            SaveData.townStorageItems,
            StorageSlotType.TownStorage,
            0,
            StorageData.TOWN_STORAGE_SIZE - 1
            );
    }
    
    /// <summary>
    /// Main Town Storage Add method
    /// </summary>
    /// <param name="rawItem"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static async Task<int> AddItemToTownStorage(
        RawInventoryItem rawItem,
        int index = -1
    )
    {
        return rawItem.quantity = await StorageController.AddItem(
            _storageGrid,
            SaveData.townStorageItems,
            rawItem,
            index
        );
    }
}
