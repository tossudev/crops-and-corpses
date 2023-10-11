using Godot;
using System;
using System.Collections;

public partial class HandheldController : Node2D
{
    [Export] private Weapon _weapon;
    [Export] private Weapon _hand;
    [Export] private Projectile _projectile;
    [Export] private Area2D _hitbox;
    [Export] private AnimationPlayer _animationPlayer;
    [Export] private Timer _timer;
    [Export] private PackedScene _projectilePrefab;
    [Export] private Sprite2D _tempWeaponSprite;

    private Attack _attack;
    private float _damage;
    private float _knockback;
    private float _cooldown;
    private EffectType _effect;
    private TargetType _targetType;
    private float _reach;
    private bool _holdAction;
    private bool _ranged;
    private float _speed;
    private float _drawTime;

    private bool _isDrawing;
    private string _targetGroup;

    public override void _Ready()
    {
        // Init();
    }

    private void Init()
    {
        if (PlayerInventoryController.selectedItem != null)
            _weapon = WeaponData.GetWeaponByItem(PlayerInventoryController.selectedItem.id);
        else
        {
            _weapon = null;
            return;
        }

        _tempWeaponSprite.Texture = _weapon.item.IconTexture;
        _tempWeaponSprite.Visible = true;

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

        _isDrawing = false;

        _attack = new Attack
        {
            damage = _damage,
            knockback = _knockback,
            effect = _effect
        };

        switch (_targetType)
        {
            case TargetType.Enemy:
                _targetGroup = "enemy";
                break;
            case TargetType.Tree:
                _targetGroup = "tree";
                break;
            default:
                GD.Print("No target group set for weapon");
                break;
        }

        this.Scale = Vector2.One * _reach;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_ranged)
        {
            LookAt(GetGlobalMousePosition());
        }

        if (_weapon == null)
            _tempWeaponSprite.Visible = false;
    }

    // TODO: this is a temporary solution
    public void SetUpHandheld(Weapon weapon, Projectile projectile = null)
    {
        if ((bool)weapon.Get("ranged") && projectile == null)
        {
            GD.Print("No projectile set for ranged weapon");
            return;
        }

        _weapon = weapon;
        _projectile = projectile;

        Init();
    }

    public void Use(bool canMelee)
    {
        Init();

        if (_timer.TimeLeft > 0 || _isDrawing || _weapon == null)
            return;

        if (_ranged)
        {
            StartDraw();
        }
        else if (canMelee || _holdAction)
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

    public void Release()
    {
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

        _attack.damage *= power;
        projectile.attack = _attack;
        projectile.speed = _speed * power;
        projectile.projectile = _weapon.projectile;
        projectile.targetGroup = _targetGroup;

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
            if (hitbox.IsInGroup(_targetGroup))
                hitbox.ApplyAttack(_attack);
        }
    }
}
