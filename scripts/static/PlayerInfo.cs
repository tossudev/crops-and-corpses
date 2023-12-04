using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public static class PlayerInfo
{
	public static int health { get; set; }
	public static int stamina { get; set; }
	public static SceneID sceneID { get; set; }

	public static Dictionary GetDictionary()
	{
		Dictionary playerInfo = new Dictionary
		{
			{ "health", health },
			{ "stamina", stamina },
			{ "sceneID", (int)sceneID}
		};

		return playerInfo;
	}

	public static void LoadPlayerInfo(Dictionary saveData)
	{
		if (saveData == null)
		{
			GD.PrintErr("Save data is null, can't load player info!");
			return;
		}

		Dictionary playerInfo = (Dictionary)saveData[SaveData.PLAYER_INFO_KEY];

		health = (int)playerInfo["health"];
		stamina = (int)playerInfo["stamina"];
		sceneID = (SceneID)(int)playerInfo["sceneID"];
	}

	public static async Task<int> GetHealth()
	{
		await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete);
		return health;
	}

	public static async Task<int> GetStamina()
	{
		await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete);
		return stamina;
	}
}
