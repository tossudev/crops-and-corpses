using System;
using Godot;
using System.Threading.Tasks;

public partial class PlayerInventoryController : Control
{
    public static TextureRect selectedIcon;
    public static Label selectedQuantityLabel;
    public static Sprite2D heldItemIndicator;

    static Control _inventoryPanel;
    static GridContainer _inventoryGrid;
    static GridContainer _hotbarGrid;

    public static bool isItemSelected = false;
    public static bool isInitialized = false;
    public static RawInventoryItem selectedItem;
    public static RawInventoryItem heldItem;
    const string droppedItemNodePath = "res://scenes/world/dropped_item.tscn";

    public bool isOpen = false;
    Control _selectedItemNode;
    const string INVENTORY_GRID_GROUP = "InventoryGrid";
    const string INVENTORY_PANEL_GROUP = "InventoryPanel";

    const string HOTBAR_GRID_GROUP = "HotbarGrid";


    const int SELECTED_ITEM_OFFSET = 64;

    public static int heldItemIndex;
    
    public override void _Ready()
    {
        var tree = GetTree();

        _inventoryPanel = (Control)tree.GetFirstNodeInGroup(INVENTORY_PANEL_GROUP);
        _inventoryGrid = (GridContainer)tree.GetFirstNodeInGroup(INVENTORY_GRID_GROUP);

        _hotbarGrid = (GridContainer)tree.GetFirstNodeInGroup(HOTBAR_GRID_GROUP);

        _selectedItemNode = GetNode<Control>("SelectedItem");
        selectedIcon = GetNode<TextureRect>("SelectedItem/Icon");
        selectedQuantityLabel = GetNode<Label>("SelectedItem/Quantity");

        heldItemIndicator = GetNode<Sprite2D>("HeldItemIndicator");

        _inventoryPanel.Visible = false;
        _InitInventory();
    }


