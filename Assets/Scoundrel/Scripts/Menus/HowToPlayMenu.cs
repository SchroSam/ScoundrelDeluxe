using UnityEngine;

public class HowToPlayMenu : MonoBehaviour
{
    public void ReturnToStart()
    {
        GetComponent<Canvas>().enabled = false;
        GameObject.Find("StartCanvas").GetComponent<Canvas>().enabled = true;
        //GetComponent<AudioSource>().Stop();
        AudioPlayer.instance.StopMusic();
    }

}
