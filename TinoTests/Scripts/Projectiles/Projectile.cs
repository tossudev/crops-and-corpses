using Godot;
using System;

[GlobalClass, Icon("res://icon.svg")]
public partial class Projectile : Item
{
    [Export] public float airtime { get; set; } = 2;
    [Export] public float despawnTime { get; set; } = 0.5f;
}
