using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;


public static class SceneManager
{
    static bool sceneChanged = true;
    static Node _currentRootScene;
    static AudioController _audioController;


    public static Scene.RootScene GetCurrentScene(Node caller)
    {
        if (sceneChanged) CacheCurrentScene(caller);

        return Scene.allRootScenes.Find(rootScene => rootScene.Name == _currentRootScene.Name);
    }

    public static bool IsCurrentScene(Node caller, IEnumerable<Scene.RootScene> scenes)
    {
        return scenes.Any(scene => CompareCurrentSceneTo(caller, scene));
    }

    public static bool IsCurrentScene(Node caller, Scene.RootScene scene)
    {
        return CompareCurrentSceneTo(caller, scene);
    }

    static bool CompareCurrentSceneTo(Node caller, Scene.RootScene scene)
    {
        if (sceneChanged) CacheCurrentScene(caller);

        return _currentRootScene.Name == scene.Name;
    }

    static void CacheCurrentScene(Node caller)
    {
        _currentRootScene = caller.GetTree().CurrentScene;
    }

    public static void ChangeScene(Node caller, Scene.RootScene scene)
    {
        if (IsCurrentScene(caller, Scene.Town))
        {
            BuildingMenu.buildMenu?.SaveBuildings();
        }

        PlayerController player;
        player = caller.GetTree().GetFirstNodeInGroup("player") as PlayerController;
        player.SaveState();

        SaveData.SyncAll();
        caller.GetTree().ChangeSceneToFile(scene.Path);
        sceneChanged = true;

        _audioController = caller.GetNode<Node>("/root/Audio") as AudioController;
        _audioController.PlayBackground();
    }
}