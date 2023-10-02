using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Weapon : Node2D
{
    [Export] private Area2D _hitbox;
    [Export] private AnimationPlayer _animationPlayer;

    [Export] private Timer _cooldownTimer;
    private float _cooldown = 0.5f;

    [Export] PackedScene _projectilePrefab;
    bool _ranged = true;
    float _speed = 150f;

    Attack _attack;
    float _damage = 10f;
    float _knockback = 0f;

    public void Use(Vector2 direction)
    {
        float angle = direction.Angle() * 180 / Mathf.Pi;
        angle -= 90;

        if (_cooldownTimer.TimeLeft > 0)
            return;

        if (_ranged == false)
            angle = Mathf.Round(angle / 45) * 45;

        this.RotationDegrees = angle;

        _attack = new Attack
        {
            damage = _damage,
            knockback = _knockback,
            direction = direction
        };

        if (_ranged)
            Shoot();
        else
            Melee();

        _cooldownTimer.Start(_cooldown);
    }

    private void Melee()
    {
        _animationPlayer.SpeedScale = 1 / _cooldown;
        _animationPlayer.Play("swing");
    }

    private void Shoot()
    {
        Projectile projectile = (Projectile)_projectilePrefab.Instantiate();
        projectile._attack = _attack;
        projectile._speed = _speed;
        GetParent().AddChild(projectile);

        projectile.GlobalPosition = this.GlobalPosition + _attack.direction * 10;
    }

    private void OnHitboxEntered(Area2D body)
    {
        if (body is HitboxComponent hitbox)
        {
            hitbox.Damage(_attack);
        }
    }
}
