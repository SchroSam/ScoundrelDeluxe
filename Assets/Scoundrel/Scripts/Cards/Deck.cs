using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

// the top of the deck is the last element in cards
public class Deck : MonoBehaviour
{
    public GameObject cardPrefab;
    public List<Card> cards;
    public static List<Card> fullDeck;
    public List<RectTransform> slotPositions;
    public List<bool> slotsUsed = new List<bool> {false, false, false, false};
    public List<GameObject> cardsOnBoard = new List<GameObject> {null, null, null, null};
    public Sprite smallHearts;
    public Sprite smallDiamonds;

    public Sprite smallClubs;
    public Sprite midClubs;
    public Sprite jackClubs;
    public Sprite queenClubs;
    public Sprite kingClubs;
    public Sprite aceClubs;

    public Sprite smallSpades;
    public Sprite midSpades;
    public Sprite jackSpades;
    public Sprite queenSpades;
    public Sprite kingSpades;
    public Sprite aceSpades;

    public Sprite manaGem;
    public Material foilMat;
    public Sprite skullStamp;

    public Sequence FlipTop(RectTransform spot, int index)
    {
        // handle ending in game logic
        // if(cards.Count == 0)
        //     FullShuffle();

        Card cardInfo = new Card(cards[cards.Count - 1]);

        cards.RemoveAt(cards.Count - 1);

        // Instantiate on top of the deck, then arc over to the spot.
        // They are not interactable until they get done animating

        //GameObject cardSpawn = Instantiate(cardPrefab, spot.position, Quaternion.identity, spot.parent);
        GameObject cardSpawn = Instantiate(cardPrefab, transform.position, Quaternion.Euler(90f, 0f, 0f), spot.parent);
        Quaternion startRotation = cardSpawn.transform.localRotation;
        cardSpawn.transform.localRotation = startRotation * Quaternion.Euler(180f, 0f, 0f);

        Sequence sq = DOTween.Sequence();
        sq.TogglePause();
        sq.Append(cardSpawn.transform.DOJump(spot.position, 50.0f, 1, 0.5f)
        .Join(cardSpawn.transform.DOLocalRotateQuaternion(startRotation, 0.5f))
        .OnComplete(()=> cardSpawn.GetComponent<Button>().interactable = true));
        
        cardSpawn.transform.GetComponent<CardOnObj>().card.suit = cardInfo.suit;
        cardSpawn.transform.GetComponent<CardOnObj>().card.value = cardInfo.value;
        cardSpawn.transform.GetComponent<CardOnObj>().card.slotIndex = index;
        cardsOnBoard[index] = cardSpawn;
        cardSpawn.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{cardInfo.value}{cardInfo.SuitString()}";
        cardSpawn.transform.GetChild(1).GetComponent<TMP_Text>().text = $"{cardInfo.value}{cardInfo.SuitString()}";
        cardSpawn.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{cardInfo.value}{cardInfo.SuitString()}";
        if(cardInfo.suit == Suit.Hearts || cardInfo.suit == Suit.Diamonds)
        {
            cardSpawn.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
            cardSpawn.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.red;
        }
        
        GiveCardArt(cardSpawn, cardInfo.suit, cardInfo.value);



        cardSpawn.transform.localScale = Vector3.one * 0.9f;
        transform.GetChild(0).GetComponent<TMP_Text>().text = $"{cards.Count}";

        return sq;
    }

    // Percentage of the deck with a value greater than or equal to this card's value.
    float CalcTopPercentile(int value)
    {
        int cardsGreaterOrEqual = 0;
        for (int i = fullDeck.Count - 1; i >= 0; i--)
        {
            if (fullDeck[i].value >= value)
                cardsGreaterOrEqual++;
            else
                break;
        }

        return fullDeck.Count == 0 ? 0f : cardsGreaterOrEqual * 100f / fullDeck.Count;
    }

