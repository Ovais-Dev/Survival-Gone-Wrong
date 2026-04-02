using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LevelManager : MonoBehaviour
{
    private event Action<string> NextLevelAction;
    private event Action<string> RestartLevelAction;

    [Header("Next Level Setup")]
    [SerializeField] private string nextScene;

    [Header("Level Settings")]
    [SerializeField] private int currentLevelIndex; // 👈 SET THIS PER SCENE IN INSPECTOR

    private const string GAME_PROGRESS_KEY = "GameProgress";

    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>();
            }
            return _instance;
        }
    }

    void Start()
    {
        NextLevelAction += LevelChange;
        RestartLevelAction += LevelChange;
    }

    // -------------------------------
    // 🔹 LEVEL LOADING
    // -------------------------------
    public void LevelChange(string scene)
    {
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene(scene);
    }

    public void GoToNextLevel(float loadDuration)
    {
        Invoke(nameof(NextLevelMethod), loadDuration);
    }

    public void GoToNextLevel()
    {
        NextLevelMethod();
    }

    public void RestartLevel(float loadDuration)
    {
        Invoke(nameof(CurrentLevelMethod), loadDuration);
    }

    private void CurrentLevelMethod()
    {
        string currentLevel = SceneManager.GetActiveScene().name;
        RestartLevelAction?.Invoke(currentLevel);
        RestartLevelAction -= LevelChange;
    }

    private void NextLevelMethod()
    {
        // 🔥 SAVE PROGRESS BEFORE LOADING NEXT LEVEL
        SaveProgress();

        NextLevelAction?.Invoke(nextScene);
        NextLevelAction -= LevelChange;
        RestartLevelAction -= LevelChange;
    }

    // -------------------------------
    // 🔹 SAVE SYSTEM
    // -------------------------------
    private void SaveProgress()
    {
        int savedProgress = PlayerPrefs.GetInt(GAME_PROGRESS_KEY, 0);

        // Only update if player reached further level
        if (currentLevelIndex> savedProgress)
        {
            PlayerPrefs.SetInt(GAME_PROGRESS_KEY, currentLevelIndex);
            PlayerPrefs.Save();
        }
    }

    // -------------------------------
    // 🔹 LOAD GAME (IMPORTANT)
    // -------------------------------
    public void LoadGame()
    {
        int progress = PlayerPrefs.GetInt(GAME_PROGRESS_KEY, 0);

        if (progress <= 0)
        {
            Debug.Log("No progress found");
            return;
        }

        // 👇 Convert index to scene name
        string sceneToLoad = "Level" + progress;

        SceneManager.LoadScene(sceneToLoad);
    }

    // -------------------------------
    public void QuitGame()
    {
        Application.Quit();
    }
}