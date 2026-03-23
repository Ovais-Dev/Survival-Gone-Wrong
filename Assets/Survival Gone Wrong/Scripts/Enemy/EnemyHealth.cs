using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private GameObject deadEffect;
    public override void Die()
    {
        gameObject.SetActive(false);
        Instantiate(deadEffect, transform.position, Quaternion.identity);
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
