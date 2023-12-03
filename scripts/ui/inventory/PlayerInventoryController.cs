using Godot;
using System.Threading.Tasks;

public partial class PlayerInventoryController : Control
{
    public static TextureRect selectedIcon;
    public static Label selectedQuantityLabel;
    public static Sprite2D heldItemIndicator;

    static Control _inventoryPanel;
    static GridContainer _inventoryGrid;
    static Control _hotbarPanel;
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

    const string HOTBAR_PANEL_GROUP = "HotbarPanel";
    const string HOTBAR_GRID_GROUP = "HotbarGrid";


    const int SELECTED_ITEM_OFFSET = 64;
    const int HOTBAR_SIZE = 8;
    public const int HOTBAR_START_IDX = StorageData.PLAYER_INVENTORY_MAX_SIZE - HOTBAR_SIZE;


    public override void _Ready()
    {
        var tree = GetTree();

        _inventoryPanel = (Control)tree.GetFirstNodeInGroup(INVENTORY_PANEL_GROUP);
        _inventoryGrid = (GridContainer)tree.GetFirstNodeInGroup(INVENTORY_GRID_GROUP);

        _hotbarPanel = (Control)tree.GetFirstNodeInGroup(HOTBAR_PANEL_GROUP);
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


    void UpdateHeldItem(int hotbarIndex)
    {
        int itemInventoryIndex = HOTBAR_START_IDX + hotbarIndex;
        heldItem = SaveData.organizedPlayerInventory[itemInventoryIndex];

        Vector2 heldItemPos = _hotbarGrid.GetChild<Control>(hotbarIndex).GlobalPosition;
        heldItemPos.X += SELECTED_ITEM_OFFSET / 2;
        heldItemPos.Y += SELECTED_ITEM_OFFSET / 2;

        heldItemIndicator.GlobalPosition = heldItemPos;
    }


    async void _InitInventory()
    {
        await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete, 100);

        foreach (var node in _inventoryGrid.GetChildren())
        {
            node.Free();
        }

        foreach (var node in _hotbarGrid.GetChildren())
        {
            node.Free();
        }

        // Init all slots with null items
        for (int i = 0; i < StorageData.PLAYER_INVENTORY_MAX_SIZE; i++)
        {
            var itemSlotNode = GD.Load<PackedScene>(StorageController.INVENTORY_SLOT_RESPATH);
            var itemSlot = itemSlotNode.Instantiate<Control>();

            // This should probably be done better, works for now
            if (IsSlotInHotbar(i))
            {
                _hotbarGrid.AddChild(itemSlot);
            }
            else
            {
                _inventoryGrid.AddChild(itemSlot);
            }

            var inventorySlot = (InventorySlot)itemSlot;

            inventorySlot?.InitializeSlot(i);
        }

        for (int i = 0; i < StorageData.PLAYER_INVENTORY_MAX_SIZE; i++)
        {
            UpdateInventorySlot(
                SaveData.organizedPlayerInventory[i],
                IsSlotInHotbar(i)
                    ? i - HOTBAR_START_IDX
                    : i
            );
        }

        UpdateHeldItem(0);
        await SaveData.SyncInventory();
        isInitialized = true;
    }


