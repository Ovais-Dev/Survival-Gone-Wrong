using UnityEngine;

public class WeaponEquipment : Equipment
{
    public WeaponData weaponData;
    public override void Equip()
    {
        FindFirstObjectByType<WeaponHandler>().SetWeapon(weaponData);
       // WeaponInventory.Instance.AddWeapon(weaponData);
    }
}
