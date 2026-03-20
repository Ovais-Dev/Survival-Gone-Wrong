using UnityEngine;

public abstract class Health : MonoBehaviour
{
    [SerializeField] private float health;

    public float InitialHealth { get; private set; }
    protected virtual void Start()
    {
        InitialHealth = health;
    }
    public void IncreaseHealth(float amount)
    {
        if (health <= 0) return;
        health += amount;
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
