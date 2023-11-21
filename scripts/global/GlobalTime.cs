using Godot;
using System;
using System.Diagnostics;
using System.IO;
using Dictionary = Godot.Collections.Dictionary;

[GlobalClass]
public partial class GlobalTime : Node
{
    private float globalTime;
    private Color sunlight;
    private int day;
    private float timeSpeed = 1;
   // private int currenTowntDay;
   // private float globalhour;
    private string _savePath = ProjectSettings.GlobalizePath("user://saves/");
    private string _fileName = "Time.cfg";
    private bool hasTownBeenDestroyed;

    TimeManager timeManager;

    public override void _Ready()
    {
        hasTownBeenDestroyed = false;
     
        if (!Directory.Exists(_savePath))
        {
            Directory.CreateDirectory(_savePath);
        }
     
       // GD.Print(day);

        LoadData();
    }
    public override void _Process(double delta)
    {
        globalTime += (float)delta * timeSpeed;
    }


    public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			SaveData();
			GetTree().Quit(); // default behavior
		}
		
	}


  /*   public float GetHour()
    {
        return globalhour;
    }
    public void SetHour(float hour)
    {
        globalhour = hour;
    } */
    public float GetTime()
    {
        return globalTime;
    }

    public void SetTime(float time)
    {
        globalTime = time;
    }

    public Color GetColor()
    {
        return sunlight;
    }

    public void SetColor(Color color)
    {
        sunlight = color;
    }

    public int GetDay()
    {
        return day;
    }

    public void SetDay(int currentDay)
    {
        //Check if player is in town, when day changes. If not then town is destroyed and zombiespawning is stopped.
        Node root = GetTree().Root;
        Node2D townScene = root.GetNodeOrNull<Node2D>("Town");
        if(townScene == null)
        {
            hasTownBeenDestroyed = true;
        }
        else
        {
            hasTownBeenDestroyed = false;
        }
        day = currentDay;
    }
    public bool HasTownBeenDestroyed()
    {
        return hasTownBeenDestroyed;
    }

    public void LoadData()
    {
        string saveFile = Path.Join(_savePath + _fileName);
		

        if (File.Exists(saveFile))
        {
            ConfigFile config = new ConfigFile();
            var loaded = new Dictionary();

            config.Load(saveFile);
            loaded["global_time"] = config.GetValue("GlobalTime", "global_time");
            loaded["sunlight_r"] = config.GetValue("GlobalTime", "sunlight_r");
            loaded["sunlight_g"] = config.GetValue("GlobalTime", "sunlight_g");
            loaded["sunlight_b"] = config.GetValue("GlobalTime", "sunlight_b");
            loaded["day"] = config.GetValue("GlobalTime", "day");

            if (loaded.ContainsKey("global_time"))
            {
                globalTime = (float)loaded["global_time"];
				//GD.Print(globalTime);
            }

            if (loaded.ContainsKey("sunlight_r") && loaded.ContainsKey("sunlight_g") && loaded.ContainsKey("sunlight_b"))
            {
                sunlight = new Color((float)loaded["sunlight_r"], (float)loaded["sunlight_g"], (float)loaded["sunlight_b"]);
            }

            if (loaded.ContainsKey("day"))
            {
                day = (int)loaded["day"];
				//GD.Print(day);
            }
        }
    }

    public void SaveData()
    {
        ConfigFile config = new ConfigFile();
        config.SetValue("GlobalTime", "global_time", globalTime);
        config.SetValue("GlobalTime", "sunlight_r", sunlight.R);
        config.SetValue("GlobalTime", "sunlight_g", sunlight.G);
        config.SetValue("GlobalTime", "sunlight_b", sunlight.B);
        config.SetValue("GlobalTime", "day", day);

        var savePath = "user://saves/Time.cfg";
        config.Save(savePath);
    }
}