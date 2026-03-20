using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingBase : MonoBehaviour
{

    [Header("Gun Settings")]
    [SerializeField] protected PoolingSystem bulletPooling;
    [SerializeField] protected float maxShootDistance = 5f;

    [Header("Fire Setting")]
    [SerializeField] protected float fireRate = 0.5f;
    [SerializeField] protected float reloadTime = 1f;
    [SerializeField] protected Transform firePoint;

    [Header("Ammo Settings")]
    [SerializeField] protected bool infiniteAmmo = false;
    [SerializeField] protected int maxAmmoCapacity = 40;
    [SerializeField] protected int maxMagazineSize = 12;

    protected int currentAmmo;
    protected int currentAmmoInMagazine;

    protected bool isReloading = false;
    protected bool outOfAmmo = false;

    [Header("Target Settings")]
    [SerializeField] protected LayerMask targetLayerMask;
    [SerializeField] protected string targetTag = "Enemy";

    [Header("Bullet Settings")]
    [SerializeField] protected float damageAmount = 10f;


    [Header("Effects")]
    [SerializeField] protected ParticleSystem shootEffect;
    [SerializeField] protected AudioClip shootSoundClip;

    protected float lastFireTime;

    public virtual void SetWeapon(WeaponData _wpData)
    {
        bulletPooling.SetPoolingProperties(_wpData.bulletPrefab, 8);

        maxShootDistance = _wpData.maxShootDistance;
        damageAmount = _wpData.damageAmount;

        fireRate = _wpData.fireRate;
        reloadTime = _wpData.reloadTime;

        infiniteAmmo = _wpData.infiniteAmmo;
        maxAmmoCapacity = _wpData.maxAmmoCapacity;
        maxMagazineSize = _wpData.maxMagazineSize;

        shootEffect = _wpData.shootEffect;
        shootSoundClip = _wpData.shootSoundClip;

    }
   
    protected virtual void Shoot(Vector3 shootPos)
    {
        Vector2 dir = shootPos - firePoint.position;
        RaycastHit2D rayHit = Physics2D.Raycast(firePoint.position, dir, maxShootDistance,targetLayerMask);
        bool hit = rayHit.collider != null;
        Vector2 hitPos;
        if (hit) {
            hitPos = rayHit.point;
            //if (rayHit.collider.CompareTag(targetTag))
            //{
            //    rayHit.collider.GetComponent<Health>().TakeDamage(damageAmount);
            //}
        }
        else
        {
            hitPos = dir * maxShootDistance;
        }
        if(bulletPooling)
        {
            GameObject bullet = bulletPooling.GetPoolObject();
            bullet.SetActive(true);
            bullet.transform.position = firePoint.position;
            bullet.GetComponent<Bullet>().Initialize(hitPos,damageAmount);
        }
       
        PlayEffectAndSound();
    }
    protected void AmmoVariablesUpdate()
    {
        if(infiniteAmmo) return;
        currentAmmo--;
        currentAmmoInMagazine--;
        if (currentAmmo <= 0) {
            currentAmmo = 0;
            outOfAmmo = true;
            return;
        }
        if(currentAmmoInMagazine <= 0)
        {
            currentAmmoInMagazine = 0;
        }
    }
    protected IEnumerator Reload() // reload logic will be on the derived class
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        int ammoToReload = Mathf.Min(maxMagazineSize, currentAmmo);
        currentAmmoInMagazine = ammoToReload;
        isReloading = false;
    }

    public void TakeAmmo(int ammoAmount)
    {
        currentAmmo += ammoAmount;
        if (currentAmmoInMagazine <= 0) StartCoroutine(Reload());
        if (currentAmmo > 0) outOfAmmo = false;
        if(currentAmmo > maxAmmoCapacity) currentAmmo = maxAmmoCapacity;
    }
    void PlayEffectAndSound()
    {
        if (shootEffect) shootEffect.Play();
        if (shootSoundClip) SoundManager.Instance.PlayClip(shootSoundClip);
    }
}
