using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingBase : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] protected float maxShootDistance = 5f;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform bulletPoolParent;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected LayerMask targetLayerMask;

    [Header("Bullet Settings")]
    [SerializeField] protected float damageAmount = 10f;


    [Header("Effects")]
    [SerializeField] protected ParticleSystem shootEffect;
    [SerializeField] protected AudioClip shootSound;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePos = CursorObj.Instance.GetMouseWorldPosition();
            Shoot(mousePos);
        }

    }
    public virtual void Shoot(Vector3 shootPos)
    {
        Vector2 dir = shootPos - firePoint.position;
        RaycastHit2D rayHit = Physics2D.Raycast(firePoint.position, dir, maxShootDistance,targetLayerMask);
        bool hit = rayHit.collider != null;
        if (hit) {
            if (rayHit.collider.CompareTag("Enemy"))
            {
                rayHit.collider.GetComponent<Health>().TakeDamage(damageAmount);
            }
        }
        if(bulletPrefab)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity, bulletPoolParent);
            bullet.GetComponent<Bullet>().Initialize(dir.normalized);
        }
        PlayEffectAndSound();
    }
    void PlayEffectAndSound()
    {
        if (shootEffect) shootEffect.Play();
        if (shootSound) SoundManager.Instance.PlayClip(shootSound);
    }
}
