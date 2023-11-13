using Godot;
using System;

public partial class VillagerInfo : Control
{
	ColorRect _backgroundColor;
	TextureRect _villagerTexture;
	RichTextLabel _nameText;
	RichTextLabel _loreText;
	RichTextLabel _statusText;
	Button _healButton;
	Button _closeButton;
	
	
	public override void _Ready()
	{
		_backgroundColor = GetNode<ColorRect>("ColorRect");

		_villagerTexture = GetNode<TextureRect>("ColorRect/VillagerTexture");
		_nameText = GetNode<RichTextLabel>("ColorRect/NameText");
		_loreText = GetNode<RichTextLabel>("ColorRect/LoreText");
		_statusText = GetNode<RichTextLabel>("ColorRect/StatusText");
		_healButton = GetNode<Button>("ColorRect/HealButton");
		_closeButton = GetNode<Button>("ColorRect/CloseButton");

		Visible = false;
	}

	public void InitializeVillagerInfo(Texture2D villagerTexture, string villagerName, string villagerLore, VillagerManager.VillagerStates villagerState){

		_villagerTexture.Texture = villagerTexture;
		_nameText.Text = villagerName;
		_loreText.Text = villagerLore;
		UpdateStatus(villagerState);
		
	}


	public void UpdateStatus(VillagerManager.VillagerStates villagerState){
		_healButton.Visible=false;
		switch(villagerState){
			case VillagerManager.VillagerStates.RoamAround:
				_statusText.Text = "Status: Healthy";
				break;
			case VillagerManager.VillagerStates.FollowPlayer:
				_statusText.Text = "Status: Following";
				break;
			case VillagerManager.VillagerStates.FixFence:
				_statusText.Text = "Status: Repairing";
				break;
			case VillagerManager.VillagerStates.FindArcherTower:
				_statusText.Text = "Status: Defending";
				break;
			case VillagerManager.VillagerStates.FindShelter:
				_statusText.Text = "Status: Finding Cover";
				break;
			case VillagerManager.VillagerStates.GetHospitalized:
				_statusText.Text = "Status: Injured";
				_healButton.Visible=true;
				break;
			case VillagerManager.VillagerStates.ChooseTask:
				_statusText.Text = "Status: Healthy";
				break;
			case VillagerManager.VillagerStates.FarmingTask:
				_statusText.Text = "Status: Farming";
				break;
			
			case VillagerManager.VillagerStates.FindWoodTask:
				_statusText.Text = "Status: Cutting wood";
				break;
			
			case VillagerManager.VillagerStates.FindStoneTask:
				_statusText.Text = "Status: Mining stone";
				break;
		}
	}
	void CloseInfo(){
		Visible = false;
	}
}
