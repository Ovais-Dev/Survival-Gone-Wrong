using UnityEngine;

public class EnemyHealth : Health
{
    public override void Die()
    {
        Debug.Log("Dead Enemies");
    }

    public override void Heal()
    {
        throw new System.NotImplementedException();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        Debug.Log("Got me!");
        // Play hit effect and sound
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
