using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : MonoBehaviour
{
    private ScoundrelGame gm;
    public TMP_Text goldText;
    public List<GameObject> items = new List<GameObject>();
    private List<ShopItem> itemStats = new List<ShopItem>();

    public void Start()
    {
        gm = FindFirstObjectByType<ScoundrelGame>();
    }

    public void GenerateItems()
    {
        itemStats = new List<ShopItem>();
        goldText.text = $"Gold: {gm.gold}";

        for(int i = 0; i < 4; i++)
        {
            ItemType type = (ItemType)Random.Range(0, 4);

            float itemRoll = Random.Range(8, 16 + gm.floornum);

            itemStats[i].value = (int)itemRoll;

            switch (type)
            {
                case ItemType.Health:
                    itemStats[i].strength = (int)(itemRoll / 3);
                break;

                case ItemType.Attack:
                    itemStats[i].strength = (int)(itemRoll / 2);
                break;

                case ItemType.PotionCard:
                    itemStats[i].strength = (int)(itemRoll / 1.5f);
                break;

                case ItemType.WeaponCard:
                    itemStats[i].strength = (int)itemRoll;
                break;
            }
        }
    }

    public string DescribeItem(ShopItem item)
    {
        switch (item.type)
        {
            case ItemType.Health:
                return $"A fortifying draught which increases your max health by {item.strength}. Worth {item.value} gold.";

            case ItemType.Attack:
                return $"A magic charm which grants a passive +{item.strength} damage to all attacks. Worth {item.value} gold.";

            case ItemType.PotionCard:
                return $"Adds a potion card of strength {item.strength} to the deck permanently. Worth {item.value} gold.";

            case ItemType.WeaponCard:
                return $"Add a weapon card of strength {item.strength} to the deck permanently. Worth {item.value} gold.";
        }

        return "ERROR DESCRIPTION";
    }

    public void ItemButton(int index)
    {
        if(gm.gold >= itemStats[index].value)
        {
            gm.gold -= itemStats[index].value;
            goldText.text = $"Gold: {gm.gold}";
            GiveItem(itemStats[index]);
            items[index].GetComponent<Button>().interactable = false;
        }
    }

    public void GiveItem(ShopItem item)
    {
        switch (item.type)
        {
            case ItemType.Health:
                gm.maxHealth += item.value;
                gm.health += item.value;
            break;

            case ItemType.Attack:
                gm.attackModifier += item.value;
            break;

            case ItemType.PotionCard:
                Deck.fullDeck.Add(new Card(Suit.Hearts, item.value));
            break;

            case ItemType.WeaponCard:
                Deck.fullDeck.Add(new Card(Suit.Diamonds, item.value));
            break;
        }
    }
}

public enum ItemType{Health, Attack, PotionCard, WeaponCard}

public class ShopItem
{
    public int value;
    public int strength;
    public ItemType type;

}
