using UnityEngine;

public class HowToPlayMenu : MonoBehaviour
{
    public void ReturnToStart()
    {
        GetComponent<Canvas>().enabled = false;
        GameObject.Find("StartCanvas").GetComponent<Canvas>().enabled = true;
        GetComponent<AudioSource>().Stop();
    }

    public void Classes()
    {
        GetComponent<Canvas>().enabled = false;
        GameObject.Find("ArchCanvas").GetComponent<Canvas>().enabled = true;
    }

    public void ReturnToBasics()
    {
        GameObject.Find("ArchCanvas").GetComponent<Canvas>().enabled = false;
        GetComponent<Canvas>().enabled = false;
    }

}
