using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace MenuSystem
{
    public class SelectedButtonMethods : MonoBehaviour, IPointerEnterHandler
    {
        public UnityEvent idleEvent;
        public UnityEvent selectionEvent;
        public UnityEvent pressedEvent;

        ButtonSelectionController btnController;

        [HideInInspector] private Button myButton;

        private void Awake()
        {
            myButton = GetComponent<Button>();
        }

        public void SetButtonController(ButtonSelectionController btnCont)
        {
            btnController = btnCont;
        }
        public void SetIdle()
        {
            idleEvent?.Invoke();
        }
        public void Selection()
        {
            selectionEvent?.Invoke();
        }
        public void Click()
        {
            pressedEvent?.Invoke();
        }
        public void ReturnButton()
        {
            btnController.clickedButton = this.GetComponent<Button>();
        }

        #region pointer enter exit handler
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (!CheckIfInteractable()) return;
            btnController.HoverSelectedButton(this.GetComponent<Button>());
        }
        public bool CheckIfInteractable() {
            if(myButton == null)
            {
                Debug.Log("Null");

            }
            if (!myButton.gameObject.activeInHierarchy) return false;
            return myButton.IsInteractable(); 
        }
        public void MakeInteractable(bool value)
        {
            if (value)
            {
                myButton.interactable = true;
                btnController.SetInteractableProperty(myButton);
            }
            else
            {
                myButton.interactable = false;
                btnController.SetUnInteractableProperty(myButton);
            }
        }
        #endregion
    }
}
