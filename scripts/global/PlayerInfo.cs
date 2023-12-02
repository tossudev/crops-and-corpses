using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public static class PlayerInfo
{
	public static int health { get; set; }
	public static int stamina { get; set; }
	public static PortalID travelID { get; set; }

	public static Dictionary GetDictionary()
	{
		Dictionary playerInfo = new Dictionary
		{
			{ "health", health },
			{ "stamina", stamina }
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
	}

	public static async Task<int> GetHealth()
	{
		if (!SaveData.firstLoadComplete)
		{
			await Task.Delay(100);
			return await GetHealth();
		}

		return health;
	}

	public static async Task<int> GetStamina()
	{
		if (!SaveData.firstLoadComplete)
		{
			await Task.Delay(100);
			return await GetStamina();
		}

		return stamina;
	}
}
