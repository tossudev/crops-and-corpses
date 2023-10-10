using Godot;
using System;

public partial class Harvestable : Node2D
{
    [Export]
    private HealthComponent healthComponent;

    [Export]
    private HitboxComponent hitboxComponent;

    [Export]
    private PackedScene dropItem;

    private Random random;

    public enum HealthLevel
    {
        Full,
        ThreeQuarters,
        Half,
        Quarter
    }

    public override void _Ready()
    {
        healthComponent = GetNode<HealthComponent>("HealthComponent");
        hitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
        random = new Random();
    }

    public override void _PhysicsProcess(double delta)
    {
        DropItems();
    }

    private HealthLevel GetHealthLevel()
    {
        float healthRatio = (float)healthComponent.GetHealth() / healthComponent.GetMaxHealth();
        const float epsilon = 0.001f;

        if (Mathf.Abs(healthRatio - 1.0f) < epsilon) return HealthLevel.Full;
        if (healthRatio >= 0.75f) return HealthLevel.ThreeQuarters;
        if (healthRatio >= 0.5f) return HealthLevel.Half;
        if (healthRatio >= 0.25f) return HealthLevel.Quarter;

        return HealthLevel.Quarter;
    }

   private void DropItems()
{
    int dropAmount = 0;

    if (healthComponent.GetHealth() == healthComponent.GetMaxHealth())
        dropAmount = 1;
    else if (healthComponent.GetHealth() >= healthComponent.GetMaxHealth() * 3 / 4)
        dropAmount = random.Next(1, 3);
    else if (healthComponent.GetHealth() >= healthComponent.GetMaxHealth() / 2)
        dropAmount = random.Next(2, 4);
    else if (healthComponent.GetHealth() >= healthComponent.GetMaxHealth() / 4)
        dropAmount = random.Next(3, 5);

    for (int i = 0; i < dropAmount; i++)
    {
        Node2D itemInstance = dropItem.Instantiate() as Node2D; // Change 'Node2D' to the actual base class of your 'Item' class

        if (itemInstance != null)
        {
            float radius = 50.0f;
            Vector2 randomOffset = new Vector2(random.Next(-1, 2), random.Next(-1, 2)) * radius;
            itemInstance.Position = Position + randomOffset;
            GetParent().AddChild(itemInstance);
        }
    }
}
}



