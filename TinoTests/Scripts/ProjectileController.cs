using Godot;
using System;

public partial class ProjectileController : Node2D
{
    [Export] public Resource _projectile;  // temp export
    [Export] private Timer _lifetimeTimer;

    public Attack _attack;
    public float _speed;
    private float _lifetime;

    public override void _Ready()
    {
        _lifetime = (float)_projectile.Get("lifetime");
        this.TopLevel = true;
        _lifetimeTimer.Start(_lifetime);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_lifetimeTimer.TimeLeft <= 0)
            QueueFree();

        this.Position += _attack.direction * (float)delta * _speed;
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
