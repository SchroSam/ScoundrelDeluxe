using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoundrelGame : MonoBehaviour
{
    public int health = 20;
    public int maxHealth = 20;
    public int weaponVal = 0;
    public int weaponDamage = 14;
    public bool usingWeapon = false;
    public List<Card> monstersSlainWithWeapon;
    private Deck deck;
    private TMP_Text healthText;
    private TMP_Text damageText;
    private GameObject weaponButton;
    private GameObject skipButton;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deck = FindFirstObjectByType<Deck>();
        healthText = GameObject.Find("Health").GetComponent<TMP_Text>();
        damageText = GameObject.Find("Damage").GetComponent<TMP_Text>();
        weaponButton = GameObject.Find("Weapon");
        skipButton = GameObject.Find("SkipButton");
        
        deck.CreateNew52();
        NewRoom();
    }

    public void CardSelected(CardOnObj cardO)
    {
        // healing potions
        if(cardO.card.suit == Suit.Hearts)
        {
            health += cardO.card.value;

            if(health > maxHealth)
                health = maxHealth;

            healthText.text = $"Health: {health}/{maxHealth}";
        }

        // weapons
        else if (cardO.card.suit == Suit.Diamonds)
        {
            weaponVal = cardO.card.value;
            weaponDamage = 14;

            monstersSlainWithWeapon = new List<Card>();

            weaponButton.transform.GetChild(0).GetComponent<TMP_Text>().text = $"D {weaponVal}";
            weaponButton.GetComponent<Button>().interactable = true;

            damageText.text = $"{14}";
        }

        // monster fighting
        else
        {
            FightMonster(cardO.card);
        }




        // removing card once done with it
        deck.slotsUsed[cardO.card.slotIndex] = false;

        Destroy(cardO.gameObject);

        // if we're not dead, procede as normal
        if(!(health <= 0)){
            // determine if the room has been cleared
            int numSpotsFilled = 4;

            foreach(bool spotFilled in deck.slotsUsed)
            {
                if(spotFilled == false)
                    numSpotsFilled--;
            }

            // win!
            if(deck.cards.Count + numSpotsFilled < 4)
            {
                winRound();
            }

            else if(numSpotsFilled <= 1)
            {
                skipButton.GetComponent<Button>().interactable = true;
                NewRoom();
            }
        }
        // disable all buttons on death
        else
        {
            Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

            foreach (Button button in buttons)
            {
                button.interactable = false;
            }
        }
    }

    public void NewRoom()
    {
        deck.FillSpots();
    }

    public void SkipRoom()
    {
        skipButton.GetComponent<Button>().interactable = false;
        GameObject[] tucks = GameObject.FindGameObjectsWithTag("Card");

        //I HAVE NO IDEA WHAT'S MESSING UP SKIP ON FIRST ROUND
        deck.done = false;

        deck.TuckCards();
        Debug.Log($"Cards in deck after tuck: {deck.cards.Count}");

        while(!deck.done);

        

        NewRoom();
    }

    public void SelectWeaponToggle()
    {
        usingWeapon = !usingWeapon;

        if (usingWeapon)
            weaponButton.GetComponent<Image>().color = Color.green;
        else
            weaponButton.GetComponent<Image>().color = Color.grey;
    }

    void FightMonster(Card card)
    {
        if(!usingWeapon || weaponDamage <= card.value)
            health -= card.value;
        else if (weaponDamage > card.value)
        {
            if(card.value - weaponVal > 0)
                health -= card.value - weaponVal;

            weaponDamage = card.value;
            damageText.text = $"{weaponDamage - 1}";

            monstersSlainWithWeapon.Add(card);
        }

        healthText.text = $"Health: {health}/{maxHealth}";
    }

    void winRound()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

}
