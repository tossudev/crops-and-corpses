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

    Attack _attack;
    float _speed;
    string _targetGroup;
    float _attackRange;

    int _attackSpeed;
    int _accuracy;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _speed = 800;
        _targetGroup = "enemy";
        _attackRange = _speed * (_projectile.airtime - _projectile.despawnTime);

        _attack = new Attack
        {
            damage = 10,
            knockback = 200,
            effect = 0
        };

        if (TownManager.currentTownStats.soldierAttackSpeed != 0)
        {
            _attackSpeed = TownManager.currentTownStats.soldierAttackSpeed;
            _attackTimer.WaitTime = 1f / _attackSpeed;
        }

        if (TownManager.currentTownStats.soldierAccuracy != 0)
        {
            _accuracy = TownManager.currentTownStats.soldierAccuracy;
            _speed = _accuracy * _speed;
            _attackRange = _speed * (_projectile.airtime - _projectile.despawnTime);
        }


        // Remove this when villager "jobs" are added
        _attackTimer.Start();
    }


    public void ActivateTower()
    {
        _attackTimer.Start();
    }

    public void DeactivateTower()
    {
        _attackTimer.Stop();
    }

    public void OnShootTimerTimeout()
	{
        if (!FindTarget())
            return;

        if (_attackSpeed != TownManager.currentTownStats.soldierAttackSpeed && TownManager.currentTownStats.soldierAttackSpeed != 0)
        {
            _attackSpeed = TownManager.currentTownStats.soldierAttackSpeed;
            _attackTimer.WaitTime = 1f / _attackSpeed;
        }
        
        if(_accuracy != TownManager.currentTownStats.soldierAccuracy && TownManager.currentTownStats.soldierAccuracy != 0)
        {
            _accuracy = TownManager.currentTownStats.soldierAccuracy;
            _speed = _accuracy * _speed;
            _attackRange = _speed * (_projectile.airtime - _projectile.despawnTime);
        }

        ProjectileController projectile = (ProjectileController)_projectilePrefab.Instantiate();
        AddChild(projectile);

        projectile.attack = _attack;
        projectile.speed = _speed;
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

        if (_closestEnemy == null || _projectileStartPosition.GlobalPosition.DistanceTo(_closestEnemy.GlobalPosition) > _attackRange) 
        {
            return false;
        }
        else
        {
            _attack.direction = _projectileStartPosition.GlobalPosition.DirectionTo(_closestEnemy.GlobalPosition);
            return true;
        }

        
        // For testing purposes..
        Node2D _player = GetNode("/root/Town/Objects/Player") as Node2D;
        _attack.direction = _projectileStartPosition.GlobalPosition.DirectionTo(_player.GlobalPosition);

        return true;
    }
}
