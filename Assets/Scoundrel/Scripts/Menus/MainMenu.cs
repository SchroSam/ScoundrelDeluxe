using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum Archetype{Knight, Wizard, Elf, Warrior}

public class MainMenu : MonoBehaviour
{
    private GameObject playerManager;
    private GameObject newGameButton;
    private GameObject howToPlayCanvas;

    void Start()
    {
        howToPlayCanvas = GameObject.Find("HowToPlayCanvas");
        newGameButton = GameObject.Find("NewGameButton");
        playerManager = FindFirstObjectByType<ScoundrelGame>().gameObject;
    }

    public void HowToPlay()
    {
        GetComponent<Canvas>().enabled = false;
        howToPlayCanvas.GetComponent<Canvas>().enabled = true;
        howToPlayCanvas.GetComponent<HowToPlayMenu>().ResetSlides();
        AudioPlayer.instance.PlayMusic("08 - Shop");
    }

    public void Knight()
    {
        StartGame(Archetype.Knight);
    }

    public void Wizard()
    {
        StartGame(Archetype.Wizard);
    }

    public void Elf()
    {
        StartGame(Archetype.Elf);
    }

    public void Warrior()
    {
        StartGame(Archetype.Warrior);
    }

    public void StartGame(Archetype p)
    {
        // turn main menu off and game screen on
        GetComponent<Canvas>().enabled = false;
        GameObject.Find("GameCanvas").GetComponent<Canvas>().enabled = true;
        //GameObject.Find("GameCanvas").GetComponent<AudioSource>().Play();
        AudioPlayer.instance.PlayMusic(AudioPlayer.instance.musicClips[Random.Range(1, 5)].clip.name);
        

        // assign player archetype
        playerManager.GetComponent<ScoundrelGame>().ChooseArch(p);
        newGameButton.GetComponent<Button>().interactable = true;
    }



}
