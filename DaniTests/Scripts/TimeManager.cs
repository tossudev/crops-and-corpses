using Godot;
using System;

public partial class TimeManager : Node
{

    private int dayCounter;
    private float currentTime;
    public float timeSpeed = 1f;
    [Export] private Color nightTimeColor = new Color((float)0.5,(float) 0.5, (float)0.5);  // Set your desired nighttime color
    [Export] private Color dayTimeColor = new Color(1, 1, 1);    // Set your desired daytime color
    [Export] private float transitionDuration = 3f; // Set the duration of the transition
    [Export] private float dayTimeLength = 60f;    // 10 min duration for day
    [Export] private float nightTimeLength = 30f;  // 10 min duration for night
    private GlobalTime globalTime;

    private bool isDayTime = true;
    public bool dayTime { get { return isDayTime; } set { dayTime= value; } }

    private CanvasModulate sunlight;

    public override void _Ready()
    {
        
        globalTime = GetNode<GlobalTime>("/root/GlobalTime");
        sunlight = GetNode<CanvasModulate>("Sunlight");

        if (sunlight != null)
        {
            sunlight.Color = dayTimeColor;
        }
        else
        {
            GD.Print("Sunlight not found in the scene.");
        }

        dayCounter = globalTime.GetDay();
        currentTime = globalTime.GetTime();
        sunlight.Color = globalTime.GetColor();
    }

    public override void _Process(double delta)
    {
        
        currentTime += (float)delta * timeSpeed;
        globalTime.SetTime(currentTime);
        float timeOfDay = currentTime % (dayTimeLength + nightTimeLength);

        // Determine if it's daytime
        bool isNowDayTime = timeOfDay <= dayTimeLength;
        if(isNowDayTime != isDayTime)
        {
            // Day has changed, but only increment dayCounter when transitioning from night to day
            if (!isDayTime && isNowDayTime)
            {
                dayCounter++;
                globalTime.SetDay(dayCounter);
                GD.Print(dayCounter);
            }

            isDayTime = isNowDayTime;
        }
        
        if (timeOfDay <= dayTimeLength)
        {
            
            if (sunlight.Color != dayTimeColor)
            {
                // Transition back to daytime
                sunlight.Color = LerpColor(sunlight.Color, dayTimeColor,(float) delta / transitionDuration);
            }
        }
        else
        {
            if (sunlight.Color != nightTimeColor)
            {
                // Transition to nighttime
                sunlight.Color = LerpColor(sunlight.Color, nightTimeColor,(float) delta / transitionDuration);
            }
            
        }
        globalTime.SetColor(sunlight.Color);
        isDayTime = timeOfDay <= dayTimeLength + 10f; // 1s for delaying zombievawes

    }

    // Custom function to interpolate between two colors
    private Color LerpColor(Color from, Color to, float t)
    {
        return new Color(
            Mathf.Lerp(from.R, to.R, t),
            Mathf.Lerp(from.G, to.G, t),
            Mathf.Lerp(from.B, to.B, t)
        );
    }
}