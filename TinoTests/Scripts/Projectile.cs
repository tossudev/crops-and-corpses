using Godot;
using System;

public partial class Projectile : Node2D
{
    public Attack _attack;
    public float _speed;
    [Export] Timer _lifetimeTimer;
    float _lifetime = 5f;

    public override void _Ready()
    {
        TopLevel = true;
        _lifetimeTimer.Start(_lifetime);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_lifetimeTimer.TimeLeft <= 0)
            QueueFree();

        Position += _attack.direction * (float)delta * _speed;
    }

    private void OnHitboxEntered(Area2D body)
    {
        if (body is HitboxComponent hitbox)
        {
            hitbox.Damage(_attack);
        }

        QueueFree();
    }
}
