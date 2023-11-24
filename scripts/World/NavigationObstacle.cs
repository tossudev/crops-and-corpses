using Godot;
using System;

public partial class NavigationObstacle : Polygon2D
{
	public NavigationManager navManager;
	public int nodeIndex = 0;
	public Label debugText;


    public override void _EnterTree()
    {
        base._EnterTree();
		debugText = GetNode<Label>("Label");

		Node scene = GetTree().CurrentScene;
		navManager = scene.GetNode<Node2D>("%NavigationManager") as NavigationManager;
    }


    // If necessary, visualize all polygon indexes
    // public override void _Process(double delta)
    // {
    //     base._Process(delta);

	// 	if (debugText == null) {
	// 		return;
	// 	}
	// 	debugText.Text = nodeIndex.ToString();
    // }


    public override void _ExitTree() {
        base._ExitTree();
		navManager.RemoveArea(nodeIndex);
    }
}
