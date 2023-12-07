using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

public partial class Fences : Node2D
{
    [Export]
    public int fenceLengthY = 9;
	[Export]
	public int fenceLengthX = 9;
    [Export]
    public bool centered = true;

    [Export]
    private int _northDoorSpot = -1, _southDoorSpot = -1, _westDoorSpot = -1, _eastDoorSpot = -1;

    Node2D _fences;

    Area2D _inputArea;

    PackedScene _fenceNorth, _fenceSouth, _fenceWest, _fenceEast;

    PackedScene _fenceDoorSouth, _fenceDoorEast, _fenceDoorWest;

    string _savePath, _fileName;

    List<int> _fencesList = new List<int>();

    int _fenceIndex = 0;

    public override void _Ready()
    {
        _fenceNorth = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_north.tscn");
        _fenceSouth = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_south.tscn");
        _fenceWest = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_west.tscn");
        _fenceEast = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_east.tscn");

        _fenceDoorSouth = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_door_south.tscn");
        _fenceDoorEast = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_door_east.tscn");
        _fenceDoorWest = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_door_west.tscn");

        _savePath = ProjectSettings.GlobalizePath("user://saves/");
        _fileName = "buildings.txt";

        _fences = GetNode("Fences") as Node2D;

        if (SceneManager.IsCurrentScene(this, Scene.Town))
        {
            LoadBuildings(_savePath, _fileName);
        }

		InstantiateFences();
    }

    private void LoadBuildings(string path, string fileName)
    {
        path = Path.Join(path, fileName);

        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            JsonArray _loadedBuildings = (JsonArray)JsonArray.Parse(File.ReadAllText(path));
            GetFences(_loadedBuildings);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public void GetFences(JsonArray loadedBuildings)
    {
        foreach (JsonObject jsonObject in loadedBuildings)
        {
            if (jsonObject["name"] == null)
            {
                _fencesList.Add((int)jsonObject["health"]);
            }
        }
    }

    public async void InstantiateFences()
	{
        foreach (Node2D node in _fences.GetChildren())
        {
            node.QueueFree();
        }

        await Task.Delay((int)(GD.Randi() % 1000));

        // west fence
        for (int i = 0; i < fenceLengthY; i++)
        {
            int x = -(fenceLengthX / 2) * 128;
            int y = (fenceLengthY / 2) * 128 - i * 128;

            if(fenceLengthX % 2 == 0)
            {
                x += 128;
            }

            if (_westDoorSpot == i)
            {
                //InstantiateDoor(x - 128, y + 40, _fenceNorth, x, y, _fenceEast);
                InstantiateDoor(x, y, _fenceDoorWest);
                continue;
            }
            else if (Math.Floor(fenceLengthY / 2f) == i && _westDoorSpot == -1)
            {
                //InstantiateDoor(x - 128, y + 40, _fenceNorth, x, y, _fenceEast);
                InstantiateDoor(x, y, _fenceDoorWest);
                continue;
            }

            InstantiateFence(x, y, _fenceEast);
        }

        // east fence
        for (int i = 0; i < fenceLengthY; i++)
        {
            int x = (fenceLengthX / 2) * 128;
            int y = (fenceLengthY / 2) * 128 - i * 128;

            if (_eastDoorSpot == i)
            {
                //InstantiateDoor(x + 128, y + 40, _fenceNorth, x, y, _fenceWest);
                InstantiateDoor(x, y, _fenceDoorEast);
                continue;
            }
            else if (Math.Floor(fenceLengthY / 2f) == i && _eastDoorSpot == -1)
            {
                //InstantiateDoor(x + 128, y + 40, _fenceNorth, x, y, _fenceWest);
                InstantiateDoor(x, y, _fenceDoorEast);
                continue;
            }

            InstantiateFence(x, y, _fenceWest);
        }

        // north fence
        for (int i = 0; i < fenceLengthX; i++)
        {
            int x = (fenceLengthX / 2) * 128 - i * 128;
            int y = -(fenceLengthY / 2) * 128;

            if (fenceLengthY % 2 == 1)
            {
                y -= 128;
            }

            // North fence door commented out

            //if (_northDoorSpot == i)
            //{
            //    InstantiateDoor(x, y, _fenceWest);
            //    continue;
            //}
            //else if (Math.Floor(fenceLengthX / 2f) == i && _northDoorSpot == -1)
            //{
            //    InstantiateDoor(x, y, _fenceWest);
            //    continue;
            //}

            InstantiateFence(x, y, _fenceNorth);
        }


        // south fence
        for (int i = 0; i < fenceLengthX; i++)
        {
            int x = (fenceLengthX / 2) * 128 - i * 128;
            int y = (fenceLengthY / 2) * 128;

            if (_southDoorSpot == i)
            {
                //InstantiateDoor(x + 128, y + 90, _fenceEast, x, y, _fenceNorth);
                InstantiateDoor(x, y, _fenceDoorSouth);
                continue;
            }
            else if (Math.Floor(fenceLengthX / 2f) == i && _southDoorSpot == -1)
            {
                //InstantiateDoor(x + 128, y + 90, _fenceEast, x, y, _fenceNorth);
                InstantiateDoor(x, y, _fenceDoorSouth);
                continue;
            }

            InstantiateFence(x,y,_fenceNorth);
        }

        if (centered)
        {
            _fences.Position = new Vector2(64, 64);
        }        
    }

    public void InstantiateFence(float posX, float posY, PackedScene fenceScene)
    {
        Node2D _fenceScene = fenceScene.Instantiate() as Node2D;
        _fenceScene.Position = new Vector2(posX, posY);
        _fences.AddChild(_fenceScene);

        if (SceneManager.IsCurrentScene(this, Scene.Town) && _fenceIndex < _fencesList.Count)
        {
            BuildingHealth healthscript = _fenceScene.GetNode("BuildingHealth") as BuildingHealth;
            healthscript.loadedHealth = _fencesList[_fenceIndex];
            healthscript.isLoaded = true;
            healthscript.LoadBuildingHealth(_fencesList[_fenceIndex]);
            _fenceIndex++;
        }
        else if (SceneManager.IsCurrentScene(this, Scene.Town))
        {
            HealthComponent healtComponent = _fenceScene.GetNode<HealthComponent>("%HealthComponent");
            healtComponent.SetHealth(100 + SaveData.townHallStats.wallHP);
        }
    }

    private void InstantiateDoor(float posX, float posY, PackedScene door)
    {
        Node2D _fenceDoorScene = door.Instantiate() as Node2D;
        _fenceDoorScene.Position = new Vector2(posX,posY);
        _fences.AddChild(_fenceDoorScene);

        if (SceneManager.IsCurrentScene(this, Scene.Town) && _fenceIndex < _fencesList.Count)
        {
            BuildingHealth healthscript = _fenceDoorScene.GetNode("BuildingHealth") as BuildingHealth;
            healthscript.loadedHealth = _fencesList[_fenceIndex];
            healthscript.isLoaded = true;
            healthscript.LoadBuildingHealth(_fencesList[_fenceIndex]);
            _fenceIndex++;
        }
        else if (SceneManager.IsCurrentScene(this, Scene.Town))
        {
            HealthComponent healtComponent = _fenceDoorScene.GetNode<HealthComponent>("%HealthComponent");
            healtComponent.SetHealth(100 + SaveData.townHallStats.wallHP);          
        }


        if (_fenceDoorScene.HasMethod("DoorsOpen"))
        {
            _fenceDoorScene.CallDeferred("DoorsOpen");
        }
    }
}
