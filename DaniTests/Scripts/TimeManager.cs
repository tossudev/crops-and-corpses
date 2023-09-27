using Godot;
using System;
using System.Diagnostics;

public partial class TimeManager : Node
{
    private float currentTime = 0f;
    public float timeSpeed = 1f;
    private float originalEnergy = 2f; // Set your desired initial energy value
    private float targetEnergy = 0f;  // Set your desired nighttime energy value
    private float transitionDuration = 2f; // Set the duration of the transition

    private DirectionalLight2D sunlight;

    public override void _Ready()
    {
        sunlight = GetNode<DirectionalLight2D>("Sunlight");
        
        if (sunlight != null)
        {
            sunlight.Energy = originalEnergy;
        }
        else
        {
            GD.Print("Sunlight not found in the scene.");
        }
        
        currentTime = 12.0f;
    }

    public override void _Process(double delta)
    {
        currentTime += (float)delta * timeSpeed;
        float timeOfDay = currentTime % 24;

        if (timeOfDay > 18.0f)
        {
            // Transition to nighttime
            sunlight.Energy = Mathf.Lerp(sunlight.Energy, targetEnergy,(float) delta / transitionDuration);
        }
        else if (timeOfDay > 6.0f)
        {
            // Transition back to daytime
            sunlight.Energy = Mathf.Lerp(sunlight.Energy, originalEnergy, (float)delta / transitionDuration);
        }
    }
}
