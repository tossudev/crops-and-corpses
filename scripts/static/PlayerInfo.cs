using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public static class PlayerInfo
{
	// Keys
	public const string PLAYER_HEALTH_KEY = "health";
	public const string PLAYER_STAMINA_KEY = "stamina";
	public const string PLAYER_CURRENT_SCENE_KEY = "sceneID";
	public const string PLAYER_ACTIVE_QUEST_KEY = "activeQuest";
	
	public static int health { get; set; }
	public static int stamina { get; set; }
	public static SceneID sceneID { get; set; }
	
	public static Quest activeQuest { get; set; }

	public static Dictionary GetDictionary()
	{
		Dictionary playerInfo = new Dictionary
		{
			{ PLAYER_HEALTH_KEY, health },
			{ PLAYER_STAMINA_KEY, stamina },
			{ PLAYER_CURRENT_SCENE_KEY, (int)sceneID},
			{ PLAYER_ACTIVE_QUEST_KEY, Quest.GetDictionary(activeQuest)}
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

		health = (int)playerInfo[PLAYER_HEALTH_KEY];
		stamina = (int)playerInfo[PLAYER_STAMINA_KEY];
		sceneID = (SceneID)(int)playerInfo[PLAYER_CURRENT_SCENE_KEY];
		activeQuest = Quest.LoadQuestFromData((Dictionary)playerInfo[PLAYER_ACTIVE_QUEST_KEY]);
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
	
	public static async Task<Quest> GetActiveQuest()
	{
		await TaskExtensions.SuspendWhile(() => !SaveData.firstLoadComplete);
		return activeQuest;
	}

	public static void SetActiveQuest(Quest quest)
	{
		activeQuest = quest;
	}
}
