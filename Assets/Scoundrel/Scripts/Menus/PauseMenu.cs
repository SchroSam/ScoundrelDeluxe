using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private AudioSource gameMusic;
    private AudioSource instructionMusic;
    private Slider volumeSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameMusic = GameObject.Find("GameCanvas").GetComponent<AudioSource>();
        instructionMusic = GameObject.Find("HowToPlayCanvas").GetComponent<AudioSource>();
        volumeSlider = GameObject.Find("VolumeSlider").GetComponent<Slider>();
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
        gameMusic.volume = volumeSlider.value;
        instructionMusic.volume = volumeSlider.value;
    }
}
