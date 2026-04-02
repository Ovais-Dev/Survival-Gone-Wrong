using System.Collections.Generic;
using UnityEngine;

public class WeaponDatabase : MonoBehaviour
{
    public static WeaponDatabase Instance;

    public List<WeaponData> allWeapons;

    private Dictionary<string, WeaponData> weaponDict;

    private void Awake()
    {
        Instance = this;

        weaponDict = new Dictionary<string, WeaponData>();

        foreach (var weapon in allWeapons)
        {
            weaponDict[weapon.weaponName] = weapon;
        }
        DontDestroyOnLoad(gameObject);
    }

    public WeaponData GetWeapon(string id)
    {
        return weaponDict.ContainsKey(id) ? weaponDict[id] : null;
    }
}