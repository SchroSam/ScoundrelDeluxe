using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoundrelGame : MonoBehaviour
{
    public int health = 20;
    public int maxHealth = 20;
    public int weaponVal = 0;
    public int weaponDamage = 14;
    public bool usingWeapon = false;
    public int wizardMana = 0;
    public int wizardMaxMana = 15;
    public Archetype player;
    private List<Card> monstersSlainWithWeapon;
    private Deck deck;
    private TMP_Text healthText;
    private TMP_Text damageText;
    private GameObject weaponButton;
    private GameObject skipButton;
    private string playerName = "Scott";
    private bool noPotThisRoom = true;
    private bool weaponUsed = false;
    private Color precisionColor;
    private GameObject elfPrecision;
    private GameObject rageButton;
    
    void StartGame()
    {
        deck = FindFirstObjectByType<Deck>();

        GameObject.Find("Name").GetComponent<TMP_Text>().text = playerName;

        healthText = GameObject.Find("Health").GetComponent<TMP_Text>();
        healthText.text = $"Health: {health}/{maxHealth}";

        damageText = GameObject.Find("Damage").GetComponent<TMP_Text>();
        weaponButton = GameObject.Find("Weapon");

        skipButton = GameObject.Find("SkipButton");

        monstersSlainWithWeapon = new List<Card>();
        
        deck.CreateNew52();
        NewRoom();
    }

    public void ChooseArch(Archetype p)
    {

        player = p;

        switch (player)
        {
            // Knight is more tanky
            case Archetype.Knight:
                maxHealth += 5;
                health += 5;

                playerName = "Reynault - Knight";
                break;

            // For the wizard, weapons are not swords, they are mana potions.
            // Mana potions add to your mana pool, and when you fight a monster
            // with magic (using your mana pool) it just subtracts the two directly
            // with overflow of course going to the player as normal
            // gaining more mana is limited by a max mana of 15 by default
            case Archetype.Wizard:

                playerName = "Merlin - Wizard";
                break;

            // Elf is less healthy, but the first time they hit an enemy with a given weapon, the weapon takes no damage
            case Archetype.Elf:
                maxHealth -= 5;
                health -= 5;

                if(elfPrecision == null)
                    elfPrecision = GameObject.Find("Precision");

                elfPrecision.GetComponent<Image>().enabled = true;
                precisionColor = elfPrecision.GetComponent<Image>().color;
                elfPrecision.transform.GetChild(0).GetComponent<TMP_Text>().enabled = true;

                playerName = "Questor - Elf";
                break;

            // Warrior can once per game go into a rage and destroy everything in the room without taking any damage
            case Archetype.Warrior:

                if(rageButton == null)
                    rageButton = GameObject.Find("Rage");
                    
                rageButton.GetComponent<Button>().enabled = true;
                rageButton.GetComponent<Image>().enabled = true;
                rageButton.transform.GetChild(0).GetComponent<TMP_Text>().enabled = true;

                playerName = "Dwalin - Warrior";
                break;

        }

        StartGame();
    }

    void DisableArchSpec()
    {
        // Warrior
        if(player == Archetype.Warrior){
            GameObject rageButton = GameObject.Find("Rage");
            rageButton.GetComponent<Button>().enabled = false;
            rageButton.GetComponent<Image>().enabled = false;
            rageButton.transform.GetChild(0).GetComponent<TMP_Text>().enabled = false;
        }

        else if(player == Archetype.Elf)
        {
            GameObject elfPrecision = GameObject.Find("Precision");
            // make the image appear active again on cleanup
            elfPrecision.GetComponent<Image>().enabled = false;
            elfPrecision.transform.GetChild(0).GetComponent<TMP_Text>().enabled = false;
        }
    }

    public void CardSelected(CardOnObj cardO)
    {
        // healing potions
        if(cardO.card.suit == Suit.Hearts && noPotThisRoom)
        {
            GameObject[] cardsObj = GameObject.FindGameObjectsWithTag("Card");

            foreach(GameObject obj in cardsObj)
            {
                if(obj.GetComponent<CardOnObj>().card.suit == Suit.Hearts)
                    obj.GetComponent<Image>().color = Color.grey;

            }
            

            noPotThisRoom = false;
            health += cardO.card.value;

            //Debug.Log($"halth > maxHealth is: {health > maxHealth}");
            if(health > maxHealth)
                health = maxHealth;

            healthText.text = $"Health: {health}/{maxHealth}";
        }

        // weapons
        else if (cardO.card.suit == Suit.Diamonds)
        {

            if(player == Archetype.Elf)
            {
                elfPrecision.GetComponent<Image>().color = precisionColor;
                weaponUsed = false;
            }

            weaponVal = cardO.card.value;
            weaponDamage = 14;

            monstersSlainWithWeapon = new List<Card>();

            weaponButton.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{Card.valToString(weaponVal)}D";
            weaponButton.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
            weaponButton.GetComponent<Button>().interactable = true;

            damageText.text = $"{14}";
        }

        // monster fighting
        else if (cardO.card.suit == Suit.Clubs || cardO.card.suit == Suit.Spades)
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

            if(numSpotsFilled <= 1)
            {
                skipButton.GetComponent<Button>().interactable = true;
                NewRoom();
            }
        }
        // disable all buttons on death
        else
            PlayerDeath();
    }

    void PlayerDeath()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            if(button.transform.parent.name == "GameCanvas")
                button.interactable = false;
        }
    }

    public void NewRoom()
    {

        int numSpotsFilled = 4;

        foreach(bool spotFilled in deck.slotsUsed)
        {
            if(spotFilled == false)
                numSpotsFilled--;
        }

        // win!
        if(deck.cards.Count + numSpotsFilled < 4)
        {
            WinRound();
        }

        else{
            noPotThisRoom = true;

            GameObject[] cardsObj = GameObject.FindGameObjectsWithTag("Card");

            foreach(GameObject obj in cardsObj)
            {
                if(obj.GetComponent<CardOnObj>().card.suit == Suit.Hearts)
                    obj.GetComponent<Image>().color = Color.white;

            }

            deck.FillSpots();
        }
    }

    public void SkipRoom()
    {
        skipButton.GetComponent<Button>().interactable = false;

        //I HAVE NO IDEA WHAT'S MESSING UP SKIP ON FIRST ROUND
        //deck.done = false;

        deck.TuckCards();
        //Debug.Log($"Cards in deck after tuck: {deck.cards.Count}");

        //while(!deck.done);

        

        NewRoom();
    }

    public void SelectWeaponToggle()
    {

        weaponButton = GameObject.Find("Weapon");
        usingWeapon = !usingWeapon;

        if (usingWeapon)
            weaponButton.GetComponent<Image>().color = Color.green;
        else
            weaponButton.GetComponent<Image>().color = Color.grey;
    }

    void FightMonster(Card card)
    {
        if(!usingWeapon || (weaponDamage <= card.value && weaponDamage != 14))
            health -= card.value;
        else if ((usingWeapon && weaponDamage > card.value) || weaponDamage == 14)
        {
            if(card.value - weaponVal > 0)
                health -= card.value - weaponVal;

            if(player == Archetype.Elf && !weaponUsed)
            {
                weaponUsed = true;
                elfPrecision.GetComponent<Image>().color = precisionColor.WithAlphaMultiplied(0.5f);
            }
            else if(player != Archetype.Elf || weaponUsed)
            {
                weaponDamage = card.value;
            }
            
            if(weaponDamage != 14)
                damageText.text = $"{weaponDamage - 1}";
            else
                damageText.text = $"{weaponDamage}";

            monstersSlainWithWeapon.Add(card);
        }

        healthText.text = $"Health: {health}/{maxHealth}";
    }

    public void Rage()
    {
        int numSpotsFilled = 4;

        foreach(bool spotFilled in deck.slotsUsed)
        {
            if(spotFilled == false)
                numSpotsFilled--;
        }

        if(numSpotsFilled == 4){

            GameObject[] cardsObj = GameObject.FindGameObjectsWithTag("Card");

            foreach(GameObject obj in cardsObj)
            {
                Destroy(obj);
            }

            deck.slotsUsed = new List<bool> {false, false, false, false};

            GameObject.Find("Rage").GetComponent<Button>().interactable = false;

            deck.FillSpots();

        }
    }

    void WinRound()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            if(button.transform.parent.name == "GameCanvas")
                button.interactable = false;
        }
    }

    public void CleanupNewRound()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            button.interactable = true;

            if(button.tag == "Card")
                Destroy(button.gameObject);
        }

        health = 20;
        maxHealth = 20;

        deck.slotsUsed = new List<bool> {false, false, false, false};

        GameObject.Find("GameCanvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("StartCanvas").GetComponent<Canvas>().enabled = true;
        GameObject.Find("PauseCanvas").GetComponent<Canvas>().enabled = false;

        monstersSlainWithWeapon.Clear();

        weaponUsed = false;

        DisableArchSpec();

        damageText.text = "0";
        weaponButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "";

        GameObject.Find("GameCanvas").GetComponent<AudioSource>().Stop();

    }

}