    public override void _Process(double delta)
    {
        UpdateSelectedItem();
    }


    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("toggle_inventory"))
        {
            isOpen = !isOpen;
            _inventoryPanel.Visible = isOpen;
        }

        if (@event.IsActionPressed("close_inventory"))
        {
            isOpen = false;
            _inventoryPanel.Visible = false;
        }

        for (int hotbarKey = 0; hotbarKey < 8; hotbarKey++)
        {
            int actualKeyNumber = hotbarKey + 1;
            if (@event.IsActionPressed("hotbar_" + actualKeyNumber.ToString()))
            {
                UpdateHeldItem(hotbarKey);
                return;
            }
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        isInitialized = false;
    }

    public static async void UpdateHeldItem(int hotbarIndex)
    {
        await TaskExtensions.SuspendWhile(() => !isInitialized);
        
        heldItem = SaveData.playerHotbarItems[hotbarIndex];
        heldItemIndex = hotbarIndex; 

        Vector2 heldItemPos = _hotbarGrid.GetChild<Control>(hotbarIndex).GlobalPosition;
        heldItemPos.X += SELECTED_ITEM_OFFSET / 2;
        heldItemPos.Y += SELECTED_ITEM_OFFSET / 2;

        heldItemIndicator.GlobalPosition = heldItemPos;
    }


    async void _InitInventory()
    {
        await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete, 100);

        StorageController.InitializeItemGridContainer(
            _inventoryGrid,
            SaveData.organizedPlayerInventory,
            StorageSlotType.PlayerInventory,
            0,
            StorageData.PLAYER_INVENTORY_SIZE - 1
            );
        
        StorageController.InitializeItemGridContainer(
            _hotbarGrid,
            SaveData.playerHotbarItems,
            StorageSlotType.Hotbar,
            0,
            StorageData.HOTBAR_SIZE - 1
            );

        UpdateHeldItem(0);
        await SaveData.SyncInventory();
        isInitialized = true;
    }


    void UpdateSelectedItem()
    {
        Vector2 mousePosition = GetGlobalMousePosition();
        mousePosition.X -= SELECTED_ITEM_OFFSET;
        mousePosition.Y -= SELECTED_ITEM_OFFSET;

        _selectedItemNode.GlobalPosition = mousePosition;
        _selectedItemNode.Visible = isItemSelected;
    }


    public static void SelectNewItem(RawInventoryItem rawItem)
    {
        if (rawItem == null)
        {
            GD.Print("Null item selected");
            return;
        }

        isItemSelected = true;
        selectedItem = rawItem;

        UpdateSelectedItemVisuals();
    }

    public static bool HasSameItemSelected(RawInventoryItem item)
    {
        if (!isItemSelected) return false;

        return selectedItem.id == item.id
               && selectedItem.hostGrid == item.hostGrid
               && selectedItem.hostArray == item.hostArray
               && selectedItem.indexInStorageArray == item.indexInStorageArray;
    }
    
    public static void UpdateSelectedItemVisuals()
    {
        Item itemResource = ItemData.GetItemById(selectedItem.id);

        Texture2D iconTexture = itemResource.IconTexture;
        selectedIcon.Texture = iconTexture;

        selectedQuantityLabel.Visible = selectedItem.quantity > 1;
        selectedQuantityLabel.Text = selectedItem.quantity.ToString();
    }


    ///  <summary> Main inventory additive operation </summary>
    ///  <param name="rawItem"> Includes id, name and quantity to add </param>
    ///  <param name="index"> optional: desired index in the organized player inventory array </param>
    ///  <param name="affectSelectedItem"> optional : affect the current selected item </param>
    ///  <param name="deselectOnAllAdded"> optional : deselect current item if all items were added </param>
    ///  <param name="prioritizeHotbar"></param>
    ///  <returns> how many items could *NOT* be added </returns>
    public static async Task<int> AddItemToHotbarOrInventory(
        RawInventoryItem rawItem,
        int index = -1,
        bool affectSelectedItem = false,
        bool deselectOnAllAdded = true,
        bool prioritizeHotbar = true)
    {
        var remaining = prioritizeHotbar
            ? await AddItemToHotbar(rawItem, index)
            : await AddItemToInventory(rawItem, index);

        if (remaining > 0)
        {
            remaining = prioritizeHotbar
                ? await AddItemToInventory(rawItem, index)
                : await AddItemToHotbar(rawItem, index);
        }

        return remaining;
    }
    
    
    
    public static async Task<int> AddItemToInventory(
        RawInventoryItem rawItem,
        int index = -1)
    {
        return await StorageController.AddItem(
            _inventoryGrid,
            SaveData.organizedPlayerInventory,
            rawItem,
            index
            );
    }
    
    public static async Task<int> AddItemToHotbar(
        RawInventoryItem rawItem,
        int index = -1
        )
    {
        return rawItem.quantity = await StorageController.AddItem(
            _hotbarGrid,
            SaveData.playerHotbarItems,
            rawItem,
            index
            );
    }

    /// <summary> Main inventory item removal method </summary>
    /// 
    /// <param name="rawItem"> Includes id, name and quantity to remove. </param>
    /// <param name="index"> (Optional) If specified, only removes the quantity available in that index. </param>
    ///	<returns> Whether the removal operation was successful or not </returns>
    public static async Task<bool> RemoveItemFromInventory(RawInventoryItem rawItem, int index = -1)
    {
        bool success = await StorageController.RemoveItemFromStorage(
            _inventoryGrid,
            SaveData.organizedPlayerInventory,
            rawItem,
            index);
        
        if (!success)
        {
            success = await StorageController.RemoveItemFromStorage(
                _hotbarGrid,
                SaveData.playerHotbarItems,
                rawItem,
                index);
        }

        return success;
    }

    public static void SwapItems(RawInventoryItem itemToSwapOut, int swapOutSlotIndex)
    {
        RawInventoryItem tempSwapItem = new(
            itemToSwapOut.id, itemToSwapOut.name, itemToSwapOut.quantity, itemToSwapOut.stackSize);
        
        tempSwapItem.hostSlotType = selectedItem.hostSlotType;
        tempSwapItem.hostArray = selectedItem.hostArray;
        tempSwapItem.hostGrid = selectedItem.hostGrid;
        tempSwapItem.indexInStorageArray = selectedItem.indexInStorageArray;

        
        selectedItem.hostSlotType = itemToSwapOut.hostSlotType;
        selectedItem.hostGrid = itemToSwapOut.hostGrid;
        selectedItem.hostArray = itemToSwapOut.hostArray;
        selectedItem.indexInStorageArray = itemToSwapOut.indexInStorageArray;
        
        StorageController.UpdateStorageSlot(selectedItem.hostGrid, selectedItem.hostArray, selectedItem, swapOutSlotIndex);
        
        
        StorageController.UpdateStorageSlot(tempSwapItem.hostGrid, tempSwapItem.hostArray, tempSwapItem, tempSwapItem.indexInStorageArray);
        StorageController.SelectItemAtSlot(tempSwapItem.hostGrid, tempSwapItem.indexInStorageArray);
    }

    public static void DropSelectedItem(Vector2 position, Node parent)
    {
        RawInventoryItem temp = new RawInventoryItem(
            selectedItem.id, selectedItem.name, selectedItem.quantity, selectedItem.stackSize);

        temp.hostSlotType = selectedItem.hostSlotType;
        temp.hostArray = selectedItem.hostArray;
        temp.hostGrid = selectedItem.hostGrid;

        CreateDroppedItem(temp, position, parent);
        
        if (selectedItem.HasValidIndexInArray())
        {
            int idx = selectedItem.indexInStorageArray;
            
            StorageController.NullifyItemAtIndex(temp.hostGrid, temp.hostArray, idx);
        }

        DeselectItem();

        // Select a new item of the same type if exists
        int index = StorageData.GetFirstStackIndexOfItem(temp.hostArray, temp.id);

        if (index == 0)
        {
            if (!StorageData.ExistsInStorage(temp.hostArray, temp.id, temp.quantity + 1))
            {
                return;
            }
        }

        StorageController.SelectItemAtSlot(temp.hostGrid, index);
    }

    /// <summary>
    /// Creates a dropped item on the ground at the specified position
    /// </summary>
    /// <param name="item"></param>
    /// <param name="atPosition"></param>
    /// <param name="toParent"> Object for which the item will be parented to</param>
    /// <returns> Reference to the created dropped item Node2D </returns>
    public static Node2D CreateDroppedItem(RawInventoryItem item, Vector2 atPosition, Node toParent)
    {
        var droppedItemNode = GD.Load<PackedScene>(droppedItemNodePath).Instantiate<Node2D>();

        droppedItemNode.GlobalPosition = atPosition;
        toParent.AddChild(droppedItemNode);

        DroppedItem script = droppedItemNode as DroppedItem;

        script?.SetItem(item);

        return droppedItemNode;
    }

    public static void DeselectItem()
    {
        selectedItem = null;
        isItemSelected = false;
    }
}