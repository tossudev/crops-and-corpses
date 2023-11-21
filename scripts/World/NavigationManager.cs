using Godot;
using Godot.NativeInterop;
using System;
using System.Linq;

public partial class NavigationManager : Node2D {

	string _obstacleGroupName = "obstacle_area";
	NavigationRegion2D _regionNode;
	NavigationPolygon _regionPolygon;

	Godot.Collections.Array<Vector2[]> _obstacleAreas = new ();


    public override void _Ready() {
        _regionNode = GetNode<NavigationRegion2D>("Region");
		_regionPolygon = _regionNode.NavigationPolygon;

		InitRegion();
    }


	void InitRegion() {
		// Get all obstacles
		foreach (Polygon2D _obstacleArea in GetTree().GetNodesInGroup(_obstacleGroupName)) {
			Vector2[] polygonPoints = GetPolygonFromObject(_obstacleArea);
			_obstacleAreas.Add(polygonPoints);
			_regionPolygon.AddOutline(polygonPoints);
		}

		// Update navigation region
		_regionPolygon.MakePolygonsFromOutlines();
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


	public void RemoveArea(int nodeIndex) {
		GD.Print("Remove!");
	}
}
