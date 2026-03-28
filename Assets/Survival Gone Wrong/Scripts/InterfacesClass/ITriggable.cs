using UnityEngine;

public interface ITriggable
{
    string TriggeredMessage();
    bool ShowMessage();
    void TriggerEnter();
    void TriggerExit();
}
