using Godot;
using Godot.Collections;
using System;

public partial class Attack : Node2D
{
	public int damage = 0;
	public float knockback = 0f;
	public Vector2 direction = Vector2.Zero;
	public EffectType effect = EffectType.None;
}

public enum EffectType
{
	None,
	Cure,
	Repair,
	CaveEntrance,
}
