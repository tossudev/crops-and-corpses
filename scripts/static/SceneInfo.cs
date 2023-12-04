using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class SceneInfo
{
	public static bool caveBridgeOpen { get; set; }
	public static bool ruinsCaveOpen { get; set; }
	public static bool forestBridgeOpen { get; set; }
	public static bool forestBuildABridgeOpen { get; set; }

	public static Dictionary GetDictionary()
	{
		Dictionary sceneInfo = new Dictionary
		{
			{ "CaveBridgeOpen", caveBridgeOpen },
			{ "ruinsCaveOpen", ruinsCaveOpen },
			{ "forestBridgeOpen", forestBridgeOpen },
			{ "forestBuildABridgeOpen", forestBuildABridgeOpen}
		};

		return sceneInfo;
	}

	public static void LoadSceneInfo(Dictionary saveData)
	{
		if (saveData == null)
		{
			GD.PrintErr("Save data is null, can't load scene info!");
			return;
		}

		Dictionary sceneInfo = (Dictionary)saveData[SaveData.SCENE_INFO_KEY];

		caveBridgeOpen = (bool)sceneInfo["CaveBridgeOpen"];
		ruinsCaveOpen = (bool)sceneInfo["ruinsCaveOpen"];
		forestBridgeOpen = (bool)sceneInfo["forestBridgeOpen"];
		forestBuildABridgeOpen = (bool)sceneInfo["forestBuildABridgeOpen"];
	}

	public static async Task<bool> GetCaveBridgeOpen()
	{
		await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete);
		return caveBridgeOpen;
	}

	public static async Task<bool> GetRuinsCaveOpen()
	{
		await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete);
		return ruinsCaveOpen;
	}

	public static async Task<bool> GetForestBridgeOpen()
	{
		await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete);
		return forestBridgeOpen;
	}

	public static async Task<bool> GetForestBuildABridgeOpen()
	{
		await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete);
		return forestBuildABridgeOpen;
	}
}