    void GiveCardArt(GameObject obj, Suit suit, int value)
    {
        float perc = CalcTopPercentile(value);



        Image image = obj.GetComponent<Image>();

        if(perc < 30.77f)
            image.material = foilMat;

        Material instance = new Material(image.material);


        if(perc < 30.77f)
        {
            instance.SetTextureScale("_BaseTexture", Vector2.one);
            instance.SetTextureOffset("_BaseTexture", Vector2.zero);
            instance.SetFloat("_Holo_Offset", Random.Range(0.0f, 100.0f));
            instance.SetVector("_Holo_Direction", new Vector4(Random.Range(1.0f, 2.0f), Random.Range(1.0f, 2.0f)));

            int temp = Random.Range(0, 4);
            Sprite monsterArt = GetMonsterArt(suit, perc);

            //Full art
            if(temp == 1)
            {
                instance.SetFloat("_Holo_Density", 0.01f);
                instance.SetFloat("_Holo_Noise_Scale", 42f);
                instance.SetFloat("_Holo_Anim_Speed", 0.1f);
                instance.SetFloat("_Holo_Offset", 100.0f);
                instance.SetVector("_Holo_Direction", new Vector2(0.19f, -0.43f));
            }
            //Full art smooth
            else if(temp == 2)
            {
                instance.SetFloat("_Holo_Density", 0.01f);
                instance.SetFloat("_Holo_Noise_Scale", 0.01f);
                instance.SetFloat("_Holo_Anim_Speed", 0.1f);
                instance.SetFloat("_Holo_Offset", 100.0f);
                instance.SetVector("_Holo_Direction", new Vector2(0.19f, -0.43f));
            }
            //Skull stamp
            else if(temp == 3 && (suit == Suit.Spades || suit == Suit.Clubs))
            {
                instance.SetColor("_Holo_Color",  new Color(138f, 0f, 0f));
                instance.SetFloat("_Holo_Density", 0.01f);
                instance.SetFloat("_Holo_Offset", 100.0f);
                instance.SetFloat("_Holo_Anim_Speed", 0.21f);
                instance.SetFloat("_Holo_Noise_Scale", 0.1f);
            }

            image.material = instance;
            image.type = Image.Type.Simple;

            switch (suit)
            {
                case Suit.Hearts:
                instance.SetTexture("_BaseTexture", smallHearts.texture);
                instance.SetTexture("_Holo_Mask", smallHearts.texture);
                break;

                case Suit.Diamonds:
                if(ScoundrelGame.instance.player != Archetype.Wizard)
                {
                    instance.SetTexture("_BaseTexture", smallDiamonds.texture);
                    instance.SetTexture("_Holo_Mask", smallDiamonds.texture);
                }
                else
                {
                    instance.SetTexture("_BaseTexture", manaGem.texture);
                    instance.SetTexture("_Holo_Mask", manaGem.texture);
                }
                break;

                case Suit.Clubs:
                    instance.SetTexture("_BaseTexture", monsterArt.texture);
                    if(temp != 3)
                        instance.SetTexture("_Holo_Mask", monsterArt.texture);
                    else
                        instance.SetTexture("_Holo_Mask", skullStamp.texture);
                break;

                case Suit.Spades:
                    instance.SetTexture("_BaseTexture", monsterArt.texture);
                    if(temp != 3)
                        instance.SetTexture("_Holo_Mask", monsterArt.texture);
                    else
                        instance.SetTexture("_Holo_Mask", skullStamp.texture);
                break;
            }
        }
        else
        {

            switch (suit)
            {
                case Suit.Hearts:
                    //instance.SetTexture("_BaseTexture", smallHearts.texture);
                    obj.GetComponent<Image>().sprite = smallHearts;
                    break;

                case Suit.Diamonds:
                    if(ScoundrelGame.instance.player != Archetype.Wizard)
                        obj.GetComponent<Image>().sprite = smallDiamonds;
                    else
                        obj.GetComponent<Image>().sprite = manaGem;
                    break;

                case Suit.Clubs:
                    obj.GetComponent<Image>().sprite = GetMonsterArt(suit, perc);

                    break;

                case Suit.Spades:
                    obj.GetComponent<Image>().sprite = GetMonsterArt(suit, perc);

                    break;

            }
        }
    }

    Sprite GetMonsterArt(Suit suit, float perc)
    {
        switch (suit)
        {
            case Suit.Clubs:
                if (perc > 61.54f)
                    return smallClubs;
                if (perc > 30.77f)
                    return midClubs;
                if (perc > 23.08f)
                    return jackClubs;
                if (perc > 15.38f)
                    return queenClubs;
                if (perc > 7.69f)
                    return kingClubs;
                return aceClubs;

            case Suit.Spades:
                if (perc > 61.54f)
                    return smallSpades;
                if (perc > 30.77f)
                    return midSpades;
                if (perc > 23.08f)
                    return jackSpades;
                if (perc > 15.38f)
                    return queenSpades;
                if (perc > 7.69f)
                    return kingSpades;
                return aceSpades;

            default:
                return null;
        }
    }

    public void FillSpots()
    {
        Sequence sq = DOTween.Sequence();
        sq.TogglePause();
        //Debug.Log($"FillSpots called - cards.Count: {cards.Count}, slotsUsed: {string.Join(",", slotsUsed)}");
        for (int i = 0; i < slotPositions.Count; i++)
        {
            if(!slotsUsed[i]){
                slotsUsed[i] = true;
                //Debug.Log($"Filling slot {i} - cards remaining: {cards.Count}");

                sq.Append(FlipTop(slotPositions[i], i));
                //sq.AppendInterval(0.1f);
            }                
        }

        sq.PlayForward();

        GameObject.Find("Progress").GetComponent<Slider>().value = 1 - ((float)cards.Count/fullDeck.Count);
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

        //done = true;
    }


    public void CreateNew52()
    {
        fullDeck = new List<Card>();
        

        for (int i = 1; i <= 4; i++)
        {
            for(int j = 2; j <= 14; j++)
            {
                fullDeck.Add(new Card((Suit)i, j));
            }
        }


        transform.GetChild(0).GetComponent<TMP_Text>().text = $"{cards.Count}";
        cards = new List<Card>(fullDeck);

        fullDeck.Sort();
        Shuffle();
        
    }

    // not to be used mid floor!
    public void RemoveTopN(int n)
    {
        for(int i = 0; i < n; i++)
        {
            cards.RemoveAt(cards.Count - 1);
        }

        fullDeck = new List<Card>(cards);
        fullDeck.Sort();
    }

    public int FindGreatestMonster()
    {
        fullDeck.Sort();

        for(int i = fullDeck.Count; i >= 0; i--)
        {
            if(fullDeck[i].suit == Suit.Clubs || fullDeck[i].suit == Suit.Spades)
                return fullDeck[i].value;
        }

        return 0;
    }
}
