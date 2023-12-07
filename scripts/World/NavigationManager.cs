using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Timer = Godot.Timer;

public partial class NavigationManager : Node2D {

	public const string _obstacleGroupName = "obstacle_area";
	
	static NavigationRegion2D _regionNode;
	static NavigationPolygon _region;

	static readonly Array<NavigationObstacle> _obstacleArray = new ();

	public static bool _initialized;

	public override void _Ready()
	{
		_regionNode = GetNode<NavigationRegion2D>("Region");
		_region = _regionNode.NavigationPolygon;

		InitRegion(this);
	}


	void InitRegion(Node caller)
	{
		foreach (var node in caller.GetTree().GetNodesInGroup(_obstacleGroupName))
		{
			var navigationObstacle = (NavigationObstacle) node;

			AddNavigationObstacleToMap(navigationObstacle);
		}

		UpdateObstacleIndexes();

		
		
		Timer updateTimer = new Timer()
		{
			Autostart = true,
			OneShot = false,
			WaitTime = 10f
		};

		updateTimer.Timeout += BakeMap;
		AddChild(updateTimer);
		
		// Update navigation region
		BakeMap();
		_initialized = true;
    }
    


	public void AddArea(NavigationObstacle obstacle)
	{

		if (!_initialized)
		{
			GD.PushError("Unable to initialize navigationManager");
			return;
		}
		
		if (obstacle is null)
		{
			GD.PushWarning("Tried to add null obstacle");
			return;
		}
		
		if (_obstacleArray.Any(existingObstacle => existingObstacle.nodeIndex == obstacle.nodeIndex)) return;


		AddNavigationObstacleToMap(obstacle);
		UpdateObstacleIndexes();
	}

	static bool _bakeInProgress;
	static async void BakeMap()
	{
		await TaskExtensions.SuspendWhile(() => _bakeInProgress);

		_bakeInProgress = true;
		await Task.Run(() =>
		{
			_region.MakePolygonsFromOutlines();
		});
		_bakeInProgress = false;
	}
	
	public void RemoveArea(NavigationObstacle obstacle)
	{
	
		if (!_initialized)
		{
			GD.PushError("Unable to initialize navigationManager");
			return;
		}
		
		if (obstacle is null)
		{
			GD.PushWarning("Tried to remove null obstacle");
			return;
		}

		if (_obstacleArray.Any(existing => obstacle.nodeIndex == existing.nodeIndex))
		{
			RemoveNavigationObstacleFromMap(obstacle);
			UpdateObstacleIndexes();
        }
	}
	
	static void AddNavigationObstacleToMap(NavigationObstacle obstacle)
	{
		_obstacleArray.Add(obstacle);

		var polygonPoints = GetPolygonFromObject(obstacle);
	    
		_region.AddOutline(polygonPoints);
	}
    
	static void RemoveNavigationObstacleFromMap(NavigationObstacle obstacle)
	{
		_region.RemoveOutline(obstacle.nodeIndex);
		_obstacleArray.Remove(obstacle);
	}
    

	static Vector2[] GetPolygonFromObject(Polygon2D _obstacleNode) 
	{
		Vector2[] polygonPoints = (Vector2[]) _obstacleNode.Get("polygon");

		// Adjust areas local position to global
		int _posIndex = 0;
		foreach (Vector2 pos in polygonPoints)
		{
			polygonPoints[_posIndex] += _obstacleNode.GlobalPosition;
			_posIndex ++;
		}

		return polygonPoints;
	}

	static void UpdateObstacleIndexes()
	{
		if (_obstacleArray.Count == 0) return;
		
		for (int i = 1; i < _region.Outlines.Count; i++)
		{
			var obstacle = _obstacleArray[i - 1];
			
			if (obstacle != null)
			{
				obstacle.nodeIndex = i;
			}
		}
	}
    
}
