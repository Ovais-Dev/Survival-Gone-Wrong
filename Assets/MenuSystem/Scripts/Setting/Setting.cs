using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Collections.Generic;

public class Setting : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer mainMixer; // Drag your AudioMixer here
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle muteToggle;

    [Header("Video Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    //[SerializeField] private TMP_Dropdown qualityDropdown;

    private Resolution[] resolutions;

    private void Start()
    {
        InitializeResolutions();
        LoadSettings();
    }

    #region Video Settings
    private void InitializeResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResIndex", resolutionIndex);
    }

    // public void SetQuality(int qualityIndex)
    // {
    //     QualitySettings.SetQualityLevel(qualityIndex);
    //     PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    // }
    #endregion

    #region Audio Settings
    public void SetMasterVolume(float volume)
    {
        // Mixers use Logarithmic scale, -80 is silent, 0 is full
        mainMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVol", volume);
    }

    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVol", volume);
    }

    public void SetMute(bool isMuted)
    {
        AudioListener.pause = isMuted;
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
    }
    #endregion

    private void LoadSettings()
    {
        // Load Audio
        float master = PlayerPrefs.GetFloat("MasterVol", 0.75f);
        masterVolumeSlider.value = master;
        SetMasterVolume(master);

        float sfx = PlayerPrefs.GetFloat("SFXVol", 0.75f);
        sfxVolumeSlider.value = sfx;
        SetSFXVolume(sfx);

        bool muted = PlayerPrefs.GetInt("Muted", 0) == 1;
        muteToggle.isOn = muted;
        SetMute(muted);

        // Load Quality
        // int quality = PlayerPrefs.GetInt("QualityLevel", 3);
        // qualityDropdown.value = quality;
        // SetQuality(quality);
    }
}