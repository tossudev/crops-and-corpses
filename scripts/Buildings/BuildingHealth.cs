using Godot;
using System;
using System.Diagnostics;
using System.Security;

public partial class BuildingHealth : Node2D
{
	HealthComponent _healthComponent;
    CollisionShape2D _collisionShape;
    const string HEALTH_COMPONENT_NODENAME = "%HealthComponent";
    const string COLLISIONSHAPE2D_NODENAME = "%StaticCollisionShape2D";
    Node2D _parent;

    public bool isBroken;

    public int buildingHealth;
    
	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _healthComponent = GetNode<HealthComponent>(HEALTH_COMPONENT_NODENAME);
        _collisionShape = GetNode<CollisionShape2D>(COLLISIONSHAPE2D_NODENAME);
        _parent = GetParent() as Node2D;

        buildingHealth = _healthComponent.GetMaxHealth();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void OnHealth(float health)
    {
        buildingHealth = Mathf.FloorToInt(health);

        if (health <= 0)
        {
            BreakBuilding();
        }
    }

    private void BreakBuilding()
    {
        isBroken = true;
        _collisionShape.Disabled = true;
        _parent.Modulate = new Color(1, 1, 1, 0.3f);

        if (_parent == null || !_parent.HasMethod("OnBreak"))
        {
            return;
        }

        _parent.CallDeferred("OnBreak");
    }

    public void FixBuilding()
    {
        isBroken = false;
        _collisionShape.Disabled = false;
        _parent.Modulate = new Color(1, 1, 1, 1);

        if (_parent == null || !_parent.HasMethod("OnFixed"))
        {
            return;
        }

        _parent.CallDeferred("OnFixed");
    }
}
