using System;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;



public partial class UpgradeConfirmationPanel: Panel
{
    enum UnlockButtonState
    {
        Unlockable,
        Locked,
        Unlocked,
    }
    
    [Export] StyleBoxTexture unlockPossibleStyle;
    [Export] StyleBoxTexture unlockImpossibleStyle;
    [Export] StyleBoxTexture unlockedStyle;
    
    TextureRect _upgradeIconTextureRect;
    const string UPGRADE_ICON_NODENAME = "%UpgradeIcon";
	
    Label _upgradeNameLabel;
    const string UPGRADE_NAME_LABEL_NODENAME = "%UpgradeNameLabel";
    
    Button _closeButton;
    const string CLOSE_BUTTON_NODENAME = "%CloseButton";

    RichTextLabel _upgradeDescLabel;
    const string UPGRADE_DESCRIPTION_LABEL_NODENAME = "%UpgradeDescriptionLabel";
    
    Label _upgradeEffectLabel;
    const string UPGRADE_EFFECT_LABEL_NODENAME = "%EffectDescriptionLabel";
	
    Array<InventorySlot> _inventorySlotContainer = new ();
    const string INVENTORY_SLOT_CONTAINER_NODENAME = "%InventorySlotContainer";
    
    Button _unlockButton;
    const string UNLOCK_BUTTON_NODENAME = "%UnlockButton";
    
    Label _unlockButtonTextLabel;
    const string UNLOCK_BUTTON_TEXT_LABEL_NODENAME = "%UnlockText";

    TownUpgrade _currentUpgrade;
    TownUpgradeButton _currentCallerButton;
	
    
    public static UpgradeConfirmationPanel instance;
    public override void _Ready()
    {
        base._Ready();
        
        if (instance is null)
        {
            instance = this;
        }
        else
        {
            GD.PushError("2 instances of Upgrade confirmation panel, destroying newest");
            QueueFree();
        }
        
        _upgradeIconTextureRect = GetNode<TextureRect>(UPGRADE_ICON_NODENAME);
        _upgradeNameLabel = GetNode<Label>(UPGRADE_NAME_LABEL_NODENAME);
        _closeButton = GetNode<Button>(CLOSE_BUTTON_NODENAME);
        _upgradeDescLabel = GetNode<RichTextLabel>(UPGRADE_DESCRIPTION_LABEL_NODENAME);
        _upgradeEffectLabel = GetNode<Label>(UPGRADE_EFFECT_LABEL_NODENAME);
        _unlockButton = GetNode<Button>(UNLOCK_BUTTON_NODENAME);
        _unlockButtonTextLabel = GetNode<Label>(UNLOCK_BUTTON_TEXT_LABEL_NODENAME);
        _unlockButtonTextLabel = GetNode<Label>(UNLOCK_BUTTON_TEXT_LABEL_NODENAME);

        _closeButton.Pressed += ClosePanel;
        
        foreach (var node in GetNode(INVENTORY_SLOT_CONTAINER_NODENAME).GetChildren())
        {
            _inventorySlotContainer.Add(node as InventorySlot);
        }

        _unlockButton.Pressed += UnlockUpgrade;
    }
    
    public void OpenPanel(TownUpgrade upgrade, TownUpgradeButton caller)
    {
        if (upgrade == null)
        {
            GD.PushError("Upgrade was null @TownUpgradeButton.cs");
            return;
        }
		
        _currentUpgrade = upgrade;
        _currentCallerButton = caller;
		
        _upgradeIconTextureRect.Texture = _currentUpgrade.upgradeIcon;

        _upgradeNameLabel.Text = _currentUpgrade.upgradeHeader;
        _upgradeDescLabel.Text = _currentUpgrade.upgradeDescription;
        _upgradeEffectLabel.Text = _currentUpgrade.GetEffectDescription();

        SetUnlockButtonState(
            _currentUpgrade.IsUpgradeApplied() 
                ? UnlockButtonState.Unlocked 
                : UnlockButtonState.Unlockable,
            
            _unlockButton, _unlockButtonTextLabel);


        for (int i = 0; i < _inventorySlotContainer.Count; i++)
        {
            if (i < _currentUpgrade.craftingRequirements.Length)
            {
                
                if (!_inventorySlotContainer[i].slotInitiated)
                {
                    _inventorySlotContainer[i].InitiateSlot(-1);
                }
				
                Item itemResource = ItemData.GetItemById(_currentUpgrade.craftingRequirements[i].item.ID);

                Texture2D iconTexture = itemResource.IconTexture;
                _inventorySlotContainer[i].icon.Texture = iconTexture;

                _inventorySlotContainer[i].quantityLabel.Visible = itemResource.StackSize > 1;
                _inventorySlotContainer[i].quantityLabel.Text = _currentUpgrade.craftingRequirements[i].quantity.ToString();
				
                _inventorySlotContainer[i].Visible = true;
            }
            else
            {
                _inventorySlotContainer[i].Visible = false;
            }
        }
        
        Visible = true;
    }

    void ClosePanel()
    {
        Visible = false;
    }
    
    void SetUnlockButtonState(UnlockButtonState state, Button button, Label label)
    {
        switch (state)
        {
            case UnlockButtonState.Unlockable:
                button.Disabled = false;
                button.Set("theme_override_styles/normal", unlockPossibleStyle);
                label.Text = "Unlock";
                break;
            
            case UnlockButtonState.Locked:
                button.Disabled = false;
                button.Set("theme_override_styles/normal", unlockPossibleStyle);
                label.Text = "Can't afford";
                break;
            
            case UnlockButtonState.Unlocked:
                button.Disabled = true;
                button.Set("theme_override_styles/disabled", unlockPossibleStyle);
                label.Text = "Unlocked";
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public async void UnlockUpgrade()
    {
        try
        {
            if (_currentUpgrade.craftingRequirements.Any(
                    craftingRequirement => !PlayerInventoryData.ExistsInInventory(
                        craftingRequirement.item.ID,craftingRequirement.quantity)))
            {
                SetUnlockButtonState(UnlockButtonState.Locked, _unlockButton, _unlockButtonTextLabel);
                return;
            }
			
            foreach (var craftingRequirement in _currentUpgrade.craftingRequirements)
            {
                if (!await PlayerInventoryController.RemoveItemFromInventory(craftingRequirement.RequirementAsRaw()))
                {
                    throw new Exception(
                        "Failed to remove enough of item " + craftingRequirement.item.Name + " from inventory");
                }
            }
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
            return;
        }
        
        TownManager.ApplyUpgrade(_currentUpgrade);
        _currentCallerButton.ActivateUnlockedPanel();
        ClosePanel();
    }
}