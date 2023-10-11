using Godot;
using System;
using System.Linq;

public partial class WeaponData : Node
{

	[Export] public string weaponDirectory = "res://assets/resources/game_weapons/";
	public static Godot.Collections.Dictionary weapons = new();

	public override void _Ready()
	{
		_LoadWeaponsFromPath();
	}

	void _LoadWeaponsFromPath()
	{
		using var dir = DirAccess.Open(weaponDirectory);
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();

			while (fileName != "")
			{
				string filePath = weaponDirectory + fileName;
				var resource = (Weapon)GD.Load(filePath);
				weapons.Add(resource.item.ID, resource);

				fileName = dir.GetNext();
			}
		}
	}

	public static Weapon GetWeaponByItem(int id)
	{
		if (weapons.TryGetValue(id, out var weapon))
		{
			return (Weapon)weapon;
		}

		return null;
	}
}
