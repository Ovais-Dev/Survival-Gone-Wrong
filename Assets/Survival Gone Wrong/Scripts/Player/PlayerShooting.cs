using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerShooting : ShootingBase
{
    protected bool autoReload = true;

    [Space(10)]
    [SerializeField] private GameObject lightMuzzleEffect;
    [SerializeField] private float lightMuzzleDuration = 0.1f;

    [Space(10)]
    [SerializeField] private float shootBaseSound = 5f;
    private void Start()
    {
        lastFireTime = Time.time;
        currentAmmo = maxAmmoCapacity;
        currentAmmoInMagazine = maxMagazineSize;
    }
    public override void SetWeapon(WeaponData _wpData)
    {
        base.SetWeapon(_wpData);
        shootBaseSound = _wpData.shootBaseSound;
    }
    private void Update()
    {
        if(Keyboard.current.rKey.wasPressedThisFrame && currentAmmoInMagazine<maxMagazineSize)
        {
            StartCoroutine(Reload());
        }
        if(isReloading || outOfAmmo) return;

        if (Mouse.current.leftButton.isPressed)
        {
            if (lastFireTime + 1 / fireRate > Time.time) return;
            Vector3 mousePos = CursorObj.Instance.GetMouseWorldPosition();
            Shoot(mousePos);
            lastFireTime = Time.time;
        }
    }
    protected override void Shoot(Vector3 shootPos)
    {
        base.Shoot(shootPos);
        AmmoVariablesUpdate();
        if(currentAmmoInMagazine<=0 && autoReload)
        {
            StartCoroutine(Reload());
        }
        LightMuzzleEffect();
        SoundManager.EmitSound(transform.position, shootBaseSound);
    }
    void LightMuzzleEffect()
    {
        if (!lightMuzzleEffect.activeInHierarchy)
        {
            StartCoroutine(InvokeLightEffect());
        }
    }
    IEnumerator InvokeLightEffect()
    {
        lightMuzzleEffect.SetActive(true);
        yield return new WaitForSeconds(lightMuzzleDuration);
        lightMuzzleEffect.SetActive(false);
    }
}
