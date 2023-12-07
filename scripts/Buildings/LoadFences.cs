using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Nodes;

public partial class LoadFences : Node2D
{
    Node2D _fences;

    Area2D _inputArea;

    PackedScene _fenceNorth, _fenceSouth, _fenceWest, _fenceEast;

    PackedScene _fenceDoorSouth, _fenceDoorEast, _fenceDoorWest;

    string _savePath, _fileName;

    List<int> _fencesList = new List<int>();

    int _fenceIndex = 0;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _savePath = ProjectSettings.GlobalizePath("user://saves/");
        _fileName = "buildings.txt";

        _fences = GetNode("Fences") as Node2D;

        LoadBuildings(_savePath, _fileName);

        LoadFenceHealths();
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

    public async void LoadFenceHealths()
    {
        await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete);

        foreach (Node2D fence in _fences.GetChildren())
        {
            if (_fenceIndex < _fencesList.Count)
            {
                BuildingHealth healthscript = fence.GetNode("%BuildingHealth") as BuildingHealth;
                healthscript.loadedHealth = _fencesList[_fenceIndex];
                healthscript.isLoaded = true;
                healthscript.LoadBuildingHealth(_fencesList[_fenceIndex]);

                HealthComponent healtComponent = fence.GetNode<HealthComponent>("%HealthComponent");
                healtComponent.SetHealth(_fencesList[_fenceIndex]);

                _fenceIndex++;
            }
            else
            {
                HealthComponent healtComponent = fence.GetNode<HealthComponent>("%HealthComponent");
                healtComponent.SetHealth(100 + TownManager.currentTownStats.wallHP);
            }
        }
    }
}