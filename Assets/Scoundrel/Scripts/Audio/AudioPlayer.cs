using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0f, 3f)]
    public float pitch = 1f;

    public Sound(AudioClip clip)
    {
        this.clip = clip;
        volume = 1f;
        pitch = 1f;
    }
}


public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer instance;

    [Header("Music")]
    public AudioSource musicSource;
    private List<Sound> musicClips;

    [Header("SFX")]
    public AudioSource sfxSource;
    private List<Sound> sfxClips;

    private void Awake()
    {
        if (instance == null)
        {
            musicClips = new List<Sound>
            {
                new Sound(Resources.Load<AudioClip>("Audio/Music/Bard-Instructions")),
                new Sound(Resources.Load<AudioClip>("Audio/Music/Final Fantasy 7 Boss battle"))
            };

            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(string name)
    {

        //Debug.Log(musicClips[0].clip.name);
        Sound sound = musicClips.Find(s => s.clip.name == name);
        if (sound != null)
        {
            musicSource.Stop();
            musicSource.clip = sound.clip;
            musicSource.volume = sound.volume;
            musicSource.pitch = sound.pitch;
            musicSource.loop = true;
            musicSource.Play();
            // if (musicSource.loop == true)
            // {
            //     Debug.LogWarning("Looping");
            // }
            // else
            // { 
            //     Debug.LogWarning("Not Looping"); 
            // }
        }
        else
        {
            Debug.LogWarning("Music not found: " + name);
        }
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void PlaySFX(string name)
    {
        Sound sound = musicClips.Find(s => s.clip.name == name);
        if (sound != null)
        {
            sfxSource.PlayOneShot(sound.clip, sound.volume);
        }
        else
        {
            Debug.LogWarning("SFX not found: " + name);
        }
    }

    public void StopSFX()
    {
        if (sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }
    }
}