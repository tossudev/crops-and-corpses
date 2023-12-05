using Godot;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class ArcherTower : Node2D
{
    [Export]
    Node2D _animationsScene;
    ArcherAnimation _animator;
    const string ANIMATION_NODENAME = "%Animator";
    Node2D _archerNode2D;

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

    int _attackSpeedMultiplier;
    float _attackSpeed;
    int _accuracy;
    public bool isOccupied;

    public bool isBroken;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _animator = _animationsScene.GetNode("Skeleton2D") as ArcherAnimation;
        _archerNode2D = GetNode<Node2D>(ANIMATION_NODENAME);

        _attackSpeed = 0.5f;
        _speed = 16;
        _targetGroup = "enemy";
        _attackRange = _speed * (_projectile.airtime - _projectile.despawnTime);

        isBroken = false;

        _attack = new Attack
        {
            damage = 25,
            knockback = 200,
            effect = 0
        };

        if (TownManager.currentTownStats.soldierAttackSpeed != 0)
        {
            _attackSpeedMultiplier = TownManager.currentTownStats.soldierAttackSpeed;
            _attackTimer.WaitTime = 1f / (_attackSpeed * _attackSpeedMultiplier);
        }

        if (TownManager.currentTownStats.soldierAccuracy != 0)
        {
            _accuracy = TownManager.currentTownStats.soldierAccuracy;
            _speed = _accuracy * _speed;
            _attackRange = _speed * (_projectile.airtime - _projectile.despawnTime);
        }
    }

    private void OnBreak()
    {
        isBroken = true;
    }

    private void OnFixed()
    {
        isBroken = false;
    }

    public void ActivateTower()
    {
        _archerNode2D.Visible = true;
        _attackTimer.Start();
        isOccupied = true;
    }

    public void DeactivateTower()
    {
        _archerNode2D.Visible = false;
        _attackTimer.Stop();
        isOccupied = false;
    }

    async void OnShootTimerTimeout()
	{
        if (!EnemiesInRange() || isBroken)
        {
            _animator.StopAnimations();
            return;
        }


        _animator.ShootAnimation(1f / (_attackSpeed * _attackSpeedMultiplier));

        int delayTime;
        delayTime = Mathf.RoundToInt(_animator.RealAnimationLength() * 0.75f * 1000);

        await Task.Delay(delayTime);

        if (isBroken)
            return;

        FindTarget();

        if (_attackSpeedMultiplier != TownManager.currentTownStats.soldierAttackSpeed && TownManager.currentTownStats.soldierAttackSpeed != 0)
        {
            _attackSpeedMultiplier = TownManager.currentTownStats.soldierAttackSpeed;
            _attackTimer.WaitTime = 1f / (_attackSpeed * _attackSpeedMultiplier);
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

    private bool EnemiesInRange()
    {
        foreach (Node2D enemy in GetTree().GetNodesInGroup("enemy"))
        {
            Node2D _parent = enemy.GetParent() as Node2D;

            if (_projectileStartPosition.GlobalPosition.DistanceTo(_parent.GlobalPosition) <= _attackRange)
            {
                return true;
            }
        }
        return false;
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
