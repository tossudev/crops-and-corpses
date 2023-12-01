using System.Linq;
using Godot;
using Godot.Collections;
using Microsoft.VisualBasic.CompilerServices;

public static class FileLoader
{
    
    public static Array<Resource> _LoadResourcesFromEachPath(string pathContainerPath)
    {
        Array<Resource> resourcesFromEachPath = new();

        if (LoadCustomResource(pathContainerPath) is not PathContainer pathContainer)
        {
            GD.PushError("PathContainer was invalid");
            return resourcesFromEachPath;
        }
        
        foreach (var keeper in pathContainer.paths)
        {
            resourcesFromEachPath.AddRange(_LoadResourcesFromPath(keeper.GetFolderPath()));
        }

        return resourcesFromEachPath;
    }
    
    static Array<Resource> _LoadResourcesFromPath(string path)
    {

        Array<Resource> resourcesFromPath = new();
        
        using var dir = DirAccess.Open(path);
        // Open item directory
        if (dir != null) {
            dir.ListDirBegin();
            string fileName = dir.GetNext();

            // Add all items from directory to resource array
            while (fileName != "") {

                var resource = LoadCustomResource(path + fileName);

                if (resource is not null)
                {
                    resourcesFromPath.Add(resource);
                }
                else
                {
                    GD.PushWarning("Failed to load resource (" + path + fileName + ")!");
                }
                
                fileName = dir.GetNext();
            }
        }

        return resourcesFromPath;
    }

    public static Resource LoadCustomResource(string filePath)
    {
        filePath = filePath.TrimSuffix(".remap");
                
        return ResourceLoader.Load(filePath);
    }
}