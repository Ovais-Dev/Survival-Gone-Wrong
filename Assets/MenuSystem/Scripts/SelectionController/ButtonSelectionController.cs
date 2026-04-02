using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace MenuSystem
{
    [System.Serializable]
    class CustomSelectionProperty
    {
        public Sprite sprite;
        public Color buttonColor;
        public Color textColor;
    }
    [System.Serializable]
    class TweeningSetting
    {

    }
    public class ButtonSelectionController : MonoBehaviour
    {

        [SerializeField] public List<Button> buttonList;

        [SerializeField] private bool useSelectionProperty = true;
        [SerializeField] CustomSelectionProperty selectionProperty;

        [HideInInspector] [SerializeField] CustomSelectionProperty initialProperty;

        [SerializeField] private bool usePressedProperty = true;
        [SerializeField] CustomSelectionProperty pressedProperty;

        [SerializeField] private bool useDisabledProperty = true;
        [SerializeField] CustomSelectionProperty disableProperty;

        [HideInInspector] public Button clickedButton;

        //[Header("Tweening Properties")]
        //[SerializeField] private bool useLeanTweenFeatures;
        //[SerializeField] private bool selectedTweening;
        //[SerializeField] private bool pressedTweening;
        //[SerializeField] private Vector3 i_scale;
        //[SerializeField] private Vector3 s_scale;
        //[SerializeField] private LeanTweenType 
        //[SerializeField] private Vector3 p_scale;


        public Button selectedButton;
        public int selectedIndex = 0;


        int currentBtnSelectedIndex = 0;
        private void Start()
        {
            SetInitialButtonProperty();
            InitialSelection();
        }

        void SetInitialButtonProperty()
        {

            for (int i = 0; i < buttonList.Count; i++)
            {
                Button btn = buttonList[i];
                //if (!btn.gameObject.activeInHierarchy) continue;

                SelectedButtonMethods sbm = btn.GetComponent<SelectedButtonMethods>();
                sbm.SetButtonController(this);
                btn.onClick.AddListener(sbm.ReturnButton);
                btn.onClick.AddListener(ButtonPressedProperty);
                //Debug.Log((sbm.GetComponent<Button>()) ? "Button exist" : "Doesn't exist");
                if (btn.IsInteractable()) continue;
                if (useDisabledProperty)
                    SetUnInteractableProperty(btn);
            }
        }

        public void HoverSelectedButton(Button button)
        {
            if (MainSelectionMenuHandler.Instance.pressed) return;
            selectedIndex = buttonList.IndexOf(button);
            InitialSelection();
        }
        public bool ReturnListCount() => buttonList.Count > 0;
        //return the boolean values if next button is activated or not
        public bool ReturnActivated()
        {
            if (selectedIndex < 0 || selectedIndex >= buttonList.Count) return false;
            if (!buttonList[selectedIndex].IsActive()) return false;
            if (!buttonList[selectedIndex].IsInteractable()) return false;
            return true;

        }

        #region Button_Pressed Selection And Key's Control
        void ButtonPressedProperty()
        {
            Debug.Log("Selectd Index: " + selectedIndex);
            selectedIndex = buttonList.IndexOf(clickedButton);
            InitialSelection();
            PressedComponent();
        }

        //setting intial property of selected button for changing it after selection over, (for multiple colored/property in the system)
        void InitialPropertySetup(Button btn)
        {
            initialProperty.buttonColor = btn.image.color;
            initialProperty.sprite = btn.image.sprite ?? null;
            initialProperty.textColor = btn.GetComponentInChildren<TMP_Text>().color;
        }

        //selecting property changes for selected color
        void HighlightSetup(Button btn)
        {
            btn.GetComponent<SelectedButtonMethods>()?.Selection();
            if (!useSelectionProperty) return;
            ButtonSetup(btn, selectionProperty);
        }

        // change the property to initial
        void IdleProperty(Button btn)
        {
            ButtonSetup(btn, initialProperty);
            btn.GetComponent<SelectedButtonMethods>()?.SetIdle();
        }

        // pressed component setup when pressed
        public void PressedComponent()
        {
            if (!usePressedProperty) return;
            ButtonSetup(selectedButton, pressedProperty);
            Invoke("DePressed", 0.3f);
            MainSelectionMenuHandler.Instance.pressed = true;
        }

        // changing the system into selection setup after pressed with delay
        void DePressed()
        {
            HighlightSetup(selectedButton);
            MainSelectionMenuHandler.Instance.pressed = false;
            selectedButton.GetComponent<SelectedButtonMethods>()?.Click();
        }
        //changing the property of a selected button
        void ButtonSetup(Button btn,CustomSelectionProperty selectionProperty)
        {
            btn.image.color = selectionProperty.buttonColor;
            if (selectionProperty.sprite !=null)
                btn.image.sprite = selectionProperty.sprite;
            btn.GetComponentInChildren<TMP_Text>().color = selectionProperty.textColor;

        }
        public void SetInteractableProperty(Button btn)
        {
            ButtonSetup(btn, initialProperty);
        }
        public void SetUnInteractableProperty(Button btn)
        {
            ButtonSetup(btn, disableProperty);
        }
        public void InitialSelection()
        {
            while (!ReturnActivated())
            {
                selectedIndex++;
                if (selectedIndex >= buttonList.Count) selectedIndex = 0;
                if (selectedIndex == currentBtnSelectedIndex) return;
            }
            currentBtnSelectedIndex = selectedIndex;
            if (selectedButton != null) IdleProperty(selectedButton);
            selectedButton = buttonList[selectedIndex];
            InitialPropertySetup(selectedButton);
            HighlightSetup(selectedButton);
        }
        // changing the next button
        public void NextSelection()
        {
            selectedIndex++;
            while (!ReturnActivated())
            {
                selectedIndex++;
                if (selectedIndex >= buttonList.Count) selectedIndex = 0;
                if (selectedIndex == currentBtnSelectedIndex) return;
            }
            currentBtnSelectedIndex = selectedIndex;
            if (selectedButton != null) IdleProperty(selectedButton);
            selectedButton = buttonList[selectedIndex];
            InitialPropertySetup(selectedButton);
            HighlightSetup(selectedButton);
        }

        // changing the previous button
        public void PreviousSelection()
        {
            selectedIndex--;
            while (!ReturnActivated())
            {
                selectedIndex--;
                if (selectedIndex < 0) selectedIndex = buttonList.Count - 1;
                if (selectedIndex == currentBtnSelectedIndex) return;
            }
            currentBtnSelectedIndex = selectedIndex;
            if (selectedButton != null) IdleProperty(selectedButton);
            selectedButton = buttonList[selectedIndex];
            InitialPropertySetup(selectedButton);
            HighlightSetup(selectedButton);
        }

        //public void ClickedSelection()
        //{
        //    if (selectedButton != null) SetInitialProperty(selectedButton);

        //    InitialPropertySetup(selectedButton);
        //    SelectionSetup(selectedButton);
        //}
        #endregion
    }
}
