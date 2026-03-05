using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : ShootingBase
{
    protected bool autoReload = true;
    private void Start()
    {
        lastFireTime = Time.time;
        currentAmmo = maxAmmoCapacity;
        currentAmmoInMagazine = maxMagazineSize;
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
    }

    
}
