using Godot;
using System;

public partial class TimeManager : Node
{
    private float currentTime = 0f;
    public float timeSpeed = 1f;
    [Export] private Color nightTimeColor = new Color((float)0.5,(float) 0.5, (float)0.5);  // Set your desired nighttime color
    [Export] private Color dayTimeColor = new Color(1, 1, 1);    // Set your desired daytime color
    [Export] private float transitionDuration = 1f; // Set the duration of the transition
    [Export] private float dayTimeLength = 10f;    // 10 min duration for day
    [Export] private float nightTimeLength = 10f;  // 10 min duration for night

    private bool isDayTime = true;
    public bool dayTime { get { return isDayTime; } set { isDayTime = value; } }

    private CanvasModulate sunlight;

    public override void _Ready()
    {
        sunlight = GetNode<CanvasModulate>("Sunlight");

        if (sunlight != null)
        {
            sunlight.Color = dayTimeColor;
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
        float timeOfDay = currentTime % (dayTimeLength + nightTimeLength);

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

        isDayTime = timeOfDay <= dayTimeLength;
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