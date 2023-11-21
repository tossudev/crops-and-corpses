using Godot;
using System;

public partial class NavigationObstacle : Polygon2D
{
	public NavigationManager navManager;
	public int nodeIndex = 0;


	public override void _Ready() {
		// remove this cursed reference by the holy power of ra
		// for some reason the unique name accessor didn't work for this node (TODO)
		navManager = GetNode<Node2D>("../../../../NavigationManager") as NavigationManager;
	}

    public override void _ExitTree() {
        base._ExitTree();
		navManager.RemoveArea(nodeIndex);
    }
}
