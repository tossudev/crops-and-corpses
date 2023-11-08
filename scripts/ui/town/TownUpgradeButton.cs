using Godot;
using System;
using System.Threading.Tasks;
using Godot.Collections;

public partial class TownUpgradeButton : Button
{
	TextureRect _upgradeIconTextureRect;
	const string UPGRADE_ICON_NODENAME = "%UpgradeIcon";
	
	Label _upgradeNameLabel;
	const string UPGRADE_NAME_LABEL_NODENAME = "%UpgradeNameLabel";
	
	Panel _unlockedPanel;
	const string UNLOCKED_PANEL_NODENAME = "%UnlockedPanel";
	
	Array<InventorySlot> _inventorySlotContainer = new ();
	const string INVENTORY_SLOT_CONTAINER_NODENAME = "%InventorySlotContainer";

	TownUpgrade _currentUpgrade;

	public void InitButton(TownUpgrade upgrade)
	{
		if (upgrade == null)
		{
			GD.PushError("Upgrade was null @TownUpgradeButton.cs");
			return;
		}

		_upgradeIconTextureRect = GetNode<TextureRect>(UPGRADE_ICON_NODENAME);
		_upgradeNameLabel = GetNode<Label>(UPGRADE_NAME_LABEL_NODENAME);
		_unlockedPanel = GetNode<Panel>(UNLOCKED_PANEL_NODENAME);

		HBoxContainer inventorySlotContainer = GetNode<HBoxContainer>(INVENTORY_SLOT_CONTAINER_NODENAME);
			
		foreach (var node in inventorySlotContainer.GetChildren())
		{
			if (node is not InventorySlot slot) return;
			_inventorySlotContainer.Add(slot);
			slot.InitiateSlot(-1);
		}
		
		_currentUpgrade = upgrade;
		
		_upgradeIconTextureRect.Texture = _currentUpgrade.upgradeIcon;

		_upgradeNameLabel.Text = _currentUpgrade.upgradeHeader;

		for (int i = 0; i < _inventorySlotContainer.Count; i++)
		{
			if (i < _currentUpgrade.craftingRequirements.Length)
			{
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

		if (upgrade.IsUpgradeApplied())
		{
			_unlockedPanel.Visible = true;
		}
	}

	public void OnButtonDown()
	{
		UpgradeConfirmationPanel.instance?.OpenPanel(_currentUpgrade, this);
	}

	public void ActivateUnlockedPanel()
	{
		_unlockedPanel.Visible = true;
	}
}
