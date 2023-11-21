using Godot;
using Godot.NativeInterop;
using System;
using System.Linq;

public partial class NavigationManager : Node2D {

	string _obstacleGroupName = "obstacle_area";
	NavigationRegion2D _regionNode;
	NavigationPolygon _region;

	Godot.Collections.Array<Vector2[]> _obstacleAreas = new ();


    public override void _Ready() {
        _regionNode = GetNode<NavigationRegion2D>("Region");
		_region = _regionNode.NavigationPolygon;

		InitRegion();
    }


	void InitRegion() {
		// Get all obstacles
		foreach (Polygon2D _obstacleArea in GetTree().GetNodesInGroup(_obstacleGroupName)) {
			Vector2[] polygonPoints = GetPolygonFromObject(_obstacleArea);
			_obstacleAreas.Add(polygonPoints);
			_region.AddOutline(polygonPoints);
		}

		// Update navigation region
		_region.MakePolygonsFromOutlines();
		UpdateObstacleIndexes();
	}


	static Vector2[] GetPolygonFromObject(Polygon2D _obstacleArea) {
		Vector2[] _polygonPoints = (Vector2[]) _obstacleArea.Get("polygon");

		// Adjust areas local position to global
		int _posIndex = 0;
		foreach (Vector2 pos in _polygonPoints) {
			_polygonPoints[_posIndex] += _obstacleArea.GlobalPosition;
			_posIndex ++;
		}

		return _polygonPoints;
	}


	void UpdateObstacleIndexes() {
		int _index = 0;
		// foreach (NavigationObstacle _obstacle in GetTree().GetNodesInGroup(_obstacleGroupName)) {
		// 	_obstacle.nodeIndex = _index;
		// }
	}


	public void RemoveArea(int nodeIndex) {
		GD.Print("Remove!");

		// _region.RemoveOutline(nodeIndex);
		// _region.MakePolygonsFromOutlines();


	}
}
