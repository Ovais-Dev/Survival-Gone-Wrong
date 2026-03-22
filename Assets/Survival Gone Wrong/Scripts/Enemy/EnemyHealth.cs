using UnityEngine;

public class EnemyHealth : Health
{
    public override void Die()
    {
        gameObject.SetActive(false);
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
