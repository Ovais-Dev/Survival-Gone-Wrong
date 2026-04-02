using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EquipmentHolder : MonoBehaviour,ITriggable, IInteractable
{
    [SerializeField]private Equipment item;
    [Header("Trigger System")]
    [SerializeField] private string message;
    [SerializeField]private GameObject itemObj;
    [SerializeField]private GameObject lightObj;
    [SerializeField]private UnityEvent interactEventCall;
    [Header("Material Control")]
    public SpriteRenderer objSprite;
    public Material unlitMaterial;
    public Material litMaterial;

    [Header("Audio Clip")]
    [SerializeField] private AudioClip equipClip;

    bool holdingItems = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(objSprite==null)
        objSprite = GetComponentInChildren<SpriteRenderer>();
        TriggerExit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool CanInteract()=>holdingItems;
    public void MakeInteractable(){return;}
    
    public void Interact()
    {
        holdingItems = false;
        itemObj.SetActive(false);

        item.Equip();
        interactEventCall?.Invoke();

        MessagePopup.Instance.HidePopup();
        SoundManager.Instance.PlayClip(equipClip, Random.Range(0.5f, 0.8f));
    }
    public string TriggeredMessage()=>message;
    public bool ShowMessage()=>holdingItems;
    public void TriggerEnter()
    {
        if(holdingItems){
            MessagePopup.Instance.ShowPopup(message);
        }
            lightObj.SetActive(true);
            objSprite.material = unlitMaterial;
        
    }
    public void TriggerExit()
    {
        lightObj.SetActive(false);
        objSprite.material = litMaterial;
        MessagePopup.Instance.HidePopup();
    }
}
