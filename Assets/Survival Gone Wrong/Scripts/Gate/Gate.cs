
using UnityEngine;

public class Gate : MonoBehaviour, ITriggable, IInteractable
{
    public string conditionMessage;
    public string triggerMessage; // message when player enter the region
    [Header("Gate Setting")]
    public bool onExitClose = true; // close gate when out of the trigger collider / box

    public bool closed = false; // check if the gate is closed from external factor like mission or else thing. if yes then don't open even after enter the region

    public bool keyCardNeeded = true;

    public bool havePutKey = false;

    [Header("Audio Clip")]
    [SerializeField] private AudioClip gateBuzzer;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if(!keyCardNeeded)havePutKey = true;
    }
    public void GateOpen()
    {
        anim.SetTrigger("Gate");
    }
    public void MakeInteractable()
    {
        if(keyCardNeeded)
        havePutKey = true;
    }
    public void Interact()
    {
        GateOpen();
        SoundManager.Instance.PlayClip(gateBuzzer, Random.Range(0.5f, .6f));
        SoundManager.EmitSound(transform.position, 3f);
        // move towards
    }
    public bool CanInteract() => keyCardNeeded ? havePutKey : true;

    public string TriggeredMessage() => triggerMessage;
    public bool ShowMessage() => !havePutKey;

    public void TriggerEnter()
    {
        if (closed || !havePutKey)
        {
            MessagePopup.Instance.ShowPopup(conditionMessage);
            return;
        }
        // if(keyCardNeeded){
        //     MessagePopup.Instance.ShowPopup(conditionMessage);
        //     return;
        // }
        //if (havePutKey) {
        MessagePopup.Instance.ShowPopup(triggerMessage);
        //}
        //else 
        //{
        //}
        
    }
    public void TriggerExit()
    {
        if (!havePutKey) return;
        if (onExitClose) GateOpen();
        MessagePopup.Instance.HidePopup();
    }
    public void CloseGate()
    {
        closed = true;
    }
    public void CloseGate(string msg)
    {
        conditionMessage = msg;
        CloseGate();
    }
    public bool NeedKey()
    {
        return keyCardNeeded && !havePutKey;
    }
}
