using Godot;

[GlobalClass]
public partial class PathContainer : Resource
{
    [Export] FolderPathKeeper[] _paths;
    public FolderPathKeeper[] paths => _paths;
}