using Godot;
using System;

public partial class TownStorageController : Node
{

    GridContainer _storageGrid;
    const string STORAGE_GRID_NODENAME = "%StorageGrid";

    public override void _Ready()
    {
        base._Ready();

        _storageGrid = GetNode<GridContainer>(STORAGE_GRID_NODENAME);
    }

    async void InitializeGrid()
    {
        await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete, 150);

        
        foreach (var node in _storageGrid.GetChildren())
        {
            node.Free();
        }
        
        
    }
}
