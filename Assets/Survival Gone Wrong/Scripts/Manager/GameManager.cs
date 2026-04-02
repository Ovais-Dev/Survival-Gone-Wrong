using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
//public enum GameState { Playihng, Paused};
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }
    public GameState gameState = GameState.Playing;

    [Header("Pause")]
    public KeyCode pauseKeyCode = KeyCode.Escape;
    public bool pauseTime = true;
    bool havePaused = false;

    [System.Serializable]
    public enum GameState { Playing, Paused, Finished};

    
    private void Awake()
    {
        //if (EffectManager.Instance == null)
        //{
        //    GameObject effectManager = Resources.Load<GameObject>("EffectManager").gameObject;
        //    Instantiate(effectManager);
        //}
        //if (ItemHolder.Instance == null)
        //{
        //    GameObject itemSystemManager = Resources.Load<GameObject>("ItemManager").gameObject;
        //    Instantiate(itemSystemManager);
        //}
        if(SoundManager.Instance == null)
        {
            GameObject soundManager = Resources.Load<GameObject>("SoundManager").gameObject;
            Instantiate(soundManager);
        }
        if (WeaponDatabase.Instance == null)
        {
            GameObject soundManager = Resources.Load<GameObject>("WeaponDatabase").gameObject;
            Instantiate(soundManager);
        }
        if (WeaponInventory.Instance == null)
        {
            GameObject soundManager = Resources.Load<GameObject>("WeaponInventory").gameObject;
            Instantiate(soundManager);
        }
        //if (MenuSystem.MainSelectionMenuHandler.Instance == null)
        //{
        //    GameObject menu = Resources.Load<GameObject>("MainMenuCanvas").gameObject;
        //    Instantiate(menu);
        //}

    }
    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Playing;

        //MenuSystem.MainSelectionMenuHandler.Instance.SelectMenu(MenuSystem.MenuState.None);
        //MenuSystem.MainSelectionMenuHandler.Instance.DisableEscapeKeyFunction();

    }
    //private void OnEnable()
    //{
    //    handler = LevelButtonEventHandler.Instance;
    //    if(handler)
    //    handler.ResumeAction += Resume;
    //}
    //private void OnDisable()
    //{
    //    if(handler)
    //    handler.ResumeAction -= Resume;
    //}
    // Update is called once per frame

    void Update()
    {

        if (Input.GetKeyDown(pauseKeyCode) && gameState != GameState.Finished)
        {
            if (havePaused)
                Resume();
            else
                Pause();
        }

    }
    public void Pause()
    {
        havePaused = true;
        gameState = GameState.Paused;

        if (pauseTime)
            Time.timeScale = 0f;

       // MenuSystem.MainSelectionMenuHandler.Instance.SelectMenu(MenuSystem.MenuState.GamePause);
    }

    public void Resume()
    {
        havePaused = false;
        gameState = GameState.Playing;

        if (pauseTime)
            Time.timeScale = 1f;

        //MenuSystem.MainSelectionMenuHandler.Instance.SelectMenu(MenuSystem.MenuState.None);
    }
    public void SetGameState(GameState state)
    {
        gameState = state;
    }
    public void SetGameState(int i)
    {
        gameState = (GameState)i;
    }
    public bool GamePlaying() => gameState == GameState.Playing;
}
