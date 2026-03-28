using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentZone : MonoBehaviour
{
    public Collider2D environmentCollider;
    [SerializeField]private LayerMask targetLayer;
    public List<GameObject> environmentObjects;

    
    void Start()
    {
        if(environmentCollider==null)
        environmentCollider = GetComponent<Collider2D>();

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.useLayerMask = true;
        filter.layerMask = targetLayer;

        Collider2D[] results = new Collider2D[20]; // adjust size if needed
        int count = environmentCollider.Overlap(filter, results);
        Debug.Log(count);
        for (int i = 0; i < count; i++)
        {
            //if(results[i].gameObject==null)continue;
            //if(results[i].CompareTag("Player"))continue;
           environmentObjects.Add(results[i].gameObject);
           results[i].gameObject.SetActive(false);
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach(var env in environmentObjects)
            {
                if(env==null)continue;
                env.SetActive(true);
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach(var env in environmentObjects)
            {
                if(env==null)continue;
                env.SetActive(false);
            }
        }
    }
}
