using UnityEngine;

public abstract class Health : MonoBehaviour
{
    [SerializeField] protected float health;

    protected float MaxHealth { get; private set; }

    protected virtual void Start()
    {
        MaxHealth = health;
    }
    public void IncreaseHealth(float amount)
    {
        if (health <= 0) return;
        health += amount;
        if (health >= MaxHealth) health = MaxHealth;
        Heal(); // heal logic when healing depending upon character types
    }
    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0) Die(); //Die logic depending upon character types
    }
    public abstract void Heal();
    public abstract void Die();
}
