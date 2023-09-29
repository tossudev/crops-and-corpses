using Godot;
using System;
using System.Diagnostics;

public partial class TimeManager : Node
{
    private float currentTime = 0f;
    public float timeSpeed = 1f;
    private float nightTimeEnergy = 0f;  // Set your desired nighttime energy value
    private float dayTimeEnergy = 1f; // Set your desired daytime energy value
    private float transitionDuration = 1f; // Set the duration of the transition
    private float dayTimeLenght = 60f; // 10 min duration for day
    private float nightTimeLeght = 30f; // 5 min duration for night
    private bool isDayTime = true;

    private DirectionalLight2D sunlight;

    public override void _Ready()
    {
        sunlight = GetNode<DirectionalLight2D>("Sunlight");
        
        if (sunlight != null)
        {
            sunlight.Energy = dayTimeEnergy;
        }
        else
        {
            GD.Print("Sunlight not found in the scene.");
        }
        
        currentTime = 0f;
    }
    public override void _Process(double delta)
    {
        currentTime += (float)delta * timeSpeed;
        float timeOfDay = currentTime % (dayTimeLenght + nightTimeLeght);

        if(timeOfDay <= dayTimeLenght)
        {
            if(sunlight.Energy < 0.99)
            {
                // Transition back to daytime
                sunlight.Energy = Mathf.Lerp(sunlight.Energy,dayTimeEnergy, (float) delta / transitionDuration);
                //GD.Print(sunlight.Energy + " Day time");
            }
            else
            {
                isDayTime = true;
            }
        }
        else 
        {
            if(sunlight.Energy > 0.01)
            {
                // Transition to nighttime
                sunlight.Energy = Mathf.Lerp(sunlight.Energy, nightTimeEnergy, (float) delta / transitionDuration);
               // GD.Print(sunlight.Energy + " Night time");
            }
            else
            {
                isDayTime = false;
            }
        }
    }
    public bool returnTimeOfDay(bool time)
    {
        time = isDayTime;
        return time;
    }
}
