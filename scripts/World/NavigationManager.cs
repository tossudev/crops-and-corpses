using Godot;
using Godot.NativeInterop;
using System;
using System.Linq;

public partial class NavigationManager : Node2D {

	string _obstacleGroupName = "obstacle_area";
	NavigationRegion2D _regionNode;
	NavigationPolygon _regionPolygon;

	Godot.Collections.Array<Vector2[]> _obstacleAreas;
	[Export] public Vector2[] arrayTest;


    public override void _Ready() {
        _regionNode = GetNode<NavigationRegion2D>("Region");
		_regionPolygon = _regionNode.NavigationPolygon;

		InitRegion();
    }


	void InitRegion() {

		// Get all obstacles
		foreach (Polygon2D _obstacleArea in GetTree().GetNodesInGroup(_obstacleGroupName)) {
			Vector2[] _polygonPoints = GetPolygonFromObject(_obstacleArea);
			// GD.Print(_polygonPoints[0]);
			_obstacleAreas.Append(_polygonPoints);
		}

		// Carve out every obstacle from the navigation region
		foreach (Vector2[] _area in _obstacleAreas) {
			Vector2[] _polygonToRemove = Array.Empty<Vector2>();

			foreach (Vector2 _pos in _area) {
				_polygonToRemove.Append(_pos);
			}

			_regionPolygon.AddOutline(_polygonToRemove);
		}

		GD.Print(_obstacleAreas);

		// Update navigation region
		_regionPolygon.MakePolygonsFromOutlines();
	}


	static Vector2[] GetPolygonFromObject(Polygon2D _obstacleArea) {
		// TODO:
		// Figure out why the hell a Vector2[] is always empty when trying to fetch it

		// var test = _obstacleArea.Get("polygon");
		// Vector2[] _polygonPoints = _obstacleArea.Get("polygon").Obj as Vector2[];

		Vector2[] _polygonPoints = (Vector2[]) _obstacleArea.Get("polygon");

		GD.Print(_polygonPoints[0]);	// Prints "(616, -112)"
		GD.Print(_polygonPoints);		// Prints "Godot.Vector2[]"

		// Adjust areas local position to global
		int _posIndex = 0;
		foreach (Vector2 pos in _polygonPoints) {
			_polygonPoints[_posIndex] += _obstacleArea.GlobalPosition;
			_posIndex ++;
		}

		return _polygonPoints;
	}
}
