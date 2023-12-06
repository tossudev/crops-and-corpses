using Godot;
using System;
using System.Diagnostics;
using System.Security;
public enum BuildingType{
    House,
    Fence,
    ArcherTower
}
public partial class BuildingHealth : Node2D
{
	HealthComponent _healthComponent;
    const string HEALTH_COMPONENT_NODENAME = "%HealthComponent";
    
    VillagerResidence _villagerResidenceComponent;
    const string RESIDENCE_COMPONENT_NODENAME = "%VillagerResidenceComponent";

    
    CollisionShape2D _collisionShape;
    const string COLLISIONSHAPE2D_NODENAME = "%StaticCollisionShape2D";
    Node2D _parent;

    public bool isBroken;
    public bool isDamaged;
    public bool isLoaded = false;

    public int buildingHealth;
    public int loadedHealth = 0;
     
    public BuildingType buildingType;
	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        if (!SceneManager.IsCurrentScene(this, Scene.Town))
        {
            QueueFree();
            return;
        }
        
        _healthComponent = GetNode<HealthComponent>(HEALTH_COMPONENT_NODENAME);
        _healthComponent.AssignBuilding(this);

        _villagerResidenceComponent = GetNodeOrNull<VillagerResidence>(RESIDENCE_COMPONENT_NODENAME);
        
        _collisionShape = GetNode<CollisionShape2D>(COLLISIONSHAPE2D_NODENAME);
        _parent = GetParent() as Node2D;

        if(_parent.IsInGroup("fence"))
        {
            buildingType = BuildingType.Fence;
        }
        if(_parent.IsInGroup("ArcherTower"))
        {
            buildingType = BuildingType.ArcherTower;
        }
        if(_parent.IsInGroup("House"))
        {
            buildingType = BuildingType.House;
        }

        buildingHealth = _healthComponent.GetMaxHealth();
        RegisterBuilding();
	}

    async void RegisterBuilding()
    {
        await TaskExtensions.SuspendWhile(() =>
        VillagerManager.villagerManagerInstance == null || !SaveData.firstLoadComplete,
            GD.Randi() % 2000 + 100);
        
        VillagerManager.villagerManagerInstance.AddNewBuilding(this);
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        if (VillagerManager.villagerManagerInstance?.allBuildings.Contains(this) ?? false)
        {
            VillagerManager.villagerManagerInstance.RemoveBuilding(this);
        }
    }
    
    private void OnHealth(float health)
    {
        buildingHealth = Mathf.FloorToInt(health);

        if (health <= 0)
        {
            BreakBuilding();
            return;
        }
        
        if (isBroken && buildingHealth > 0)
        {
            FixBuilding();
        }

        if (isDamaged && buildingHealth == _healthComponent.GetMaxHealth())
        {
            isDamaged = false;
        }
    }

    public void LoadBuildingHealth(int loadedHealth)
    {
        buildingHealth = loadedHealth;      

        _healthComponent.SetHealth(loadedHealth);

        if (buildingHealth <= 0)
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

        _villagerResidenceComponent?.OnBreak();
        _parent.CallDeferred("OnBreak");
    }
    
    void FixBuilding()
    {
        isBroken = false;
        _collisionShape.Disabled = false;
        _parent.Modulate = new Color(1, 1, 1, 1);

        if (_parent == null || !_parent.HasMethod("OnFixed"))
        {
            return;
        }

        _villagerResidenceComponent?.OnFixed();
        _parent.CallDeferred("OnFixed");
    }

    public void FixBrokenBuilding()
    {
        _healthComponent.SetHealth(_healthComponent.GetMaxHealth());
    }
}