    static bool IsSlotInHotbar(int index)
    {
        return index >= HOTBAR_START_IDX;
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

    public static void UpdateSelectedItemVisuals()
    {
        Item itemResource = ItemData.GetItemById(selectedItem.id);

        Texture2D iconTexture = itemResource.IconTexture;
        selectedIcon.Texture = iconTexture;

        selectedQuantityLabel.Visible = selectedItem.quantity > 1;
        selectedQuantityLabel.Text = selectedItem.quantity.ToString();
    }


    /// <summary> Main inventory additive operation </summary>
    /// <param name="rawItem"> Includes id, name and quantity to add </param>
    /// <param name="index"> optional: desired index in the organized player inventory array </param>
    /// <param name="affectSelectedItem"> optional : affect the current selected item </param>
    /// <param name="deselectOnAllAdded"> optional : deselect current item if all items were added </param>
    /// 
    ///	<returns> how many items could *NOT* be added </returns>
    public static async Task<int> AddItemToInventory(
        RawInventoryItem rawItem,
        int index = -1,
        bool affectSelectedItem = false,
        bool deselectOnAllAdded = true)
    {
        return await StorageController.AddItem(
            ChooseGrid(index),
            SaveData.organizedPlayerInventory,
            rawItem,
            index,
            affectSelectedItem,
            deselectOnAllAdded);
    }

    /// <summary> Main inventory item removal method </summary>
    /// 
    /// <param name="rawItem"> Includes id, name and quantity to remove. </param>
    /// <param name="index"> (Optional) If specified, only removes the quantity available in that index. </param>
    ///	<returns> Whether the removal operation was successful or not </returns>
    public static async Task<bool> RemoveItemFromInventory(RawInventoryItem rawItem, int index = -1)
    {
        return await StorageController.RemoveItemFromStorage(
            ChooseGrid(index),
            SaveData.organizedPlayerInventory,
            rawItem,
            index
        );
    }

    static GridContainer ChooseGrid(int index)
    {
        return IsSlotInHotbar(index)
            ? _hotbarGrid
            : _inventoryGrid;
    }

    public static void SelectSingleItem(RawInventoryItem item, int index)
    {
        item.quantity -= 1;

        if (item.quantity >= 1)
        {
            StorageController.UpdateStorageSlot(
                ChooseGrid(index), SaveData.organizedPlayerInventory, item, index);
        }
        else
        {
            StorageController.NullifyItemAtIndex(
                ChooseGrid(index), SaveData.organizedPlayerInventory, index);
        }


        SelectNewItem(new RawInventoryItem(item.id, item.name, 1, item.stackSize));
    }

    public static void SelectInventoryItemAtSlot(int index)
    {
        InventorySlot slot = IsSlotInHotbar(index)
            ? _hotbarGrid.GetChild<InventorySlot>(index - HOTBAR_START_IDX)
            : _inventoryGrid.GetChild<InventorySlot>(index);

        SelectNewItem(slot.slotItem);
        slot.ToggleVisuals(false);
    }

    public static void SwapItems(RawInventoryItem slotItem, int index)
    {
        RawInventoryItem tempSwapItem = new(
            slotItem.id, slotItem.name, slotItem.quantity, slotItem.stackSize);

        UpdateInventorySlot(selectedItem, index);

        UpdateInventorySlot(tempSwapItem, selectedItem.indexInStorage);

        SelectInventoryItemAtSlot(selectedItem.indexInStorage);
    }

    static void UpdateInventorySlot(RawInventoryItem item, int index)
    {
        InventorySlot slotToUpdate;
        if (IsSlotInHotbar(index))
        {
            slotToUpdate = _hotbarGrid.GetChild<InventorySlot>(index - HOTBAR_START_IDX);
        }
        else
        {
            slotToUpdate = _inventoryGrid.GetChild<InventorySlot>(index);
        }

        StorageSlotController.UpdateSlot(slotToUpdate, SaveData.organizedPlayerInventory, item);
    }

    public static void DropSelectedItem(Vector2 position, Node parent)
    {
        RawInventoryItem temp = new RawInventoryItem(
            selectedItem.id, selectedItem.name, selectedItem.quantity, selectedItem.stackSize);

        CreateDroppedItem(temp, position, parent);

        if (selectedItem.HasValidIndexInArray(SaveData.organizedPlayerInventory))
        {
            int idx = selectedItem.indexInStorage;
            StorageController.NullifyItemAtIndex(ChooseGrid(idx), SaveData.organizedPlayerInventory, idx);
        }

        DeselectItem();

        // Select a new item of the same type if exists
        int index = StorageData.GetFirstStackIndexOfItem(SaveData.organizedPlayerInventory, temp.id);

        if (index == 0)
        {
            if (!StorageData.ExistsInStorage(
                    SaveData.organizedPlayerInventory, temp.id, temp.quantity + 1))
            {
                return;
            }
        }

        SelectInventoryItemAtSlot(index);
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