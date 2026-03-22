using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : Health
{
    [SerializeField] private Slider healthSlider;
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
    }
    public override void Heal()
    {
        return;
    }
    public override void Die()
    {
        return;
    }
}
