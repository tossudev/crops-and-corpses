using System;
using System.Collections.Generic;
using Godot.Collections;

public static class PlayerInfo
{
	public static int health { get; set; } = 100;
	public static int stamina { get; set; } = 100;
	public static PortalID travelID { get; set; }

	public static Dictionary GetDictionary()
	{
		Dictionary playerInfo = new Dictionary();

		playerInfo.Add("health", health);
		playerInfo.Add("stamina", stamina);
		playerInfo.Add("travelID", (int)travelID);

		return playerInfo;
	}
}
