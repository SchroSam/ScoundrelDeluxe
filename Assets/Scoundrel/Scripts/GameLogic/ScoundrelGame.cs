using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoundrelGame : MonoBehaviour
{
    public int health = 20;
    public int maxHealth = 20;
    public int weaponVal = 0;
    public int elfWeaponVal = 0;
    private int weaponDamage = 14;
    private int maxDamage = 14;
    public int elfWeaponDamage = 14;
    //public bool usingWeapon = false;
    private int wizardMana = 0;
    private int wizardMaxMana = 15;
    public Archetype player;
    private List<Card> monstersSlain;
    public Deck deck;
    public TMP_Text healthText;
    private TMP_Text damageText;
    private TMP_Text elfDamageText;
    private GameObject weaponButton;
    private GameObject elfWeaponButton;
    private GameObject primaryWeaponToggle;
    private GameObject skipButton;
    private string playerName = "Scott";
    private bool noPotThisRoom = true;
    private GameObject rageButton;
    private GameObject spellSlot;
    private TMP_Text winLoseText;
    private GameObject shopButton;
    private Tuple<bool, bool> elfWeaponsEquipped = new Tuple<bool, bool>(false, false);
    public bool elfWeaponPrimary = false;
    public string winMessage = "Dungeon Cleared - Success";
    public string loseMessage = "You have fallen";
    public int startingDeckSize = 32;
    public int floornum = 1;
    public int monstersPerFloor = 3;
    public int attackModifier = 0;
    public ShopScreen shop;
    public int gold;
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

        monstersSlain = new List<Card>();

        if(winLoseText == null)
            winLoseText = GameObject.Find("WinLoseText").GetComponent<TMP_Text>();

        if(shopButton == null)
            shopButton = GameObject.Find("NextFloorButton");

        if(shop == null)
            shop = FindFirstObjectByType<ShopScreen>();
        
        deck.CreateNew52();
        deck.RemoveTopN(52 - startingDeckSize);
        NewRoom();
    }

    public void ChooseArch(Archetype p)
    {
        if(damageText == null)
            damageText = GameObject.Find("Damage").GetComponent<TMP_Text>();
        if(weaponButton == null)
            weaponButton = GameObject.Find("Weapon");
        if(elfDamageText == null)
            elfDamageText = GameObject.Find("ElfDamage").GetComponent<TMP_Text>();
        if(elfWeaponButton == null)
            elfWeaponButton = GameObject.Find("ElfWeapon");

        player = p;

        switch (player)
        {
            // Knight is more tanky - most vanilla experience
            case Archetype.Knight:
                maxHealth += 7;
                health += 7;

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

                SetButtonActive(spellSlot, true);
                spellSlot.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{wizardMana}M";

                SetButtonActive(weaponButton, false);
                damageText.enabled = false;

                playerName = "Merlin - Wizard";
                break;

            // Elf is less healthy, but they have two weapon fighting
            case Archetype.Elf:
                maxHealth -= 5;
                health -= 5;

                SetButtonActive(elfWeaponButton, true);

                if(primaryWeaponToggle == null)
                    primaryWeaponToggle = GameObject.Find("PrimaryWeaponToggle");

                SetButtonActive(primaryWeaponToggle, true);
                primaryWeaponToggle.GetComponent<Button>().interactable = false;

                elfDamageText.enabled = true;

                playerName = "Questor - Elf";
                break;

            // Warrior can once per game go into a rage and destroy everything in the room without taking any damage
            case Archetype.Warrior:

                if(rageButton == null)
                    rageButton = GameObject.Find("Rage");

                SetButtonActive(rageButton, true);

                playerName = "Dwalin - Warrior";
                break;

        }

        StartGame();
    }

    void DisableArchSpec()
    {
        if(rageButton == null)
            rageButton = GameObject.Find("Rage");

        SetButtonActive(rageButton, false);

        if(elfWeaponButton == null)
            elfWeaponButton = GameObject.Find("ElfWeapon");

        SetButtonActive(elfWeaponButton, false);
        elfWeaponDamage = 14;
        elfWeaponVal = 0;

        elfWeaponButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
        elfWeaponButton.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
        elfWeaponButton.GetComponent<Button>().interactable = false;
        elfWeaponButton.GetComponent<Image>().color = Color.grey;
        elfWeaponButton.GetComponent<WeaponButton>().isReadied = false;
        elfWeaponsEquipped = new Tuple<bool, bool>(false, false);

        if(primaryWeaponToggle == null)
            primaryWeaponToggle = GameObject.Find("PrimaryWeaponToggle");
        if(elfDamageText == null)
            elfDamageText = GameObject.Find("ElfDamage").GetComponent<TMP_Text>();
        if(elfWeaponButton == null)
            elfWeaponButton = GameObject.Find("ElfWeapon");


        SetButtonActive(primaryWeaponToggle, false);
        primaryWeaponToggle.transform.GetChild(0).GetComponent<TMP_Text>().text = "Left is offhand";
        elfWeaponPrimary = false;
        elfDamageText.text = "0";
        elfDamageText.enabled = false;

        if(spellSlot == null)
            spellSlot = GameObject.Find("SpellSlot");

        SetButtonActive(spellSlot, false);

        SetButtonActive(weaponButton, true);

        damageText.enabled = true;
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
                {
                    obj.GetComponent<Animator>().enabled = false;
                    obj.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
                    obj.GetComponent<Image>().color = Color.grey;
                }

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
                    // equipping two initial weapons
                    if(elfWeaponsEquipped.Item2 == false)
                    {
                        elfWeaponsEquipped = new Tuple<bool, bool>(false, true);
                        EquipPrimary(cardO);
                    }

                    else if(elfWeaponsEquipped.Item1 == false)
                    {
                        elfWeaponsEquipped = new Tuple<bool, bool>(true, true);
                        EquipSecondary(cardO);
                    }

                    // selecting which to throw away based on which one is not the primary
                    else if (elfWeaponPrimary) // if the elf weapon is the primary
                        EquipPrimary(cardO);

                    else if (!elfWeaponPrimary) // if the elf weapon is not the primary
                        EquipSecondary(cardO);
                }
                else
                    EquipPrimary(cardO);

            }
            else // mana for the wizard
            {
                wizardMana += cardO.card.value;

                if(wizardMana > wizardMaxMana)
                    wizardMana = wizardMaxMana;

                spellSlot.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{wizardMana}M";

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

    public void EquipPrimary(CardOnObj cardO)
    {
        weaponVal = cardO.card.value;
        weaponDamage = maxDamage;

        //monstersSlainWithWeapon = new List<Card>();

        weaponButton.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{Card.ValToString(weaponVal)}D";
        weaponButton.transform.GetChild(1).GetComponent<TMP_Text>().text = $"{Card.ValToString(weaponVal)}D";
        weaponButton.GetComponent<Button>().interactable = true;

        damageText.text = $"{maxDamage}";
    }

    public void EquipSecondary(CardOnObj cardO)
    {
        elfWeaponVal = cardO.card.value;
        elfWeaponDamage = maxDamage;

        //monstersSlainWithWeapon = new List<Card>();

        elfWeaponButton.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{Card.ValToString(elfWeaponVal)}D";
        elfWeaponButton.transform.GetChild(1).GetComponent<TMP_Text>().text = $"{Card.ValToString(elfWeaponVal)}D";
        elfWeaponButton.GetComponent<Button>().interactable = true;

        elfDamageText.text = $"{maxDamage}";


        primaryWeaponToggle.GetComponent<Button>().interactable = true;
    }

    public void ChangePrimaryWeap()
    {
        elfWeaponPrimary = !elfWeaponPrimary;

        if (elfWeaponPrimary)
            primaryWeaponToggle.transform.GetChild(0).GetComponent<TMP_Text>().text = "Right is offhand";
        else
            primaryWeaponToggle.transform.GetChild(0).GetComponent<TMP_Text>().text = "Left is offhand";
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
                {
                    obj.GetComponent<Animator>().enabled = true;
                    obj.GetComponent<Button>().transition = Selectable.Transition.Animation;
                    obj.GetComponent<Image>().color = Color.white;
                }

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

    public void SelectWeaponToggle(WeaponButton weaponSelected)
    {
        Color onColor;

        if(player != Archetype.Wizard)
            onColor = Color.green;
        else
            onColor = Color.cyan;


        weaponSelected.isReadied = !weaponSelected.isReadied;
        // Debug.Log(weaponSelected.name);
        // Debug.Log(weaponSelected.isReadied);

        if(weaponSelected.isReadied)
            weaponSelected.GetComponent<Image>().color = onColor;
        else
            weaponSelected.GetComponent<Image>().color = Color.grey;

        // handles the case where we just clicked the elf weapon to ready it, but the main weapon is also readied
        if(weaponSelected.isElfWeapon && weaponSelected.isReadied) //&& weaponButton.GetComponent<WeaponButton>().isReadied)
        {
            weaponButton.GetComponent<WeaponButton>().isReadied = false;
            weaponButton.GetComponent<Image>().color = Color.grey;
        }
        // handles the case where we just clicked the main weapon to ready it, but the elf weapon is also readied
        else if(!weaponSelected.isElfWeapon && weaponSelected.isReadied) //&& elfWeaponButton.GetComponent<WeaponButton>().isReadied)
        {
            elfWeaponButton.GetComponent<WeaponButton>().isReadied = false;
            elfWeaponButton.GetComponent<Image>().color = Color.grey;
        }
    }

    void FightMonster(Card card)
    {
        // first val represents if we're using a weapon, second val is what weapon we're using (false for normal weapon, true for elf weapon)
        Tuple <bool, bool> usingWhichWeapon;

        // find which if either button is readied
        if(weaponButton.GetComponent<WeaponButton>().isReadied)
            usingWhichWeapon = new Tuple<bool, bool>(true, false);
        else if(elfWeaponButton.GetComponent<WeaponButton>().isReadied)
            usingWhichWeapon = new Tuple<bool, bool>(true, true);
        else
            usingWhichWeapon = new Tuple<bool, bool>(false, false);


        // use the weapon's information that's selected in the comparisons
        int weaponDamage;
        int weaponVal;
        TMP_Text damageText;

        if(!usingWhichWeapon.Item2)
        {
            damageText = this.damageText;
            weaponDamage = this.weaponDamage;
            weaponVal = this.weaponVal;
        }
        else
        {
            damageText = elfDamageText;
            weaponDamage = elfWeaponDamage;
            weaponVal = elfWeaponVal;
        }

        // actual damage logic
        if(!usingWhichWeapon.Item1 || (weaponDamage <= card.value && weaponDamage != maxDamage))
        {
            int damage = card.value - attackModifier;

            if (damage > 0)
                health -= damage;
        }
        else if ((usingWhichWeapon.Item1 && weaponDamage > card.value) || (usingWhichWeapon.Item1 && weaponDamage == maxDamage)) // technically usingWhicWeapon.Item1 must be true if we made it here implicitly
        {
            if(card.value - attackModifier - weaponVal > 0)
                health -= card.value - attackModifier - weaponVal;

            weaponDamage = card.value;

            if(weaponDamage != maxDamage)
                damageText.text = $"{weaponDamage - 1}";
            else
                damageText.text = $"{weaponDamage}";

            Debug.Log(damageText.text);
            monstersSlain.Add(card);
        }

        healthText.text = $"Health: {health}/{maxHealth}";

        // giving back the values to the global versions of the variables
        if(!usingWhichWeapon.Item2)
        {
            this.damageText.text = damageText.text;
            this.weaponDamage = weaponDamage;
        }
        else
        {
            elfDamageText.text = damageText.text;
            elfWeaponDamage = weaponDamage;
        }
    }

    void FightMonsterWizard(Card card)
    {
        if(spellSlot.GetComponent<WeaponButton>().isReadied)
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

        shopButton.GetComponent<Button>().interactable = true;

        GameObject.Find("Progress").GetComponent<Slider>().value = 1f;

        SetButtonActive(shopButton, true);

        for(int i = 0; i < monstersSlain.Count; i++)
        {
            gold += monstersSlain[i].value / 8;
        }

        monstersSlain.Clear();
    }

    public void NextShop()
    {
        CleanupNewRound();
        AddRandMonsters();
        deck.FullShuffle();
        // NewRoom();
        SetButtonActive(shopButton, false);
        floornum++;

        if(player == Archetype.Warrior)
            rageButton.GetComponent<Button>().interactable = true;

        StartShop();
    }

    public void AddRandMonsters()
    {
        Card monster;

        for(int i = 0; i < monstersPerFloor; i++)
        {
            monster = new Card
            {
                suit = UnityEngine.Random.Range(0, 2) == 0 ? Suit.Clubs : Suit.Spades,

                value = UnityEngine.Random.Range(5, 11) + floornum
            };

            if(monster.value >= maxDamage)
                maxDamage = monster.value;

            Deck.fullDeck.Add(monster);
        }
    }

    public void StartShop()
    {
        AudioPlayer.instance.StopMusic();
        GameObject.Find("GameCanvas").GetComponent<Canvas>().enabled = false;
        shop.GenerateItems();
        GameObject.Find("ShopCanvas").GetComponent<Canvas>().enabled = true;
        
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

    public void CleanupNewRound(bool fullReset = false)
    {
        if(GameObject.Find("StartCanvas").GetComponent<Canvas>().enabled == false)
        {
            Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

            foreach (Button button in buttons)
            {
                button.interactable = true;

                if(button.CompareTag("Card"))
                    Destroy(button.gameObject);
            }

            deck.slotsUsed = new List<bool> {false, false, false, false};

            
            winLoseText.enabled = false;

            if(fullReset)
            {
                GameObject.Find("GameCanvas").GetComponent<Canvas>().enabled = false;
                GameObject.Find("StartCanvas").GetComponent<Canvas>().enabled = true;
                GameObject.Find("PauseCanvas").GetComponent<Canvas>().enabled = false;
                health = 20;
                maxHealth = 20;
                DisableArchSpec();
                monstersSlain.Clear();
                damageText.text = "0";
                weaponButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                weaponButton.transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                weaponButton.GetComponent<WeaponButton>().isReadied = false;
                weaponButton.GetComponent<Button>().interactable = false;
                weaponButton.GetComponent<Image>().color = Color.grey;
                AudioPlayer.instance.StopMusic();
                attackModifier = 0;
                gold = 0;
                GameObject.Find("AttackModifier").GetComponent<TMP_Text>().text = "";
            }
            
        }

    }

    private void SetButtonActive(GameObject button, bool active)
    {
        button.GetComponent<Button>().enabled = active;
        button.GetComponent<Image>().enabled = active;
        button.transform.GetChild(0).GetComponent<TMP_Text>().enabled = active;
    }

}
