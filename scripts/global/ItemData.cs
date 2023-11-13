using Godot;

public static class ItemData {

    const string ITEM_DIRECTORIES_PATH = "res://assets/resources/game_items/game_item_paths.tres";
    
    static bool _itemDataInitiated = false;
    public static bool itemDataInitiated => _itemDataInitiated;
    
    public static Godot.Collections.Dictionary items = new ();

    public static void InitiateItemData()
    {
        if (_itemDataInitiated) return;
        
        foreach (var resource in FileLoader._LoadResourcesFromEachPath(ITEM_DIRECTORIES_PATH))
        {
            if (resource is Item item)
            {
                items.Add(item.ID, item);
            }
        }
        
        _itemDataInitiated = true;
    }

    
    
    

    public static Item GetItemById(int id)
    {
        if (!_itemDataInitiated)
        {
            InitiateItemData();
        }
        
        if (items.TryGetValue(id, out var item))
        {
            return (Item)item;
        }
        
        return null;
    }
}
