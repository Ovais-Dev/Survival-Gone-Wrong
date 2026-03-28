using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public bool haveKey = false;

    bool haveUsedKey = false;
    bool insideInteractonZone = false;
    IInteractable interactable;

    private void Update()
    {
        if (interactable == null) return;
        if (Input.GetKeyDown(KeyCode.E) && insideInteractonZone)
        {
            if(interactable==null)return;
            if (interactable.CanInteract()) interactable.Interact();
        }
    }
    public void CollectKey()
    {
        haveKey = true;
        haveUsedKey = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LevelCollider"))
        {
            LevelManager.Instance.GoToNextLevel();
            return;
        }
        if (collision.CompareTag("KeyCard"))
        {
            CollectKey();
            Destroy(collision.gameObject);
        }
        IInteractable i = collision.GetComponentInParent<IInteractable>();
        ITriggable t = collision.GetComponentInParent<ITriggable>();
        if (i!=null)
        {
            interactable = i;
        }

        if (t!=null)
        {
            insideInteractonZone = true;
            if(interactable!=null){
                if (interactable is Gate g)
                {
                    if (!g.NeedKey()) return;
                    if (haveKey)
                    {
                        interactable.MakeInteractable();
                        haveKey = false;
                        //haveUsedKey = true;
                    }
                    
                }
            }
            t.TriggerEnter();
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable i = collision.GetComponentInParent<IInteractable>();
        ITriggable t = collision.GetComponentInParent<ITriggable>();
        if (i!=null)
        {
            interactable = null;
        }

        if(t!=null)
        {
            insideInteractonZone = false;
            t.TriggerExit();
        }
    }
}
