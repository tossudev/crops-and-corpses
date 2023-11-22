using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public partial class ProjectileController : Node2D
{
    [Export] private Timer _lifetimeTimer;
    [Export] private Sprite2D _sprite;

    public Attack attack;
    public Projectile projectile;
    public string targetGroup;
    public float speed;
    private float _airtime;
    private float _despawnTime;
    private bool _objectCollision;

    public void Init()
    {
        _sprite.Texture = projectile.item.IconTexture;
        _airtime = projectile.airtime;
        _despawnTime = projectile.despawnTime;
        _objectCollision = projectile.objectCollision;

        if (projectile.effect != EffectType.None)
        {
            attack.effect = projectile.effect;
        }

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
            if (body.IsInGroup(targetGroup))
            {
                hitbox.ApplyAttack(attack);
                QueueFree();
            }
        }
    }

    private void OnObjectEntered(Node2D body)
    {
        if (!body.IsInGroup("player") && _objectCollision)
        {
            this.GetNode<Area2D>("Hitbox").SetDeferred("monitoring", false);
            _lifetimeTimer.Stop();
            _lifetimeTimer.Start(_despawnTime);
        }
    }
}
