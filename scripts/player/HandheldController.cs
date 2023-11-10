using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Godot.Collections;

public partial class HandheldController : Node2D
{
    [Export] private Weapon _weapon;
    [Export] private Weapon _hand;
    [Export] private Area2D _hitbox;
    [Export] private AnimationPlayer _animationPlayer;
    [Export] private Timer _timer;
    [Export] private PackedScene _projectilePrefab;
    [Export] private PlayerController _player;

    private Attack _attack;
    private int _damage;
    private float _knockback;
    private float _cooldown;
    private EffectType _effect;
    private TargetType _targetType;
    private float _reach;
    private bool _holdAction;
    private bool _ranged;
    private float _speed;
    private float _drawTime;
    private Projectile _projectile;

    private bool _isDrawing;
    private bool _actionHeld;
    private string _targetGroup;

    private void Init()
    {
        if (PlayerInventoryController.heldItem != null)
        {
            _weapon = WeaponData.GetWeaponByItem(PlayerInventoryController.heldItem.id);
        }
        else
        {
            _weapon = null;
        }

        if (_weapon == null)
            return;

        _damage = _weapon.damage;
        _knockback = _weapon.knockback;
        _cooldown = _weapon.cooldown;
        _effect = _weapon.effect;
        _targetType = _weapon.targetType;
        _reach = _weapon.reach;
        _holdAction = _weapon.holdAction;
        _speed = _weapon.speed;
        _ranged = _weapon.ranged;
        _drawTime = _weapon.drawTime;
        _projectile = _weapon.projectile;

        _isDrawing = false;

        switch (_targetType)
        {
            case TargetType.Enemy:
                _targetGroup = "enemy";
                break;
            case TargetType.Tree:
                _targetGroup = "tree";
                break;
            case TargetType.Rock:
                _targetGroup = "rock";
                break;
            default:
                return;
        }

        _attack = new Attack
        {
            damage = _damage,
            knockback = _knockback,
            effect = _effect
        };

        this.Scale = Vector2.One * _reach;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_ranged)
        {
            LookAt(GetGlobalMousePosition());
        }

        if (_timer.TimeLeft <= 0)
            _player.SetPhysicsProcess(true);

        if (_actionHeld)
        {
            Use();
        }
    }

    public void Use()
    {
        Init();

        if (_timer.TimeLeft > 0 || _isDrawing || _weapon == null)
            return;

        if (_weapon.holdAction)
            _actionHeld = true;

        if (_ranged)
        {
            StartDraw();
        }
        else
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

    async void StartDraw()
    {
        RawInventoryItem projectile = new RawInventoryItem(_projectile.item.ID, _projectile.item.Name, 1, _projectile.item.StackSize);

        if (await PlayerInventoryController.RemoveItemFromInventory(projectile) == false)
        {
            GD.Print("Handheld: out of ammo");
            return;
        }

        if (!_isDrawing)
            _isDrawing = true;

        _animationPlayer.SpeedScale = 1 / _drawTime;
        _animationPlayer.Play("draw");
        _timer.Start(_drawTime);
    }

    public void Release()
    {
        _actionHeld = false;
        _player.SetPhysicsProcess(true);

        if (_ranged)
        {
            ReleaseDraw();
        }
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

        _attack.damage *= Mathf.RoundToInt(power);
        projectile.attack = _attack;
        projectile.speed = _speed * power;
        projectile.projectile = _projectile;
        projectile.targetGroup = _targetGroup;

        projectile.Init();

        // TODO: change this to be based on weapon reach or something
        projectile.GlobalPosition = this.GlobalPosition;
        projectile.GlobalRotation = _attack.direction.Angle();

        _timer.Start(_cooldown);
    }

    private void OnHitboxEntered(Area2D body)
    {
        if (body is HitboxComponent hitbox)
        {
            if (body.IsInGroup(_targetGroup))
            {
                hitbox.ApplyAttack(_attack);
                _player.SetPhysicsProcess(false);
            }
        }
    }
}
