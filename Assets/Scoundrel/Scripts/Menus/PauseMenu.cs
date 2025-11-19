using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private MixerController mixer;
    private Slider musicVolumeSlider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mixer = FindFirstObjectByType<MixerController>();
        musicVolumeSlider = GameObject.Find("VolumeSlider").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
        }
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }

    public void ChangeMusicVolume()
    {
        mixer.SetMusicVolume(musicVolumeSlider.value);
    }
}
