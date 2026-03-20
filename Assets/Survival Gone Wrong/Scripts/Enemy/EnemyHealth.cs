using UnityEngine;

public class EnemyHealth : Health
{
    public override void Die()
    {
        gameObject.SetActive(false);
    }


    public override void Heal()
    {
        throw new System.NotImplementedException();
    }

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
