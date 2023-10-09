using Godot;
using System;
using System.Collections;

public partial class WeaponController : Node2D
{
    [Export] private Resource _weapon;
    [Export] private Resource _projectile;
    [Export] private Area2D _hitbox;
    [Export] private AnimationPlayer _animationPlayer;
    [Export] private Timer _timer;
    [Export] private PackedScene _projectilePrefab;

    private Attack _attack;
    private float _damage;
    private float _knockback;
    private float _cooldown;
    private EffectType _effect;
    private float _reach;
    private bool _ranged;
    private float _speed;
    private float _drawTime;

    private bool _isDrawing = false;

    public override void _Ready()
    {
        _damage = (float)_weapon.Get("damage");
        _knockback = (float)_weapon.Get("knockback");
        _cooldown = (float)_weapon.Get("cooldown");
        _effect = (EffectType)(int)_weapon.Get("effect");
        _reach = (float)_weapon.Get("reach");
        _speed = (float)_weapon.Get("speed");
        _ranged = (bool)_weapon.Get("ranged");
        _drawTime = (float)_weapon.Get("drawTime");

        this.Scale = Vector2.One * _reach;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_ranged)
        {
            LookAt(GetGlobalMousePosition());
        }
    }

    public void SetUpHandheld(Resource weapon, Resource projectile = null)
    {
        _weapon = weapon;
        _projectile = projectile;
    }

    public void Use(bool canMelee)
    {
        if (_timer.TimeLeft > 0 || _weapon == null)
            return;

        _attack = new Attack
        {
            damage = _damage,
            knockback = _knockback,
            effect = _effect
        };

        if (_ranged && !_isDrawing)
        {
            StartDraw();
        }
        else if (canMelee)
        {
            UseMelee();
        }
    }

    private Vector2 GetCursorVector()
    {
        Vector2 cursorPosition = GetGlobalMousePosition();
        Vector2 playerPosition = GlobalPosition;
        Vector2 cursorVector = cursorPosition - playerPosition;
        cursorVector = cursorVector.Normalized();

        return cursorVector;
    }

    private void UseMelee()
    {
        Vector2 direction = GetCursorVector();
        float angle = direction.Angle() * 180 / Mathf.Pi;
        angle = Mathf.Round(angle / 45) * 45;

        _attack.direction = direction;

        this.RotationDegrees = angle;

        _animationPlayer.SpeedScale = 1 / _cooldown;
        _animationPlayer.Play("swing");
        _timer.Start(_cooldown);
    }

    private void StartDraw()
    {
        if (!_isDrawing)
            _isDrawing = true;

        _animationPlayer.SpeedScale = 1 / _drawTime;
        _animationPlayer.Play("draw");
        _timer.Start(_drawTime);
    }

    public void ReleaseDraw()
    {
        if (!_isDrawing)
            return;

        _isDrawing = false;

        float elapsed = (float)_timer.TimeLeft;
        _timer.Stop();

        float power = 0.4f + (_drawTime - elapsed) / _drawTime * 0.6f;

        _attack.direction = GlobalPosition.DirectionTo(GetGlobalMousePosition());

        Shoot(power);

        _animationPlayer.SpeedScale = 1 / _cooldown;
        _animationPlayer.Play("swing");
        _timer.Start(_cooldown);
    }

    private void Shoot(float power)
    {
        ProjectileController projectile = (ProjectileController)_projectilePrefab.Instantiate();
        GetParent().AddChild(projectile);

        _attack.damage *= power;
        projectile.attack = _attack;
        projectile.speed = _speed * power;
        projectile.projectile = _projectile;

        projectile.Init();

        // TODO: change this to be based on weapon reach or something
        projectile.GlobalPosition = this.GlobalPosition + _attack.direction * 10;
        projectile.GlobalRotation = _attack.direction.Angle();

        _timer.Start(_cooldown);
    }

    private void OnHitboxEntered(Area2D body)
    {
        if (body is HitboxComponent hitbox)
        {
            hitbox.ApplyAttack(_attack);
        }
    }
}
