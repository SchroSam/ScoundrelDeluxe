using System.Collections.Generic;
using UnityEngine;
using TMPro;

// the top of the deck is the last element in cards
public class Deck : MonoBehaviour
{
    public GameObject cardPrefab;
    public List<Card> cards;
    public static List<Card> fullDeck;
    public List<RectTransform> slotPositions;
    public List<bool> slotsUsed = new List<bool> {false, false, false, false};
    public bool done = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //CreateNew52();
        transform.GetChild(0).GetComponent<TMP_Text>().text = $"{fullDeck.Count}";
    }

    public void FlipTop(RectTransform spot, int index)
    {
        // handle ending in game logic
        // if(cards.Count == 0)
        //     FullShuffle();

        Card cardInfo = new Card(cards[cards.Count - 1]);

        cards.RemoveAt(cards.Count - 1);


        GameObject cardSpawn = Instantiate(cardPrefab, spot.position, Quaternion.identity, spot.parent);

        
        cardSpawn.transform.GetComponent<CardOnObj>().card.suit = cardInfo.suit;
        cardSpawn.transform.GetComponent<CardOnObj>().card.value = cardInfo.value;
        cardSpawn.transform.GetComponent<CardOnObj>().card.slotIndex = index;
        cardSpawn.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{cardInfo.SuitString()} {cardInfo.value}";

        cardSpawn.transform.localScale = Vector3.one * 0.9f;
        transform.GetChild(0).GetComponent<TMP_Text>().text = $"{cards.Count}";
    }

    public void FillSpots()
    {
        Debug.Log($"FillSpots called - cards.Count: {cards.Count}, slotsUsed: {string.Join(",", slotsUsed)}");
        for (int i = 0; i < slotPositions.Count; i++)
        {
            if(!slotsUsed[i]){
                slotsUsed[i] = true;
                Debug.Log($"Filling slot {i} - cards remaining: {cards.Count}");

                FlipTop(slotPositions[i], i);
            }                
        }
    }

    //Fisher-Yattes random list scramble
    public void Shuffle()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1); // UnityEngine.Random
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }

    public void FullShuffle()
    {
        cards = new List<Card>(fullDeck);
        Shuffle();
    }

    public void TuckCards()
    {
        GameObject[] cardsObj = GameObject.FindGameObjectsWithTag("Card");
        List<Card> tuckedCards = new List<Card>();

        foreach(GameObject obj in cardsObj)
        {
            var cardData = obj.GetComponent<CardOnObj>().card;
            // Create a new Card with the same suit and value
            tuckedCards.Add(new Card(cardData.suit, cardData.value));
            Destroy(obj);
        }

        // Insert all tucked cards at the bottom of the deck (beginning of list)
        cards.InsertRange(0, tuckedCards);

        slotsUsed = new List<bool> {false, false, false, false};

        done = true;
    }


    public void CreateNew52()
    {
        cards = new List<Card>();
        

        for (int i = 1; i <= 4; i++)
        {
            for(int j = 2; j <= 13; j++)
            {
                cards.Add(new Card((Suit)i, j));
            }
        }
        
        Shuffle();
        fullDeck = new List<Card>(cards);
    }
}


