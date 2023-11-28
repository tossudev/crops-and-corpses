using Godot;
using System;

[GlobalClass, Icon("res://icon.svg")]
public partial class Projectile : Resource
{
    [Export] public Item item { get; set; }
    [Export] public float airtime { get; set; } = 2;
    [Export] public float despawnTime { get; set; } = 0.5f;
    [Export] public bool objectCollision { get; set; } = false;
    [Export] public float height { get; set; } = 0.5f;
    [Export] public EffectType effect { get; set; } = 0;
}
