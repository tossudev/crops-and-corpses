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

    PackedScene _fenceNorth, _fenceSouth, _fenceWest, _fenceEast;
	public override void _Ready()
	{
        _fences = GetNode("Fences") as Node2D;

		_fenceNorth = ResourceLoader.Load<PackedScene>("res://NikkeTests/scenes/fence_scenes/fence_north.tscn");
        _fenceSouth = ResourceLoader.Load<PackedScene>("res://NikkeTests/scenes/fence_scenes/fence_south.tscn");
        _fenceWest = ResourceLoader.Load<PackedScene>("res://NikkeTests/scenes/fence_scenes/fence_west.tscn");
        _fenceEast = ResourceLoader.Load<PackedScene>("res://NikkeTests/scenes/fence_scenes/fence_east.tscn");

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
            int x = -(fenceLengthX / 2) * 128 + 128;
            int y = (fenceLengthY / 2) * 128 - i * 128;

            if(fenceLengthY % 2 == 1)
            {
                x -= 128;

                if (Math.Floor(fenceLengthY / 2f) == i)
                {
                    Node2D _fenceDoorScene = _fenceNorth.Instantiate() as Node2D;
                    _fenceDoorScene.Position = new Vector2(x - 128, y + 40);
                    _fences.AddChild(_fenceDoorScene);

                    continue;
                }
            }
            else
            {
                if (Math.Floor(fenceLengthY / 2f) == i + 1)
                {
                    Node2D _fenceDoorScene = _fenceNorth.Instantiate() as Node2D;
                    _fenceDoorScene.Position = new Vector2(x - 128, y + 40);
                    _fences.AddChild(_fenceDoorScene);

                    continue;
                }
            }

            Node2D _fenceScene = _fenceEast.Instantiate() as Node2D;
            _fenceScene.Position = new Vector2(x, y);
            _fences.AddChild(_fenceScene);
        }

        for (int i = 0; i < fenceLengthY; i++)
        {
            int x = (fenceLengthX / 2) * 128;
            int y = (fenceLengthY / 2) * 128 - i * 128;

            if (Math.Floor(fenceLengthY / 2f) == i)
            {
                Node2D _fenceDoorScene = _fenceNorth.Instantiate() as Node2D;
                _fenceDoorScene.Position = new Vector2(x + 128, y + 40);
                _fences.AddChild(_fenceDoorScene);

                continue;
            }

            Node2D _fenceScene = _fenceWest.Instantiate() as Node2D;
            _fenceScene.Position = new Vector2(x, y);
            _fences.AddChild(_fenceScene);
        }

        for (int i = 0; i < fenceLengthX; i++)
        {
            int x = (fenceLengthX / 2) * 128 - i * 128;
            int y = -(fenceLengthY / 2) * 128;

            if (fenceLengthX % 2 == 1)
            {
                y -= 128;

                if (Math.Floor(fenceLengthX / 2f) == i)
                {
                    Node2D _fenceDoorScene = _fenceWest.Instantiate() as Node2D;
                    _fenceDoorScene.Position = new Vector2(x, y);
                    _fences.AddChild(_fenceDoorScene);

                    continue;
                }
            }
            else
            {
                if (Math.Floor(fenceLengthX / 2f) == i + 1)
                {
                    Node2D _fenceDoorScene = _fenceWest.Instantiate() as Node2D;
                    _fenceDoorScene.Position = new Vector2(x, y);
                    _fences.AddChild(_fenceDoorScene);

                    continue;
                }
            }

            Node2D _fenceScene = _fenceNorth.Instantiate() as Node2D;
            _fenceScene.Position = new Vector2(x, y);
            _fences.AddChild(_fenceScene);
        }

        for (int i = 0; i < fenceLengthX; i++)
        {
            int x = (fenceLengthX / 2) * 128 - i * 128;
            int y = (fenceLengthY / 2) * 128;

            if (Math.Floor(fenceLengthX / 2f) == i)
            {
                Node2D _fenceDoorScene = _fenceEast.Instantiate() as Node2D;
                _fenceDoorScene.Position = new Vector2(x + 128, y + 90);
                _fences.AddChild(_fenceDoorScene);

                continue;
            }

            Node2D _fenceScene = _fenceNorth.Instantiate() as Node2D;
            _fenceScene.Position = new Vector2(x, y);
            _fences.AddChild(_fenceScene);
        }


        if(centered)
        {
            _fences.Position = new Vector2(64, 64);
        }        
    }
}
