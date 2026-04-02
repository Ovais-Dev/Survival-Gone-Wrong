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

    [SerializeField] private PlayerAnimation playerAnimation;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Audio Clip")]
    [SerializeField] private AudioClip shootClip;
    private void Start()
    {
        lastFireTime = Time.time;
        currentAmmo = maxAmmoCapacity;
        currentAmmoInMagazine = maxMagazineSize;

        Physics2D.queriesHitTriggers = false;
    }
    public override void SetWeapon(WeaponData _wpData)
    {
        base.SetWeapon(_wpData);
        shootBaseSound = _wpData.shootBaseSound;
    }
    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame && currentAmmoInMagazine < maxMagazineSize)
        {
            StartCoroutine(Reload());
        }

        if (isReloading || outOfAmmo)
        {
            if (playerAnimation != null)
                playerAnimation.SetState(PlayerAnimation.AnimState.Idle);
            return;
        }

        bool isShooting = Mouse.current.leftButton.isPressed;

        if (isShooting)
        {
            if (playerAnimation != null)
            {
                if (!playerMovement.CheckMyState(PlayerState.Attack)) playerMovement.SetPlayerState(PlayerState.Attack);
                playerAnimation.SetState(PlayerAnimation.AnimState.Attack);
            }

            if (lastFireTime + 1 / fireRate > Time.time) return;

            Vector3 mousePos = CursorObj.Instance.GetMouseWorldPosition();
            Shoot(mousePos);
            lastFireTime = Time.time;
        }
        else
        {
            if (!playerMovement.CheckMyState(PlayerState.Normal) && !playerMovement.CheckMyState(PlayerState.Die)) playerMovement.SetPlayerState(PlayerState.Normal);
        }
    }
    protected override void Shoot(Vector3 shootPos)
    {
        if(!canShoot)return;
        base.Shoot(shootPos);
        AmmoVariablesUpdate();
        if(currentAmmoInMagazine<=0 && autoReload)
        {
            StartCoroutine(Reload());
        }
        LightMuzzleEffect();
        UpdateAmmoUI();
        SoundManager.EmitSound(transform.position, shootBaseSound); // for zombies attraction
        SoundManager.Instance.PlayClip(shootClip, Random.Range(0.5f, 0.6f));
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
