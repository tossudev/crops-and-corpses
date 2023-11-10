using Godot;
using System;
using System.Linq;

public partial class ItemData : Node {

    static PathContainer _itemsDirectories;
    const string ITEM_PATH_CONTAINER_PATH = "res://assets/resources/game_items/game_item_paths.tres";
    static bool _itemDataInitiated = false;
    public static bool itemDataInitiated => _itemDataInitiated;
    public static Godot.Collections.Dictionary items = new ();

    public static void InitiateItemData()
    {
        if (_itemDataInitiated) return;
        
        _itemsDirectories = (PathContainer) ResourceLoader.Load(ITEM_PATH_CONTAINER_PATH);
        _LoadItemsFromEachPath();
        
        _itemDataInitiated = true;
    }

    static void _LoadItemsFromEachPath()
    {
        foreach (var folderPathKeeper in _itemsDirectories.paths)
        {
            _LoadItemsFromPath(folderPathKeeper.GetFolderPath());
        }
    }
    
    static void _LoadItemsFromPath(string path) {

        using var dir = DirAccess.Open(path);
        // Open item directory
        if (dir != null) {
            dir.ListDirBegin();
            string fileName = dir.GetNext();

            // Add all items from directory to resource array
            while (fileName != "") {
                string filePath = path + fileName;
                var resource = ResourceLoader.Load(filePath);

                if (resource is Item item)
                {
                    items.Add(item.ID, item);
                }
                
                fileName = dir.GetNext();
            }
        }
    }

    public static Item GetItemById(int id)
    {
        if (items.TryGetValue(id, out var item))
        {
            return (Item)item;
        }
        
        return null;
    }
}
