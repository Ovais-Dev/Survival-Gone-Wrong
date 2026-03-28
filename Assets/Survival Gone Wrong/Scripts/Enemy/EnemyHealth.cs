using UnityEngine;
using UnityEngine.Events;
public class EnemyHealth : Health
{
    [SerializeField] private GameObject deadEffect;
    [SerializeField] protected UnityEvent onDieEvent;
    public override void Die()
    {
        gameObject.SetActive(false);
        Instantiate(deadEffect, transform.position, Quaternion.identity);
        onDieEvent?.Invoke();
    }


    public override void Heal()
    {
        Debug.Log("Enemy Healed");
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        GetComponent<ZombieAI>().Hurt();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
