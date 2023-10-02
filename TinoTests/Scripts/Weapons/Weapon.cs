using Godot;
using System;

[GlobalClass, Icon("res://icon.svg")]
public partial class Weapon : Item
{
    [ExportCategory("Global")]
    [Export] public float damage { get; set; }
    [Export] public float knockback { get; set; }
    [Export] public float cooldown { get; set; }

    [ExportCategory("Melee")]
    [Export] public float reach { get; set; }

    [ExportCategory("Ranged")]
    [Export] public float speed { get; set; }
    [Export] public bool ranged { get; set; }
}
