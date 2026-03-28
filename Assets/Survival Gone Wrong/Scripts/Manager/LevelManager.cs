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
    //[SerializeField] private float loadDuration = 1f;

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
    // Start is called before the first frame update
    void Start()
    {
        NextLevelAction += LevelChange;
        RestartLevelAction += LevelChange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
        NextLevelAction?.Invoke(nextScene);
        NextLevelAction -= LevelChange;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
