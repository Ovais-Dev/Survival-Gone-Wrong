using UnityEngine;

public class WeaponEquipment : Equipment
{
    public WeaponData weapon;
    public override void Equip()
    {
        FindFirstObjectByType<WeaponHandler>().SetWeapon(weapon);
    }
}
