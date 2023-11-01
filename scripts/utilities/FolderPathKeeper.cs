using Godot;

[GlobalClass]
public partial class FolderPathKeeper : Resource
{
    public string GetFolderPath()
    {
        return ResourcePath.GetBaseDir() + "/";;
    }
}