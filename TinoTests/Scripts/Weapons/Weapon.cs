using Godot;
using System;

[GlobalClass, Icon("res://icon.svg")]
public partial class Weapon : Item
{
    [ExportCategory("Global")]
    [Export] public float damage { get; set; }
    [Export] public float knockback { get; set; }
    [Export] public float cooldown { get; set; }
    [Export] public EffectType effect { get; set; }
    [Export] public TargetType targetType { get; set; }

    [ExportCategory("Melee")]
    [Export] public float reach { get; set; } = 1;
    [Export] public bool holdAction { get; set; }

    [ExportCategory("Ranged")]
    [Export] public bool ranged { get; set; }
    [Export] public float speed { get; set; }
    [Export] public float drawTime { get; set; }
}

public enum TargetType
{
    None,
    Enemy,
    Tree,
}
