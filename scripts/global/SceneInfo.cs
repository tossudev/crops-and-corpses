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

	public static Dictionary GetDictionary()
	{
		Dictionary sceneInfo = new Dictionary
		{
			{ "CaveBridgeOpen", caveBridgeOpen },
			{ "ruinsCaveOpen", ruinsCaveOpen },
			{ "forestBridgeOpen", forestBridgeOpen }
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
	}

	public static async Task<bool> GetCaveBridgeOpen()
	{
		if (!SaveData.firstLoadComplete)
		{
			await Task.Delay(100);
			return await GetCaveBridgeOpen();
		}

		return caveBridgeOpen;
	}

	public static async Task<bool> GetRuinsCaveOpen()
	{
		if (!SaveData.firstLoadComplete)
		{
			await Task.Delay(100);
			return await GetRuinsCaveOpen();
		}

		return ruinsCaveOpen;
	}

	public static async Task<bool> GetForestBridgeOpen()
	{
		if (!SaveData.firstLoadComplete)
		{
			await Task.Delay(100);
			return await GetForestBridgeOpen();
		}

		return forestBridgeOpen;
	}
}
