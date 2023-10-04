using Godot;
using System.Threading;
using System.Threading.Tasks;

[GlobalClass]
public partial class PlayerInventoryData : Node {

    private const int PlayerInventorySize = 30;

    public static Godot.Collections.Array PlayerInventory = new Godot.Collections.Array();
    

    public override void _Ready() { 
        // Test adding items to the inventory
        var emptyItem = new Godot.Collections.Dictionary<string, Variant>();
        emptyItem.Add("ID", -1);
        emptyItem.Add("Quantity", 0);

        var dummyItem = new Godot.Collections.Dictionary<string, Variant>();
        dummyItem.Add("ID", 0);
        dummyItem.Add("Quantity", 5);

        
        for (int i = 0; i < 20; i++) {
            PlayerInventory.Add(emptyItem);
        }
        for (int i = 0; i < 10; i++) {
            PlayerInventory.Add(dummyItem);
        }

        // GD.Print(PlayerInventory);

        TestAsync(new CancellationTokenSource());

    }

    
    async Task TestAsync(CancellationTokenSource tokenSrc)
    {
        CancellationToken token = tokenSrc.Token;

        await Task.Delay(250, token);
        AddItemToInventory(1, 20);
        AddItemToInventory(0, 10);
        AddItemToInventory(1, 2);
        AddItemToInventory(0, 5);

        
        tokenSrc.Dispose();
    }

    public bool AddItemToInventory(int itemId, int amount)
    {
        // Obsolete until fixed?
        Item itemToAdd = ItemData.GetItemById(itemId);
        
        if (SaveData.currentInventoryItems.Exists(rawItem => rawItem.id == itemId))
        {
            SaveData.currentInventoryItems.Find(rawItem => rawItem.id == itemId)
                .quantity += amount;
        }
        else
        {
            RawInventoryItem newRawItem = new RawInventoryItem()
            {
                name = itemToAdd.Name,
                id = itemId,
                quantity = amount
            };
            
            SaveData.currentInventoryItems.Add(newRawItem);
        }

        SaveData.Save();
        return true;
    }
    
    public bool RemoveItemFromInventory(int itemId, int amount)
    {
        if (!SaveData.currentInventoryItems.Exists(rawItem => rawItem.id == itemId)) return false;
        
        SaveData.currentInventoryItems.Find(rawItem => rawItem.id == itemId)
            .quantity -= amount;

        SaveData.Save();
        return true;
    }
}
