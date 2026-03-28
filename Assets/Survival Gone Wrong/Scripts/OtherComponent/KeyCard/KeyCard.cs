using UnityEngine;

public class KeyCard : MonoBehaviour, IInteractable
{
    public bool CanInteract()=>true;
    public void MakeInteractable(){return;}
    public void Interact()
    {
        FindFirstObjectByType<PlayerInteraction>().CollectKey();
        Destroy(gameObject);
    }
}
