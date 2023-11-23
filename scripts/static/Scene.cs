public static class Scene
{
    public class RootScene
    {
        public string Path { get; }
        public string Name { get; }

        public RootScene(string path, string name)
        {
            Name = name;
            Path = path;
        }
    }

    public static readonly RootScene Town = new("res://scenes/town.tscn", "Town");
    public static readonly RootScene Forest = new("res://scenes/forest.tscn", "Forest");
    public static readonly RootScene Ruins = new("res://scenes/ruins.tscn", "Ruins");
    public static readonly RootScene Cave = new("res://scenes/cave.tscn", "Cave");
}