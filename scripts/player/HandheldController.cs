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
    [Export] private StaminaComponent _staminaComponent;
    [Export] private Sprite2D _toolSprite;
    [Export] private Sprite2D _dynamicSprite;
    [Export] private PlayerSpriteController _skeleton;

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

    public bool isDrawing;
    private bool _actionHeld;
    private string _targetGroup;
    private string _attackAnim;
    private string _cooldownAnim;
    private AnimationPlayer _skeletonAnimPlayer;
    static AudioController _audioController;


    public override void _Ready()
    {
        _skeletonAnimPlayer = _skeleton.GetNode<AnimationPlayer>("AnimationPlayer");
        _audioController = GetNode<AudioController>("/root/Audio");
    }

    public void Init()
    {
        if (PlayerInventoryController.heldItem != null)
        {
            _weapon = WeaponData.GetWeaponByItemId(PlayerInventoryController.heldItem.id);
        }
        else
        {
            _weapon = null;
        }

        if (_weapon == null)
            _weapon = _hand;

        _dynamicSprite.Texture = null;
        _toolSprite.Texture = null;

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
        _attackAnim = GetAnimation(_weapon.attackAnim);
        _cooldownAnim = GetAnimation(_weapon.cooldownAnim);

        if (_ranged)
        {
            _hitbox.Monitoring = false;
        }
        else
        {
            _hitbox.Monitoring = true;
        }

        isDrawing = false;

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
            case TargetType.Building:
                _targetGroup = "building";
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

        if (GetGlobalMousePosition().X < GlobalPosition.X)
        {
            this.Scale = new Vector2(1, -1) * _reach;
        }
        else
        {
            this.Scale = Vector2.One * _reach;
        }

        if (_timer.TimeLeft <= 0)
        {
            _player.stopMovement = false;
            if (!_ranged)
                _skeleton.usingTool = false;
        }

        if (_actionHeld)
        {
            Use();
        }

        if (isDrawing && _staminaComponent.currentStamina <= 0)
        {
            ReleaseDraw();
        }
    }

    public void Use()
    {
        Init();

        if (_timer.TimeLeft > 0 || isDrawing || _weapon == null)
            return;

        if (_weapon.holdAction)
            _actionHeld = true;

        _skeleton.usingTool = true;

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
        if (_staminaComponent.currentStamina < 5)
            return;

        Vector2 direction = GetCursorVector();
        float angle = direction.Angle() * 180 / Mathf.Pi;
        angle = Mathf.Round(angle / 45) * 45;

        _toolSprite.Texture = _weapon.item?.IconTexture;

        _attack.direction = direction;

        this.RotationDegrees = angle;

        _staminaComponent.UseStamina(5);

        _animationPlayer.SpeedScale = 1 / _cooldown;
        _animationPlayer.Play(_attackAnim);
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

        if (!isDrawing)
            isDrawing = true;

        _skeleton.usingRanged = true;

        _player.speedPercent = 0.5f;
        _player.canRun = false;

        if (_attackAnim == GetAnimation(WeaponAnimation.None))
            _toolSprite.Texture = _weapon.item?.IconTexture;

        _staminaComponent.drainRate = 0.5f;
        _staminaComponent.canDrain = true;

        _animationPlayer.SpeedScale = 1 / _drawTime;
        _animationPlayer.Play(_attackAnim);
        _timer.Start(_drawTime);
        _audioController.PlayEffect("character_sounds/bow_draw.wav");
    }

    public void Release()
    {
        _actionHeld = false;

        _player.stopMovement = false;
        _staminaComponent.canDrain = false;

        _skeleton.usingTool = false;

        if (_ranged)
        {
            ReleaseDraw();
        }
    }

    public void ReleaseDraw()
    {
        if (!isDrawing)
            return;

        isDrawing = false;

        _skeleton.usingRanged = false;
        _skeleton.TurnHeadBack(false);
        _skeleton.GetBoneNode(PlayerBone.Right_Arm).Visible = true;

        _player.speedPercent = 1;
        _player.canRun = true;

        float elapsed = (float)_timer.TimeLeft;
        _timer.Stop();

        float power = 0.4f + (_drawTime - elapsed) / _drawTime * 0.6f;

        _attack.direction = GlobalPosition.DirectionTo(GetGlobalMousePosition());

        Shoot(power);

        _animationPlayer.SpeedScale = 1 / _cooldown;
        _animationPlayer.Play(_cooldownAnim);
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

        projectile.GlobalPosition = this.GlobalPosition;
        projectile.GlobalRotation = _attack.direction.Angle();

        _timer.Start(_cooldown);
        _audioController.PlayEffect("character_sounds/bow_shoot.wav");
    }

    private string GetAnimation(WeaponAnimation weaponAnimation)
    {
        switch (weaponAnimation)
        {
            case WeaponAnimation.None:
                return "idle";
            case WeaponAnimation.Swing:
                return "swing";
            case WeaponAnimation.bowDraw:
                return "bow_draw";
            case WeaponAnimation.PickaxeCooldown:
                return "pick_cd";
            case WeaponAnimation.AxeCooldown:
                return "axe_cd";
            case WeaponAnimation.SwordCooldown:
                return "sword_cd";
            case WeaponAnimation.BowCooldown:
                return "bow_cd";
            case WeaponAnimation.HandCooldown:
                return "hand_cd";
            default:
                return "idle";
        }
    }

    private void OnHitboxEntered(Area2D body)
    {
        if (body is HitboxComponent hitbox)
        {
            if (body.IsInGroup(_targetGroup))
            {
                hitbox.ApplyAttack(_attack);

                if (_targetGroup != "enemy")
                {
                    _player.stopMovement = true;
                }

                switch (_targetType)
                {
                    case TargetType.Enemy:
                        _audioController.PlayEffect("character_sounds/hit_enemy.wav");
                        break;
                    case TargetType.Tree:
                        _audioController.PlayEffect("character_sounds/hit_tree.wav");
                        break;
                    case TargetType.Rock:
                        _audioController.PlayEffect("character_sounds/rock_break.wav");
                        break;
                    case TargetType.Building:
                        _audioController.PlayEffect("character_sounds/hit_enemy.wav");
                        break;
                    default:
                        return;
                }
            }
        }
    }
}
