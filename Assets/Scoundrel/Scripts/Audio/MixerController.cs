using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer masterMixer;
    private float savedMusicVol = 0f;
    private float savedSfxVol = 0f;
    private float savedMasterVol = 0f; // in case of mute for master vol
    public void SetMasterVolume(float volume)
    {
        savedMasterVol = volume;
        masterMixer.SetFloat("MasterVol", volume);
    }
    public void SetMusicVolume(float volume)
    {
        savedMusicVol = volume;
        //masterMixer.SetFloat("MusicVol", Mathf.Log10(volume) * 113.6f); // For use with a slider from 0.5 to 1.5
        masterMixer.SetFloat("MusicVol", volume);
    }

    public void SetSFXVolume(float volume)
    {
        savedSfxVol = volume;
        masterMixer.SetFloat("SfxVol", volume);
    }

    public void ToggleMusicMute(bool isMuted)
    {
        if (isMuted)
        {
            masterMixer.GetFloat("MusicVol", out savedMusicVol);
            masterMixer.SetFloat("MusicVol", -80f); // Mute
        }
        else
        {
            masterMixer.SetFloat("MusicVolume", savedMusicVol);
        }
    }

    public void ToggleSFXMute(bool isMuted)
    {
        if (isMuted)
        {
            masterMixer.GetFloat("SfxVolume", out savedSfxVol);
            masterMixer.SetFloat("SfxVolume", -80f); // Mute
        }
        else
        {
            masterMixer.SetFloat("SfxVolume", savedSfxVol);
        }
    }
}
