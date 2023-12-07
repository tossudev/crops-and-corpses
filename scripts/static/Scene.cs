using System;
using System.Collections.Generic;
using System.Linq;

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

        public override string ToString()
        {
            return Name;
        }
    }

     
    
    public static readonly RootScene Town = new("res://scenes/town.tscn", "Town");
    public static readonly RootScene Forest = new("res://scenes/forest.tscn", "Forest");
    public static readonly RootScene Ruins = new("res://scenes/ruins.tscn", "Ruins");
    public static readonly RootScene Cave = new("res://scenes/cave.tscn", "Cave");
    public static readonly RootScene Menu = new("res://scenes/ui/main_menu.tscn", "Menu");
    
    public static List<RootScene> allRootScenes = new()
    {
        Town,
        Forest,
        Ruins,
        Cave,
        Menu,
    };

    public static RootScene GetRootSceneByName(string name)
    {
        return allRootScenes.Find(
            rootScene => string.Equals(rootScene.Name, name, StringComparison.CurrentCultureIgnoreCase));
    }
}