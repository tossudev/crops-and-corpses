using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public partial class ProjectileController : Node2D
{
    [Export] private Timer _lifetimeTimer;

    public Attack attack;
    public Resource projectile;
    public string targetGroup;
    public float speed;
    private float _airtime;
    private float _despawnTime;
    private EffectType _effectType;

    public void Init()
    {
        _airtime = (float)projectile.Get("airtime");
        _despawnTime = (float)projectile.Get("despawnTime");
        _effectType = (EffectType)(int)projectile.Get("effect");

        attack.effect = _effectType;

        this.TopLevel = true;
        _lifetimeTimer.Start(_airtime);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_lifetimeTimer.TimeLeft > _despawnTime)
        {
            this.Position += attack.direction * (float)delta * speed;
        }
        else if (_lifetimeTimer.TimeLeft <= 0)
        {
            QueueFree();
        }
        else
        {
            this.GetNode<Area2D>("Hitbox").SetDeferred("monitoring", false);
        }
    }

    private void OnHitboxEntered(Area2D body)
    {
        if (body is HitboxComponent hitbox)
        {
            if (hitbox.IsInGroup(targetGroup))
                hitbox.ApplyAttack(attack);
        }

        QueueFree();
        // _lifetimeTimer.Start(_despawnTime);  // for non moving objects
    }
}
