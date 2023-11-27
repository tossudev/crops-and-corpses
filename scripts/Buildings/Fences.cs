using Godot;
using System;
using System.Diagnostics;

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

    PackedScene _fenceDoorHorizontal, _fenceDoorEast, _fenceDoorWest;
	public override void _Ready()
	{
        _fences = GetNode("Fences") as Node2D;

		_fenceNorth = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_north.tscn");
        _fenceSouth = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_south.tscn");
        _fenceWest = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_west.tscn");
        _fenceEast = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_east.tscn");

        _fenceDoorHorizontal = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_door_horizontal.tscn");
        _fenceDoorEast = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_door_east.tscn");
        _fenceDoorWest = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_door_west.tscn");

		InstantiateFences();
    }

    public void InstantiateFences()
	{
        foreach (Node2D node in _fences.GetChildren())
        {
            node.QueueFree();
        }

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
                InstantiateDoor(x, y, _fenceDoorHorizontal);
                continue;
            }
            else if (Math.Floor(fenceLengthX / 2f) == i && _southDoorSpot == -1)
            {
                //InstantiateDoor(x + 128, y + 90, _fenceEast, x, y, _fenceNorth);
                InstantiateDoor(x, y, _fenceDoorHorizontal);
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
    }

    private void InstantiateDoor(float posX, float posY, PackedScene door)
    {
        Node2D _fenceDoorScene = door.Instantiate() as Node2D;
        _fenceDoorScene.Position = new Vector2(posX,posY);
        _fences.AddChild(_fenceDoorScene);

        if(_fenceDoorScene.HasMethod("DoorsTopdown"))
        {
            _fenceDoorScene.CallDeferred("Doors");
        }
    }
}
