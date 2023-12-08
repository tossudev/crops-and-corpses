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
	static NavigationPolygon _navigationRegionPolygon;

	static readonly Array<NavigationObstacle> _obstacleArray = new ();

	public static bool _initialized;
	public static bool bakeInProgress;

	Timer _updateTimer;

	public override void _Ready()
	{
		InitRegion(this);
	}

	public override void _ExitTree()
	{
		_initialized = false;
		base._ExitTree();
	}

    void InitRegion(Node caller)
	{
		_regionNode = null;
		_navigationRegionPolygon = null;

		_obstacleArray.Clear();
		_initialized = false;
		bakeInProgress = false;
		
		_regionNode = GetNode<NavigationRegion2D>("Region");
		_navigationRegionPolygon = _regionNode.NavigationPolygon;

		if (_navigationRegionPolygon is null)
		{
			GD.PushError("Navigation Region Node not found @NavigationManager");
			return;
		}
		
		foreach (var node in caller.GetTree().GetNodesInGroup(_obstacleGroupName))
		{
			var navigationObstacle = (NavigationObstacle) node;

			AddNavigationObstacleToMap(navigationObstacle);
		}

		UpdateObstacleIndexes();

		_updateTimer?.QueueFree();

		_updateTimer = new Timer()
		{
			Autostart = true,
			OneShot = false,
			WaitTime = 10f
		};

		_updateTimer.Timeout += BakeMap;
		AddChild(_updateTimer);
		
		// Update navigation region
		BakeMap();
    }
    
	static async void BakeMap()
	{
		await TaskExtensions.SuspendWhile(() => bakeInProgress);
        
		if (SceneManager.sceneChanging) return;
		
		bakeInProgress = true;
		
		await Task.Run(() =>
		{
			_navigationRegionPolygon.MakePolygonsFromOutlines();

			bakeInProgress = false;
			_initialized = true;
		});
	}

	public async void AddArea(NavigationObstacle obstacle)
	{
		await TaskExtensions.SuspendWhile(() => !_initialized);

		if (!_initialized)
		{
			GD.PushError("Unable to initialize navigationManager");
			return;
		}
		
		if (obstacle == null || obstacle.IsQueuedForDeletion() || (obstacle.Owner?.IsQueuedForDeletion() ?? true))
		{
			GD.PushWarning("Tried to add null or disposed obstacle");
			return;
		}

		try
		{
			AddNavigationObstacleToMap(obstacle);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}

	
	
	public async void RemoveArea(NavigationObstacle obstacle)
	{
		await TaskExtensions.SuspendWhile(() => !_initialized);

		if (!_initialized)
		{
			GD.PushError("Unable to initialize navigationManager");
			return;
		}
		
		if (obstacle == null)
		{
			GD.PushWarning("Tried to remove null obstacle");
			return;
		}

		try
		{
			if (_obstacleArray.All(existing => obstacle.nodeIndex != existing.nodeIndex)) return;
			RemoveNavigationObstacleFromMap(obstacle);
			UpdateObstacleIndexes();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	}


	static void AddNavigationObstacleToMap(NavigationObstacle obstacle)
	{
		if (_obstacleArray.Any(existingObstacle =>
			    existingObstacle.GetInstanceId() == obstacle.GetInstanceId()
            || existingObstacle.GlobalPosition.DistanceTo(obstacle.GlobalPosition) < 10)) return;
		
		if (obstacle.IsQueuedForDeletion() || (obstacle.Owner?.IsQueuedForDeletion() ?? true))
		{
			GD.PushWarning("Tried adding a disposed object, not happening anytime soon");
			return;
		}
			
		_obstacleArray.Add(obstacle);

		var polygonPoints = GetPolygonFromObject(obstacle);
	    
		_navigationRegionPolygon.AddOutline(polygonPoints);
		
		if (_initialized)
		{
			UpdateObstacleIndexes();
		}
	}
    
	static void RemoveNavigationObstacleFromMap(NavigationObstacle obstacle)
	{
		int removeThisIndex = obstacle.nodeIndex;
		
		for (int i = 0; i < _obstacleArray.Count; i++)
		{
			var navigationObstacleAtIndex = _obstacleArray[i];

			if (navigationObstacleAtIndex is null) continue;
			if (navigationObstacleAtIndex.nodeIndex != removeThisIndex) continue;
			
			_obstacleArray.RemoveAt(i);
			break;
		}

		_navigationRegionPolygon.RemoveOutline(removeThisIndex);
	}
    

	static Vector2[] GetPolygonFromObject(Polygon2D _obstacleNode) 
	{
		Vector2[] polygonPoints = _obstacleNode.Polygon;

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
		
		for (int i = 0; i < _navigationRegionPolygon.Outlines.Count - 1; i++)
		{
			var obstacle = _obstacleArray[i];
			
			if (obstacle != null)
			{
				obstacle.nodeIndex = i + 1;
			}
		}
	}
    
}
