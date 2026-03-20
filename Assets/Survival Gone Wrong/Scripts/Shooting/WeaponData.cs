using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/Weapon")]
public class WeaponData : ScriptableObject
{
    [Header("Basic Properties")]
    public string weaponName;
    public GameObject bulletPrefab;

    [Header("Gun Properties")]
    public float maxShootDistance;
    public float damageAmount = 10f;

    [Header("Fire Setting")]
     public float fireRate = 0.5f;
   public float reloadTime = 1f;

    [Header("Ammo Settings")]
    public bool infiniteAmmo = false;
    public int maxAmmoCapacity = 40;
    public int maxMagazineSize = 12;

    [Header("Effects")]
    public ParticleSystem shootEffect;
    public AudioClip shootSoundClip;

    [Header("Other Properties")]
    public float shootBaseSound = 5f;
}
