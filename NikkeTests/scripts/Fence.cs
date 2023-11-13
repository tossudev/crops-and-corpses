using Godot;
using System;
using System.Diagnostics;

public partial class Fence : Node2D
{
    [Export]
    public int fenceLengthY = 9;
	[Export]
	public int fenceLengthX = 9;
    [Export]
    public bool centered = true;

    Node2D _fences;

    Area2D _inputArea;

    PackedScene _fenceNorth, _fenceSouth, _fenceWest, _fenceEast;
	public override void _Ready()
	{
        _fences = GetNode("Fences") as Node2D;

		_fenceNorth = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_north.tscn");
        _fenceSouth = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_south.tscn");
        _fenceWest = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_west.tscn");
        _fenceEast = ResourceLoader.Load<PackedScene>("res://scenes/buildings/fence_scenes/fence_east.tscn");

		InstantiateFences();
    }

    public void InstantiateFences()
	{
        foreach (Node2D node in _fences.GetChildren())
        {
            node.QueueFree();
        }

        for (int i = 0; i < fenceLengthY; i++)
        {
            int x = -(fenceLengthX / 2) * 128;
            int y = (fenceLengthY / 2) * 128 - i * 128;

            if(fenceLengthX % 2 == 0)
            {
                x += 128;
            }

            if (Math.Floor(fenceLengthY / 2f) == i)
            {
                InstantiateDoor(x - 128, y + 40, _fenceNorth, x, y, _fenceEast);
                continue;
            }

            InstantiateFence(x, y, _fenceEast);
        }

        for (int i = 0; i < fenceLengthY; i++)
        {
            int x = (fenceLengthX / 2) * 128;
            int y = (fenceLengthY / 2) * 128 - i * 128;

            if (Math.Floor(fenceLengthY / 2f) == i)
            {
                InstantiateDoor(x + 128, y + 40, _fenceNorth, x, y, _fenceWest);
                continue;
            }

            InstantiateFence(x, y, _fenceWest);
        }

        for (int i = 0; i < fenceLengthX; i++)
        {
            int x = (fenceLengthX / 2) * 128 - i * 128;
            int y = -(fenceLengthY / 2) * 128;

            if (fenceLengthY % 2 == 1)
            {
                y -= 128;
            }

            if (Math.Floor(fenceLengthX / 2f) == i)
            {
                InstantiateDoor(x, y, _fenceWest, x, y, _fenceNorth);
                continue;
            }

            InstantiateFence(x, y, _fenceNorth);
        }

        for (int i = 0; i < fenceLengthX; i++)
        {
            int x = (fenceLengthX / 2) * 128 - i * 128;
            int y = (fenceLengthY / 2) * 128;

            if (Math.Floor(fenceLengthX / 2f) == i)
            {
                InstantiateDoor(x + 128, y + 90, _fenceEast, x, y, _fenceNorth);
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

    private void InstantiateDoor(float posX, float posY, PackedScene door, float closedDoorX, float closedDoorY, PackedScene closedDoor)
    {
        Node2D _fenceDoorScene = door.Instantiate() as Node2D;
        _fenceDoorScene.Position = new Vector2(posX,posY);
        _fences.AddChild(_fenceDoorScene);
    }
}
