using Godot;
using System;
using System.Diagnostics;

public partial class ArcherTower : Node2D
{
	[Export]
    Timer _attackTimer;

    [Export]
	PackedScene _projectilePrefab;

    [Export]
    Projectile _projectile;

    [Export]
    Node2D _projectileStartPosition;

    public float power;

    Attack _attack;
    float _speed;
    string _targetGroup;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        power = 1f;
        _speed = 800;
        _targetGroup = "enemy";

        _attack = new Attack
        {
            damage = 20,
            knockback = 200,
            effect = 0
        };

        _attackTimer.Start();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void OnShootTimerTimeout()
	{
        if (!FindTarget())
            return;

        ProjectileController projectile = (ProjectileController)_projectilePrefab.Instantiate();
        AddChild(projectile);

        _attack.damage *= power;
        projectile.attack = _attack;
        projectile.speed = _speed * power;
        projectile.projectile = _projectile;
        projectile.targetGroup = _targetGroup;

        projectile.Init();

        projectile.GlobalPosition = _projectileStartPosition.GlobalPosition + _attack.direction * 10;
        projectile.GlobalRotation = _attack.direction.Angle();
    }

    private bool FindTarget()
    {
        Node2D _closestEnemy = null;

        foreach(Node2D enemy in GetTree().GetNodesInGroup("enemy"))
        {
            Node2D _parent = enemy.GetParent() as Node2D;

            if(_closestEnemy == null)
            {
                _closestEnemy = _parent;
            }

            if (_projectileStartPosition.GlobalPosition.DistanceTo(_closestEnemy.GlobalPosition) > _projectileStartPosition.GlobalPosition.DistanceTo(_parent.GlobalPosition))
            {
                _closestEnemy = _parent;
            }
        }

        if (_closestEnemy == null) 
        {
            return false;
        }
        else
        {
            _attack.direction = _projectileStartPosition.GlobalPosition.DirectionTo(_closestEnemy.GlobalPosition);
            return true;
        }

        
        Node2D _player = GetNode("/root/Town/Objects/Player") as Node2D;
        _attack.direction = _projectileStartPosition.GlobalPosition.DirectionTo(_player.GlobalPosition);

        return true;
    }
}
