using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum Archetype{Knight, Wizard, Elf, Warrior}

public class MainMenu : MonoBehaviour
{
    private GameObject playerManager;
    private GameObject newGameButton;
    private GameObject fade;
    private bool fadeStarted = false;
    private bool fadeIn = false;
    public float maxAlpha = 0.5f;
    private Color fadeColor;
    public float fadeSpeed = 0.3f;

    void Start()
    {
        newGameButton = GameObject.Find("NewGameButton");
        playerManager = FindFirstObjectByType<ScoundrelGame>().gameObject;
        fade = GameObject.Find("Fade");
        fadeColor = fade.GetComponent<Image>().color;
    }

    void FixedUpdate()
    {
        if(fadeStarted && fadeIn)
        {
            fade.GetComponent<Image>().color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, fade.GetComponent<Image>().color.a + fadeSpeed);

            if(fade.GetComponent<Image>().color.a >= maxAlpha)
            {
                fadeStarted = false;
                fade.GetComponent<Image>().raycastTarget = true;
            }
        }

        if(fadeStarted && !fadeIn)
        {
            //fade.GetComponent<Image>().color.Equals(Vector4.MoveTowards(new Vector4(fadeColor.r, fadeColor.g, fadeColor.b, fade.GetComponent<Image>().color.a), new Vector4(fadeColor.r, fadeColor.g, fadeColor.b, 0), fadeSpeed));
            fade.GetComponent<Image>().color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, fade.GetComponent<Image>().color.a - fadeSpeed);

            if(fade.GetComponent<Image>().color.a <= 0)
            {
                fadeStarted = false;
                fade.GetComponent<Image>().raycastTarget = false;
            }
        }
    }

    public void ChangeFade()
    {
        if (!fadeStarted)
        {
            fadeIn = !fadeIn;
            fadeStarted = true;
        }
    }

    public void HowToPlay()
    {
        GetComponent<Canvas>().enabled = false;
        GameObject.Find("HowToPlayCanvas").GetComponent<Canvas>().enabled = true;
        AudioPlayer.instance.PlayMusic("Bard-Instructions");
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
        AudioPlayer.instance.PlayMusic("Final Fantasy 7 Boss battle");
        

        // assign player archetype
        playerManager.GetComponent<ScoundrelGame>().ChooseArch(p);
        newGameButton.GetComponent<Button>().interactable = true;
    }



}
