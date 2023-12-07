using Godot;
using System;

public partial class NavigationObstacle : Polygon2D
{
	public NavigationManager navManager;
	public int nodeIndex = 0;
	public Label debugText;


    public override async void _EnterTree()
    {
        base._EnterTree();
        
        await TaskExtensions.SuspendWhile(() => !NavigationManager._initialized);

		debugText = GetNode<Label>("Label");

		Node scene = GetTree().CurrentScene;
		navManager = scene.GetNodeOrNull<Node2D>("%NavigationManager") as NavigationManager;
        
		navManager?.AddArea(this);
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


    public override async void _ExitTree() {
	    
	    base._ExitTree();

	    
	    await TaskExtensions.SuspendWhile(() => !NavigationManager._initialized);
	    navManager?.RemoveArea(this);
    }
}
