using Godot;
using System;

public partial class TownStatUiContainer : Control
{
	TextureRect _statImage;
	const string IMAGE_NODENAME = "%TownStatImage";


	FloatingButtonName _floatingStatName;
	const string FLOATING_STAT_NAME_NODENAME = "%FloatingButtonNamePanel";
	
	Label _statText;
	const string TEXT_NODENAME = "%TownStatText";

	TownStatType _townStatType;
	public TownStatType townStatType => _townStatType;
	
	public override void _Ready()
	{
		base._Ready();

		_statImage = GetNode<TextureRect>(IMAGE_NODENAME);
		_floatingStatName = GetNode<FloatingButtonName>(FLOATING_STAT_NAME_NODENAME);
		
		_statText = GetNode<Label>(TEXT_NODENAME);
	}

	public void SetContainerTypeAndModulation(TownStatType type, Color color)
	{
		_townStatType = type;

		_statImage.Texture = (Texture2D) FileLoader.LoadCustomResource(townStatType switch
		{
			TownStatType.HOUSING => "res://assets/sprites/buildings/smallhouse_color.png",
			TownStatType.POPULATION_CAP => "res://assets/sprites/character/npc1/SVG/head.svg",
			TownStatType.SILO_CAP => "res://assets/sprites/Farm sprites/hay.png",
			TownStatType.BROKEN_BUILDINGS => "res://assets/sprites/buildings/smallhouse_color.png",
			_ => throw new ArgumentOutOfRangeException()
		});

		_statImage.SelfModulate = color;
		
		_floatingStatName.UpdateName(type switch
		{
			TownStatType.HOUSING => "Homes",
			TownStatType.POPULATION_CAP => "Max population",
			TownStatType.SILO_CAP => "Silo compartments filled",
			TownStatType.BROKEN_BUILDINGS => "Broken buildings",
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		});
		
	}
	
	public void UpdateContainer(string text)
	{
		_statText.Text = text;
	}
}

public enum TownStatType
{
	HOUSING,
	POPULATION_CAP,
	SILO_CAP,
	BROKEN_BUILDINGS
}
