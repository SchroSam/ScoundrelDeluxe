using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : MonoBehaviour
{
    public TMP_Text goldText;
    public int minimumRoll = 4;
    public List<GameObject> items = new();
    private List<ShopItem> itemStats = new();


    public void GenerateItems()
    {
        itemStats = new List<ShopItem>();
        goldText.text = $"Gold: {ScoundrelGame.instance.gold}";

        for(int i = 0; i < 4; i++)
        {
            float temp = Random.Range(0.0f, 10.0f);

            ItemType type; //= (ItemType)Random.Range(0, 5);

            if(temp < 3.0f)
                type = ItemType.Health;
            else if (temp < 5.0f)
                type = ItemType.Attack;
            else if (temp < 7.0f)
                type = ItemType.PotionCard;
            else if (temp < 9.0f)
                type = ItemType.WeaponCard;
            else
                type = ItemType.Special;


            float itemRoll = Random.Range(minimumRoll, minimumRoll*2 + ScoundrelGame.instance.floornum);

            itemStats.Add(new ShopItem());

            itemStats[i].type = type;

            itemStats[i].value = (int)itemRoll;

            switch (type)
            {
                case ItemType.Health:
                    itemStats[i].strength = (int)(itemRoll / 3);
                break;

                case ItemType.Attack:
                    itemStats[i].strength = (int)(itemRoll / 4);
                    itemStats[i].value *= 2;
                break;

                case ItemType.PotionCard:
                    itemStats[i].strength = (int)(itemRoll * 1.5);
                break;

                case ItemType.WeaponCard:
                    itemStats[i].strength = (int)(itemRoll * 1.5);
                break;

                case ItemType.Special:
                    itemStats[i].value = 25;
                break;
            }

            if(itemStats[i].strength <= 0)
                itemStats[i].strength = 1;

            items[i].transform.GetChild(0).GetComponent<TMP_Text>().text = DescribeItem(itemStats[i]);
            items[i].GetComponent<Button>().interactable = true;
        }
    }

    public string DescribeItem(ShopItem item)
    {
        switch (item.type)
        {
            case ItemType.Health:
                return $"A fortifying draught which increases your max health by {item.strength}.\n\nWorth {item.value} gold.";

            case ItemType.Attack:
                return $"A magic charm which grants a passive +{item.strength} damage to all attacks.\n\nWorth {item.value} gold.";

            case ItemType.PotionCard:
                return $"Adds a potion card of strength {item.strength} to the deck permanently.\n\nWorth {item.value} gold.";

            case ItemType.WeaponCard:
                return $"Add a weapon card of strength {item.strength} to the deck permanently.\n\nWorth {item.value} gold.";

            case ItemType.Special:
                return GetSpecialDescription(item.value);
        }

        return "ERROR DESCRIPTION";
    }

    public string GetSpecialDescription(int value)
    {
        switch (ScoundrelGame.instance.player)
        {
            case Archetype.Knight:
                return $"A magic charm that lets you drink one more potion per room before becoming sick. \n\nWorth {value} gold.";

            case Archetype.Wizard:
                return $"A disposable fireball scroll which damages the selected and adjacent enemeis. \n\nWorth {value} gold.";

            case Archetype.Elf:
                return $"Enchanted boots which allow the user to skip past one additional room. \n\nWorth {value} gold.";

            case Archetype.Warrior:
                return $"A talisman which restores the warrior's rage whenever he reaches 5 or less health (or adds 2 to this limit if already taken). \n\nWorth {value} gold.";
        }
            
        
        return "ERROR SPECIAL";
    }

    public void ItemButton(int index)
    {
        if(ScoundrelGame.instance.gold >= itemStats[index].value)
        {
            ScoundrelGame.instance.gold -= itemStats[index].value;
            goldText.text = $"Gold: {ScoundrelGame.instance.gold}";
            GiveItem(itemStats[index]);
            items[index].GetComponent<Button>().interactable = false;
        }
    }

    public void GiveItem(ShopItem item)
    {
        switch (item.type)
        {
            case ItemType.Health:
                ScoundrelGame.instance.maxHealth += item.value;
                ScoundrelGame.instance.health += item.value;
            break;

            case ItemType.Attack:
                ScoundrelGame.instance.attackModifier += item.value;
            break;

            case ItemType.PotionCard:
                Deck.fullDeck.Add(new Card(Suit.Hearts, item.value));
            break;

            case ItemType.WeaponCard:
                Deck.fullDeck.Add(new Card(Suit.Diamonds, item.value));
            break;

            case ItemType.Special:
                switch (ScoundrelGame.instance.player)
                {
                    case Archetype.Knight:
                    ScoundrelGame.instance.potionLimit++;

                    break;

                    case Archetype.Wizard:
                    ScoundrelGame.instance.SetButtonActive(ScoundrelGame.instance.fireBallButton, true);

                    break;

                    case Archetype.Elf:
                    ScoundrelGame.instance.skipLimit++;

                    break;

                    case Archetype.Warrior:
                    
                    if(ScoundrelGame.instance.rageRecovery == 0)
                        ScoundrelGame.instance.rageRecovery = 5;
                    else
                        ScoundrelGame.instance.rageRecovery += 2;

                    break;
                }
            break;
        }
    }

    public void NextFloorButton()
    {
        GetComponent<Canvas>().enabled = false;
        ScoundrelGame.instance.NewRoom();
        AudioPlayer.instance.StopMusic();
        AudioPlayer.instance.PlayMusic(AudioPlayer.instance.musicClips[Random.Range(1, 5)].clip.name);
        ScoundrelGame.instance.healthText.text = $"Health: {ScoundrelGame.instance.health}/{ScoundrelGame.instance.maxHealth}";
        if(ScoundrelGame.instance.attackModifier > 0)
            GameObject.Find("AttackModifier").GetComponent<TMP_Text>().text = $"Attack Modifier: +{ScoundrelGame.instance.attackModifier}";
        GameObject.Find("GameCanvas").GetComponent<Canvas>().enabled = true;
    }
}

/*
ideas for cool items:

Knight: can drink 1 more potion before potions are disabled

Wizard: consumable Fireball that hits adjacent enemies

Elf: Nimble boots - can skip twice in a row

Warrior: Every time you go to 7 or less health, your rage is turned back on
*/

public enum ItemType{Health, Attack, PotionCard, WeaponCard, Special}

public class ShopItem
{
    public int value;
    public int strength;
    public ItemType type;

}
