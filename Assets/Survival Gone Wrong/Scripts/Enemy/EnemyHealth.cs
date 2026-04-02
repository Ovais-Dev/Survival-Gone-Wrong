using UnityEngine;
using UnityEngine.Events;
public class EnemyHealth : Health
{
    [SerializeField] private GameObject deadEffect;
    [SerializeField] private bool spawnAtDead;
    [SerializeField] private GameObject objectToBeSpawned;

    [Space(10)]
    public AudioClip deadClip;
    public override void Die()
    {
        Instantiate(deadEffect, transform.position, Quaternion.identity);
        if (spawnAtDead) Instantiate(objectToBeSpawned, transform.position, Quaternion.identity);
        if (deadClip) SoundManager.Instance.PlayClip(deadClip);
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
