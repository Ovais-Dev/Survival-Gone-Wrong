using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private float minDistance = 0.2f;
    private Vector2 moveToPos;
    private Vector2 moveDir;
    private float damage;

    public void Initialize(Vector2 pos,float dmg)
    {
        moveToPos = pos;
        Vector2 dir = moveToPos - (Vector2)transform.position;
        dir.Normalize();
        moveDir = dir;
        transform.up = dir;
        damage = dmg;
    }

    //private void OnEnable()
    //{
    //    Invoke(nameof(Deactivate), lifeTime);
    //}

    private void Update()
    {
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
        if (Vector2.Distance(transform.position, moveToPos) < minDistance)
        {
            Deactivate();
        }
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<Health>()?.TakeDamage(damage);
        Deactivate();
    }
}
