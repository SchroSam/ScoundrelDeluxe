using UnityEngine;

public enum Archetype{Knight, Wizard, Elf, Warrior}

public class MainMenu : MonoBehaviour
{
    private GameObject playerManager;
    void Start()
    {
        playerManager = FindFirstObjectByType<ScoundrelGame>().gameObject;
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
        gameObject.SetActive(false);
        GameObject.Find("GameCanvas").GetComponent<Canvas>().enabled = true;
        GameObject.Find("GameCanvas").GetComponent<AudioSource>().Play();
        

        // assign player archetype
        playerManager.GetComponent<ScoundrelGame>().ChooseArch(p);
    }

}
