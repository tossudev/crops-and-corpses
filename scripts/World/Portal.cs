using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Portal : Node2D
{
	[Export] public SceneID targetSceneID;
	[Export] public Node2D exitPosition;

	private PortalManager _portalManager;

	public override void _Ready()
	{
		_portalManager = GetParent<PortalManager>();
	}

	public void OnBodyEntered(Node body)
	{
		if (body is PlayerController)
		{
			_portalManager.PortalTo(targetSceneID);
		}
	}
}
