using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : Health
{
    [SerializeField] private Slider healthSlider;
    [Header("Hurt Effect")]
    public ParticleSystem healEffect;
    public ParticleSystem hurtEffect;
    public GameObject dieEffect;
    [Header("Audio Clip")]
    public AudioClip healClip;
    public AudioClip hurtClip;
    public AudioClip dieClip;
    protected override void Start()
    {
        base.Start();
        healthSlider.maxValue = MaxHealth;
        healthSlider.value = health;
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        healthSlider.value = health;

        GetComponent<PlayerMovement>().SetPlayerState(PlayerState.Die);

        if (hurtEffect) hurtEffect.Play();
        SoundManager.Instance.PlayClip(hurtClip, Random.Range(0.2f, 0.5f));
    }
    public override void Heal()
    {
        if (healEffect) healEffect.Play();
        SoundManager.Instance.PlayClip(healClip, Random.Range(0.6f, 1f));
    }
    public override void Die()
    {
        if (dieEffect)
        {
            Instantiate(dieEffect, transform.position, Quaternion.identity);
        }
        SoundManager.Instance.PlayClip(dieClip, Random.Range(0.6f, 1f));
    }
}
