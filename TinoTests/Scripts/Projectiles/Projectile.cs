using Godot;
using System;

[GlobalClass, Icon("res://icon.svg")]
public partial class Projectile : Item
{
    [Export] public float lifetime { get; set; }
}
