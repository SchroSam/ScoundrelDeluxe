using System.Collections.Generic;
using TMPro;
using UnityEditor;
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
            ItemType type = (ItemType)Random.Range(0, 4);

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
        }

        return "ERROR DESCRIPTION";
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

public enum ItemType{Health, Attack, PotionCard, WeaponCard}

public class ShopItem
{
    public int value;
    public int strength;
    public ItemType type;

}
