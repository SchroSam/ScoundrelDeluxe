using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayMenu : MonoBehaviour
{
    public Image tutorialSlide;
    public List<Sprite> tutorialSlides;
    public Button forwardButton;
    public Button backwardButton;
    private int index = 0;

    public void Awake()
    {
        tutorialSlide.sprite = tutorialSlides[0];
    }

    public void ReturnToStart()
    {
        GetComponent<Canvas>().enabled = false;
        GameObject.Find("StartCanvas").GetComponent<Canvas>().enabled = true;
        //GetComponent<AudioSource>().Stop();
        AudioPlayer.instance.StopMusic();
    }

    public void NextSlide()
    {
        tutorialSlide.sprite = tutorialSlides[++index];
        
        if(index >= tutorialSlides.Count)
            forwardButton.interactable = false;

        backwardButton.interactable = true;
    }

    public void PreviousSlide()
    {
        tutorialSlide.sprite = tutorialSlides[--index];

        if(index <= 0)
            backwardButton.interactable = false;

        forwardButton.interactable = true;
    }

}
