using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;
[System.Serializable]
public enum SoundType { Move, Collect, Reached, Defuse, Blast,};
[System.Serializable]
class Sounds
{
    public SoundType clipName;
    public AudioClip soundClip;
    [Range(0,1)]public Vector2 pitchVariation;
    [Range(0, 1)] public Vector2 volumeVariation;
}
[System.Serializable]
class SoundSources
{
    public AudioSource s_source;
}
public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindFirstObjectByType<SoundManager>();
            }
            return _instance;
        }
    }

    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
   // [SerializeField] Vector2 minMaxPitchVariation;
    //[SerializeField] Vector2 volumeRange = new Vector2(0.5f, 1f);
    float initialPitch;
    //[Header("Volume Controls (Mixer)")]
    //[Range(-10, 10)] [SerializeField] float maxVolume;
    //[SerializeField] AudioMixer musicMixer;
    //float initialMainVolume;
    //[SerializeField] AudioMixer sfxMixer;

    //[Header("UI Controls")]
    //public Slider mainVolumeSlider;
    //public Slider sfxVolumeSlider;

    [Header("Sound Clips (with name)")]
    [SerializeField] List<Sounds> sounds;
    Dictionary<SoundType, Sounds> clipDict = new Dictionary<SoundType, Sounds>();
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        
        
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach(var s in sounds)
        {
            clipDict.Add(s.clipName, s);
        }
        PlayMusic();
        initialPitch = sfxSource.pitch;
    }

    // Update is called once per frame
    void Update()
    {
       

    }

   
    #region Audio Control
    public void PlayMusic()
    {
        if(musicSource!=null)
        musicSource.Play();
    }
    public void StopMusic()
    {
        musicSource.Stop();
    }
    public void PlayClip(SoundType clipName, bool pitchVar = false ,bool volumeVar = false)
    {
        var clip = clipDict[clipName];
        float randRange = pitchVar ? Random.Range(clip.pitchVariation.x, clip.pitchVariation.y) : 0;
        float volume = volumeVar ? Random.Range(clip.volumeVariation.x, clip.volumeVariation.y) : 1;
        sfxSource.pitch = initialPitch + randRange;
        sfxSource.volume = volume;
        sfxSource.PlayOneShot(clip.soundClip);
    }
    public void PlayClip(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    #endregion

    #region Audio Mixer Controls
    
    
    #endregion
}
