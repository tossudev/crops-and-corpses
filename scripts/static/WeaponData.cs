using Godot.Collections;

public static class WeaponData
{

	const string WEAPON_DIRECTORIES_PATH = "res://assets/resources/game_weapons/weapon_path_container.tres";
	
	static bool _weaponDataInitiated = false;
	public static bool weaponDataInitiated => _weaponDataInitiated;
	
    static Dictionary weapons = new();


    public static void InitiateWeaponData()
    {
	    if (_weaponDataInitiated) return;
	    
	    LoadWeaponsFromPath();
	    _weaponDataInitiated = true;
    }

	static void LoadWeaponsFromPath()
	{
		foreach (var resource in FileLoader._LoadResourcesFromEachPath(WEAPON_DIRECTORIES_PATH))
		{
			if (resource is Weapon weapon)
			{
				weapons.Add(weapon.item.ID, resource);
			}
		}
	}

	public static Weapon GetWeaponByItemId(int id)
	{
		if (!_weaponDataInitiated)
		{
			InitiateWeaponData();
		}
		
		if (weapons.TryGetValue(id, out var weapon))
		{
			return (Weapon)weapon;
		}

		return null;
	}
}
