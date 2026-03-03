using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;

    private Vector2 moveDir;

    public void Initialize(Vector2 dir)
    {
        dir.Normalize();
        moveDir = dir;
        transform.up = dir;
    }

    private void OnEnable()
    {
        Invoke(nameof(Deactivate), lifeTime);
    }

    private void Update()
    {
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

}
