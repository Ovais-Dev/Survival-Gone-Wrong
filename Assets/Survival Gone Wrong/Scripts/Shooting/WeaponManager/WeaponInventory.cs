using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    public static WeaponInventory Instance;

    public List<WeaponData> ownedWeapons;
    public int currentWeaponIndex = 0;

    private const string SAVE_KEY = "PLAYER_WEAPONS";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadWeapons(); // ✅ LOAD ON START
    }

    public void AddWeapon(WeaponData weapon)
    {
        if (!ownedWeapons.Contains(weapon))
        {
            ownedWeapons.Add(weapon);
            SaveWeapons(); // ✅ SAVE AFTER ADD
        }
    }

    public void SaveWeapons()
    {
        List<string> weaponIDs = new List<string>();

        foreach (var weapon in ownedWeapons)
        {
            weaponIDs.Add(weapon.weaponName);
        }

        string json = JsonUtility.ToJson(new WeaponSaveData(weaponIDs, currentWeaponIndex));
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public void LoadWeapons()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
            return;

        string json = PlayerPrefs.GetString(SAVE_KEY);
        WeaponSaveData data = JsonUtility.FromJson<WeaponSaveData>(json);

        ownedWeapons.Clear();

        foreach (string id in data.weaponIDs)
        {
            WeaponData weapon = WeaponDatabase.Instance.GetWeapon(id);

            if (weapon != null)
                ownedWeapons.Add(weapon);
        }

        currentWeaponIndex = data.currentIndex;
    }

    public void SetCurrentIndex(int index)
    {
        currentWeaponIndex = index;
        //SaveWeapons(); // ✅ SAVE SWITCH
    }
    private void OnApplicationQuit()
    {
        SaveWeapons();
    }
}

[System.Serializable]
public class WeaponSaveData
{
    public List<string> weaponIDs;
    public int currentIndex;

    public WeaponSaveData(List<string> ids, int index)
    {
        weaponIDs = ids;
        currentIndex = index;
    }
}