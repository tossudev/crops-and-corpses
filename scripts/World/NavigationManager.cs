using Godot;
using Godot.NativeInterop;
using System;
using System.Linq;

public partial class NavigationManager : Node2D {

	string _obstacleGroupName = "obstacle_area";
	NavigationRegion2D _regionNode;
	NavigationPolygon _region;

	Godot.Collections.Array<Vector2[]> _obstacleAreas = new ();
	NavigationObstacle _obstacle;


    public override void _Ready() {
        _regionNode = GetNode<NavigationRegion2D>("Region");
		_region = _regionNode.NavigationPolygon;

		InitRegion();
    }


	void InitRegion() {
		// Get all obstacles
		int _index = 0;
		foreach (Polygon2D _obstacleNode in GetTree().GetNodesInGroup(_obstacleGroupName)) {
			Vector2[] polygonPoints = GetPolygonFromObject(_obstacleNode);
			_obstacleAreas.Add(polygonPoints);
			_region.AddOutline(polygonPoints);

			_index ++;
		}

		UpdateObstacleIndexes();

		// Update navigation region
		_region.MakePolygonsFromOutlines();
	}


	static Vector2[] GetPolygonFromObject(Polygon2D _obstacleNode) {
		Vector2[] _polygonPoints = (Vector2[]) _obstacleNode.Get("polygon");

		// Adjust areas local position to global
		int _posIndex = 0;
		foreach (Vector2 pos in _polygonPoints) {
			_polygonPoints[_posIndex] += _obstacleNode.GlobalPosition;
			_posIndex ++;
		}

		return _polygonPoints;
	}


	void UpdateObstacleIndexes() {
		int _index = 0;
		foreach (Polygon2D _obstacleNode in GetTree().GetNodesInGroup(_obstacleGroupName)) {
			_obstacleNode.Set("nodeIndex", _index);
			// var test = _obstacleNode.Get("nodeIndex");
			// GD.Print(test);

			_index ++;
		}
	}


	public async void RemoveArea(int nodeIndex) {
		await ToSignal(GetTree().CreateTimer(0.1), "timeout");

		_region.RemoveOutline(nodeIndex + 1);
		_region.MakePolygonsFromOutlines();

		_obstacleAreas.RemoveAt(nodeIndex);


		UpdateObstacleIndexes();
	}
}
