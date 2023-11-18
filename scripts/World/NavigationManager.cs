using Godot;
using Godot.NativeInterop;
using System;
using System.Linq;

public partial class NavigationManager : Node2D {

	string _obstacleGroupName = "obstacle_area";
	NavigationRegion2D _regionNode;
	NavigationPolygon _regionPolygon;

	Object[] _obstacleAreas = Array.Empty<Object>();


    public override void _Ready() {
        _regionNode = GetNode<NavigationRegion2D>("Region");
		_regionPolygon = _regionNode.NavigationPolygon;

		InitRegion();
    }


	void InitRegion() {
		GD.Print("initialized!");

		foreach (Polygon2D _obstacleArea in GetTree().GetNodesInGroup(_obstacleGroupName)) {
			Vector2[] _polygonPoints = GetPolygonFromObject(_obstacleArea) as Vector2[];
			// GD.Print(_polygonPoints);
			_obstacleAreas.Append(_polygonPoints);
		}

		foreach (Vector2[] _area in _obstacleAreas) {
			Vector2[] _polygonToRemove = Array.Empty<Vector2>();

			foreach (Vector2 _pos in _area) {
				_polygonToRemove.Append(_pos);
			}

			_regionPolygon.AddOutline(_polygonToRemove);
		}

		// GD.Print(_obstacleAreas);

		_regionPolygon.MakePolygonsFromOutlines();
	}


	static Vector2[] GetPolygonFromObject(Polygon2D _obstacleArea) {
		
		// For some fucking reason this returns an empty array so figure that out next
		Vector2[] _polygonPoints = _obstacleArea.Polygon;

		// _temporarySize.Append(new Vector2(-48, -40));
		// _temporarySize.Append(new Vector2(48, -40));
		// _temporarySize.Append(new Vector2(48, 32));
		// _temporarySize.Append(new Vector2(-48, 32));

		// Adjust areas local position to global
		int _posIndex = 0;
		foreach (var pos in _polygonPoints) {
			_polygonPoints[_posIndex] += _obstacleArea.GlobalPosition;
			_posIndex ++;
		}

		return _polygonPoints;
	}
}
