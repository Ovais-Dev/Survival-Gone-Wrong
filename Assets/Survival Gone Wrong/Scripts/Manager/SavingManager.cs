using UnityEngine;
using UnityEngine.UI;

public class SavingManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button loadGameButton;

    private const string GAME_PROGRESS_KEY = "GameProgress";

    void Start()
    {
        Invoke(nameof(CheckGameProgress), 0.01f);

    }

    public void CheckGameProgress()
    {
        int progress = PlayerPrefs.GetInt(GAME_PROGRESS_KEY, 0);
        Debug.Log("Progress" + progress);
        if (progress <= 0)
        {
            loadGameButton.GetComponent<MenuSystem.SelectedButtonMethods>().MakeInteractable(false);
        }
        else
        {
            loadGameButton.GetComponent<MenuSystem.SelectedButtonMethods>().MakeInteractable(true);
        }
    }

    // Call this when player completes a level
    public void SaveProgress(int levelIndex)
    {
        PlayerPrefs.SetInt(GAME_PROGRESS_KEY, levelIndex);
        PlayerPrefs.Save();
    }

    // Optional: Reset progress (for testing)
    public void ResetProgress()
    {
        PlayerPrefs.SetInt(GAME_PROGRESS_KEY, 0);
        PlayerPrefs.Save();
        CheckGameProgress();
    }
}