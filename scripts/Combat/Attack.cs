using Godot;
using Godot.Collections;
using System;

public partial class Attack : Node2D
{
	public float damage = 0f;
	public float knockback = 0f;
	public Vector2 direction = Vector2.Zero;
	public EffectType effect = EffectType.None;
}

public enum EffectType
{
	None,
	Cure,
}
