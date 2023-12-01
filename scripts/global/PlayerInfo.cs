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

	private static bool _isInitialized = false;

	public static Dictionary GetDictionary()
	{
		Dictionary playerInfo = new Dictionary
		{
			{ "health", health },
			{ "stamina", stamina },
			{ "travelID", (int)travelID }
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
		travelID = (PortalID)Enum.ToObject(typeof(PortalID), (int)playerInfo["travelID"]);

		_isInitialized = true;
	}

	public static async Task<int> GetHealth()
	{
		if (!_isInitialized)
		{
			await Task.Delay(100);
			return await GetHealth();
		}

		return health;
	}

	public static async Task<int> GetStamina()
	{
		if (!_isInitialized)
		{
			await Task.Delay(100);
			return await GetStamina();
		}

		return stamina;
	}
}
