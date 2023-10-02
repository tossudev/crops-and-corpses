using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class WeaponController : Node2D
{
    [Export] private Resource _weapon;
    [Export] private Area2D _hitbox;
    [Export] private AnimationPlayer _animationPlayer;
    [Export] private Timer _cooldownTimer;
    [Export] private PackedScene _projectilePrefab;

    private Attack _attack;
    private float _damage;
    private float _knockback;
    private float _cooldown;
    private float _reach;
    private float _speed;
    private bool _ranged;

    public override void _Ready()
    {
        _damage = (float)_weapon.Get("damage");
        _knockback = (float)_weapon.Get("knockback");
        _cooldown = (float)_weapon.Get("cooldown");
        _reach = (float)_weapon.Get("reach");
        _speed = (float)_weapon.Get("speed");
        _ranged = (bool)_weapon.Get("ranged");

        this.Scale = Vector2.One * _reach;
    }

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
    }

    private void Melee()
    {
        _animationPlayer.SpeedScale = 1 / _cooldown;
        _animationPlayer.Play("swing");
        _cooldownTimer.Start(_cooldown);
    }

    private void Shoot()
    {
        // Temp Quick Fix
        if (Input.IsActionPressed("left_click"))
        {
            return;
        }

        ProjectileController projectile = (ProjectileController)_projectilePrefab.Instantiate();
        projectile._attack = _attack;
        projectile._speed = _speed;
        GetParent().AddChild(projectile);

        projectile.GlobalPosition = this.GlobalPosition + _attack.direction * 10;

        _cooldownTimer.Start(_cooldown);
    }

    private void OnHitboxEntered(Area2D body)
    {
        if (body is HitboxComponent hitbox)
        {
            hitbox.Damage(_attack);
        }
    }
}
