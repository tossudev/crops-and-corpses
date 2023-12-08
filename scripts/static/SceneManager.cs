using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;


public static class SceneManager
{
    static bool sceneChanged = true;
    public static bool sceneChanging;
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

    public static async void ChangeScene(Node caller, Scene.RootScene scene)
    {
        sceneChanging = true;

        await TaskExtensions.SuspendWhile(() => NavigationManager.bakeInProgress);
        CommitSceneChange(caller, scene);

        await TaskExtensions.SuspendWhile(() => _sceneChange == Error.Ok);
        
        sceneChanging = false;
        sceneChanged = true;
    }

    static Error _sceneChange;
    static void CommitSceneChange(Node caller, Scene.RootScene scene)
    {
        if (IsCurrentScene(caller, Scene.Town))
        {
            BuildingMenu.buildMenu?.SaveBuildings();
        }

        var player = caller.GetTree().GetFirstNodeInGroup("player") as PlayerController;
        player?.SaveState();

        SaveData.SyncAll();
        _sceneChange = caller.GetTree().ChangeSceneToFile(scene.Path);
    }
}