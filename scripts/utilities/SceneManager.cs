using System.Collections.Generic;
using System.Linq;
using Godot;


public static class SceneManager
{
    static bool sceneChanged = true;
    static Node _currentRootScene;
    
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
        SaveData.SyncAll();
        caller.GetTree().ChangeSceneToFile(scene.Path);
        sceneChanged = true;
    }
}