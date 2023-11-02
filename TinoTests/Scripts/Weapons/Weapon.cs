using Godot;
using Godot.Collections;
using System;

[GlobalClass, Icon("res://icon.svg")]
public partial class Weapon : Resource
{
    [ExportCategory("Global")]
    [Export] public Item item { get; set; }
    [Export] public float damage { get; set; }
    [Export] public Array<float> targetDamage { get; set; } = new Array<float> { 0, 0, 0 };
    [Export] public float knockback { get; set; }
    [Export] public float cooldown { get; set; }
    [Export] public EffectType effect { get; set; }

    [ExportCategory("Melee")]
    [Export] public float reach { get; set; } = 1;
    [Export] public bool holdAction { get; set; }

    [ExportCategory("Ranged")]
    [Export] public bool ranged { get; set; }
    [Export] public float speed { get; set; }
    [Export] public float drawTime { get; set; }
    [Export] public Projectile projectile { get; set; }
}

public enum TargetType
{
    Enemy,
    Tree,
    Rock,
}
