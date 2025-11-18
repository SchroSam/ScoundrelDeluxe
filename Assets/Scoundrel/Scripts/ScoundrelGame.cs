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
    public int wizardMana = 0;
    private int wizardMaxMana = 15;
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
    private GameObject spellSlot;
    private TMP_Text winLoseText;
    public string winMessage = "Dungeon Cleared - Success";
    public string loseMessage = "You have fallen";
    void StartGame()
    {
        if(deck == null)
            deck = FindFirstObjectByType<Deck>();

        GameObject.Find("Name").GetComponent<TMP_Text>().text = playerName;

        if(healthText == null)
            healthText = GameObject.Find("Health").GetComponent<TMP_Text>();

        healthText.text = $"Health: {health}/{maxHealth}";

        if(skipButton == null)
            skipButton = GameObject.Find("SkipButton");

        monstersSlainWithWeapon = new List<Card>();

        if(winLoseText == null)
            winLoseText = GameObject.Find("WinLoseText").GetComponent<TMP_Text>();
        
        deck.CreateNew52();
        NewRoom();
    }

    public void ChooseArch(Archetype p)
    {
        damageText = GameObject.Find("Damage").GetComponent<TMP_Text>();
        weaponButton = GameObject.Find("Weapon");

        player = p;

        switch (player)
        {
            // Knight is more tanky - easiest for new players
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
                wizardMana = 7;
                wizardMaxMana = 15;

                if(spellSlot == null)
                    spellSlot = GameObject.Find("SpellSlot");

                spellSlot.GetComponent<Button>().enabled = true;
                spellSlot.GetComponent<Image>().enabled = true;
                spellSlot.transform.GetChild(0).GetComponent<TMP_Text>().enabled = true;
                spellSlot.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{wizardMana}M";

                weaponButton.GetComponent<Button>().enabled = false;
                weaponButton.GetComponent<Image>().enabled = false;
                weaponButton.transform.GetChild(0).GetComponent<TMP_Text>().enabled = false;
                damageText.enabled = false;

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
        if(player == Archetype.Warrior)
        {
            if(rageButton == null)
                rageButton = GameObject.Find("Rage");

            rageButton.GetComponent<Button>().enabled = false;
            rageButton.GetComponent<Image>().enabled = false;
            rageButton.transform.GetChild(0).GetComponent<TMP_Text>().enabled = false;
        }

        else if(player == Archetype.Elf)
        {
            if(elfPrecision == null)
                elfPrecision = GameObject.Find("Precision");

            elfPrecision.GetComponent<Image>().color = precisionColor;
            elfPrecision.GetComponent<Image>().enabled = false;
            elfPrecision.transform.GetChild(0).GetComponent<TMP_Text>().enabled = false;
        }

        else if(player == Archetype.Wizard)
        {
            if(spellSlot == null)
                spellSlot = GameObject.Find("SpellSlot");

            spellSlot.GetComponent<Button>().enabled = false;
            spellSlot.GetComponent<Image>().enabled = false;
            spellSlot.transform.GetChild(0).GetComponent<TMP_Text>().enabled = false;

            weaponButton.GetComponent<Button>().enabled = true;
            weaponButton.GetComponent<Image>().enabled = true;
            weaponButton.transform.GetChild(0).GetComponent<TMP_Text>().enabled = true;

            damageText.enabled = true;
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
            if(player != Archetype.Wizard){

                if(player == Archetype.Elf)
                {
                    elfPrecision.GetComponent<Image>().color = precisionColor;
                    weaponUsed = false;
                }

                weaponVal = cardO.card.value;
                weaponDamage = 14;

                monstersSlainWithWeapon = new List<Card>();

                weaponButton.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{Card.valToString(weaponVal)}D";
                weaponButton.transform.GetChild(1).GetComponent<TMP_Text>().text = $"{Card.valToString(weaponVal)}D";
                weaponButton.GetComponent<Button>().interactable = true;

                damageText.text = $"{14}";
            }
            else // mana for the wizard
            {
                wizardMana += cardO.card.value;

                if(wizardMana > wizardMaxMana)
                    wizardMana = wizardMaxMana;

                spellSlot.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{wizardMana}M";

                // string manaTextColor = "#FFA500";

                // if (UnityEngine.ColorUtility.TryParseHtmlString(manaTextColor, out Color myColor))
                //     spellSlot.transform.GetChild(0).GetComponent<TMP_Text>().color = myColor; // Apply the color to the button

                
                //spellSlot.GetComponent<Button>().interactable = true;
            }
        }

        // monster fighting
        else if (cardO.card.suit == Suit.Clubs || cardO.card.suit == Suit.Spades)
        {
            if(player != Archetype.Wizard)
                FightMonster(cardO.card);
            else
                FightMonsterWizard(cardO.card);
        }




        // removing card once done with it
        deck.slotsUsed[cardO.card.slotIndex] = false;

        Destroy(cardO.gameObject);

        // if we're still alive, procede as normal
        if(health > 0){
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
        if(player != Archetype.Wizard){
            if(weaponButton == null)
                weaponButton = GameObject.Find("Weapon");

            usingWeapon = !usingWeapon;

            if (usingWeapon)
                weaponButton.GetComponent<Image>().color = Color.green;
            else
                weaponButton.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            if(spellSlot == null)
                spellSlot = GameObject.Find("SpellSlot");

            usingWeapon = !usingWeapon;

            if (usingWeapon)
                spellSlot.GetComponent<Image>().color = Color.cyan;
            else
                spellSlot.GetComponent<Image>().color = Color.grey;
        }
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
                elfPrecision.GetComponent<Image>().color = new Color(precisionColor.r, precisionColor.g, precisionColor.b, precisionColor.a/2f);
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

    void FightMonsterWizard(Card card)
    {
        if(usingWeapon)
        {
            int difference = card.value - wizardMana;

            if(difference > 0)
                health -= difference;

            wizardMana -= card.value;

            if(wizardMana < 0)
                wizardMana = 0;

            spellSlot.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{wizardMana}M";
        }
        else
            health -= card.value;

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
        winLoseText.text = winMessage;
        winLoseText.enabled = true;
        
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            if(button.transform.parent.name == "GameCanvas" || button.transform.parent.name == "Deck")
                button.interactable = false;
        }

        GameObject.Find("Progress").GetComponent<Slider>().value = 1f;
    }

    void PlayerDeath()
    {
        winLoseText.text = loseMessage;
        winLoseText.enabled = true;

        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            if(button.transform.parent.name == "GameCanvas" || button.transform.parent.name == "Deck")
                button.interactable = false;
        }
    }

    public void CleanupNewRound()
    {
        if(GameObject.Find("StartCanvas").GetComponent<Canvas>().enabled == false)
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
            winLoseText.enabled = false;

            DisableArchSpec();

            damageText.text = "0";
            weaponButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
            weaponButton.GetComponent<Button>().interactable = false;

            GameObject.Find("GameCanvas").GetComponent<AudioSource>().Stop();
        }

    }

}
