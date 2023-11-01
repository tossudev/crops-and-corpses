using Godot;
using System;
using System.Linq;

public partial class ItemData : Node {

    PathContainer _itemsDirectories;
    const string ITEM_PATH_CONTAINER_PATH = "res://assets/resources/game_items/game_item_paths.tres";
    public static Godot.Collections.Dictionary items = new ();

    public override void _Ready() {
        _itemsDirectories = (PathContainer) ResourceLoader.Load(ITEM_PATH_CONTAINER_PATH);

        _LoadItemsFromEachPath();
    }

    void _LoadItemsFromEachPath()
    {
        foreach (var folderPathKeeper in _itemsDirectories.paths)
        {
            _LoadItemsFromPath(folderPathKeeper.GetFolderPath());
        }
    }
    
    void _LoadItemsFromPath(string path) {

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
