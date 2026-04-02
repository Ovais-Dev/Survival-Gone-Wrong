using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace MenuSystem {
    [System.Serializable]
    public enum MenuState { Menu, Option, Setting, About, Credit, LevelSelector, GamePause, GameOver, Exit, None  }// check below comment
    // also update the string menu state dictionary
    [System.Serializable]
    class MenuSelectionEvent
    {
        public MenuState menuState;
        public ButtonSelectionController btnSelectionController;
        public bool whenSelected;
        public UnityEvent whenSelectedEvent;
        public bool whenDeselected;
        public UnityEvent whenDeselectedEvent;
    }

    public class MainSelectionMenuHandler : MonoBehaviour
    {
        public static MainSelectionMenuHandler Instance;
        [SerializeField] MenuSelectionEvent[] menuSystem;
        ButtonSelectionController selectionEventUnit;


        public KeyCode[] upMoveKey = { KeyCode.UpArrow, KeyCode.W };
        public KeyCode[] downMoveKey = { KeyCode.DownArrow, KeyCode.S };
        public KeyCode[] pressedKey = { KeyCode.Space, KeyCode.Return };

        [Header("Audio Clips")]
        public AudioClip selectClip;
        public AudioClip pressedClip;
        public float defaultVolume = 0.5f;

        public bool pressed = false; // delay the change of selection of other button before full animation of pressed already happen

        Dictionary<string, MenuState> parsedMenuState = new Dictionary<string, MenuState>
        {
            {"Menu", MenuState.Menu },
            {"Option", MenuState.Option},
            {"Settng", MenuState.Setting },
            {"About", MenuState.About },
            {"Credit", MenuState.Credit },
            {"LevelSelector", MenuState.LevelSelector },
            {"GamePause", MenuState.GamePause },
            {"GameOver", MenuState.GameOver },
            {"Exit", MenuState.Exit },
            {"None", MenuState.None }
        };

        //int currentBtnSelectedIndex = 0;

        private Stack<int> stackedMenusIndex = new Stack<int>();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SelectMenu(MenuState.Menu);
            Cursor.visible = true;
        }

        private void Update()
        {
            // input for next selection
            foreach (KeyCode key in upMoveKey)
            {
                if (!selectionEventUnit.ReturnListCount()) return;
                if (Input.GetKeyDown(key) && !pressed)
                {
                    selectionEventUnit.PreviousSelection();
                   SoundManager.Instance.PlayClip(selectClip, defaultVolume);
                }
            }
            // input for previous selection
            foreach (KeyCode key in downMoveKey)
            {
                if (!selectionEventUnit.ReturnListCount()) return;
                if (Input.GetKeyDown(key) && !pressed)
                {
                    selectionEventUnit.NextSelection();
                    SoundManager.Instance.PlayClip(selectClip, defaultVolume);
                }
            }
            // input for pressd values
            foreach (KeyCode key in pressedKey)
            {
                if (Input.GetKeyDown(key) && !pressed)
                {
                    selectionEventUnit.PressedComponent();
                    SoundManager.Instance.PlayClip(pressedClip, defaultVolume);
                }
            }

            // escape to back down
            if (Input.GetKeyDown(KeyCode.Escape) && !pressed) Back();
        }

        public void SelectMenu(MenuState menuState)
        {
            foreach (var buttonSystem in menuSystem)
            {
                if (menuState == buttonSystem.menuState)
                {
                    selectionEventUnit = buttonSystem.btnSelectionController;
                    if (buttonSystem.whenSelected)
                    {
                        buttonSystem.whenSelectedEvent?.Invoke();
                    }
                }
                else
                {
                    if (buttonSystem.whenDeselected)
                    {
                        buttonSystem.whenDeselectedEvent?.Invoke();
                    }
                }
            }
            stackedMenusIndex.Push((int)menuState);
        }
        public void SelectMenu(string menuState)
        {
            if (parsedMenuState.TryGetValue(menuState, out MenuState state))
            {
                SelectMenu(state);
            }
        }
        public void Back()
        {
            int peekValue = stackedMenusIndex.Peek();
            if (peekValue == 0) return;
            stackedMenusIndex.Pop();
            SelectMenu((MenuState)stackedMenusIndex.Peek());

            // de pressing
            Invoke("DePressed", 0.3f);
            pressed = true;
        }

        // changing the system into selection setup after pressed with delay
        void DePressed()
        {
            pressed = false;
        }
    }
}
