using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
[System.Serializable]
public enum SoundType { Move, Collect, Reached, Defuse, Blast,};
[System.Serializable]
class Sounds
{
    public SoundType clipName;
    public AudioClip soundClip;
    public Vector2 pitchVariation;
    public Vector2 volumeVariation;
}
[System.Serializable]
class SoundSources
{
    public AudioSource s_source;
}

public struct SoundEvent
{
    public Vector3 position;
    public float intensity;
    //public float radius; // player pos
    public SoundEvent(Vector3 pos, float i/*, float r*/)
    {
        position = pos;
        intensity = i;
       // radius = r;
    }
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
    //void Start()
    //{
    //    foreach(var s in sounds)
    //    {
    //        clipDict.Add(s.clipName, s);
    //    }
    //    PlayMusic();
    //    initialPitch = sfxSource.pitch;
    //}

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
        float pitchMin = Mathf.Min(clip.pitchVariation.x, clip.pitchVariation.y);
        float pitchMax = Mathf.Max(clip.pitchVariation.x, clip.pitchVariation.y);

        float volumeMin = Mathf.Min(clip.volumeVariation.x, clip.volumeVariation.y);
        float volumeMax = Mathf.Max(clip.volumeVariation.x, clip.volumeVariation.y);

        float randRange = pitchVar ? UnityEngine.Random.Range(pitchMin, pitchMax) : 0f;
        float volume = volumeVar ? UnityEngine.Random.Range(volumeMin, volumeMax) : 1f;

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

    #region Emitting Sound

    public static Action<SoundEvent> OnSoundEmitted;

    public static void EmitSound(Vector3 position, float intensity/*, float radius*/)
    {
        OnSoundEmitted?.Invoke(new SoundEvent(position, intensity/*,radius*/));
    }

    #endregion
}
