using System;

[Serializable]
public enum Suit{Hearts = 1, Diamonds, Clubs, Spades,}

[Serializable]
public class Card : IComparable
{
    public Suit suit;
    public int value;
    public int slotIndex;

    public Card(Suit s, int v)
    {
        suit = s;
        value = v;
    }
    public Card(Card card)
    {
        suit = card.suit;
        value = card.value;
        slotIndex = card.slotIndex;
    }
    public Card()
    {
        suit = 0;
        value = 0;
    }

    public string SuitString()
    {
        switch(suit)
        {
            
            case Suit.Hearts:
                return "H";

            case Suit.Diamonds:
                return "D";

            case Suit.Clubs:
                return "C";

            case Suit.Spades:
                return "S";

            default:
                return "?";
            
        }
    }

    public string ValToString()
    {
        switch (value)
        {
            case 11:
                return "J";

            case 12:
                return "Q";

            case 13:
                return "K";

            case 14:
                return "A";

            default:
                return $"{value}";
        }

    }

    public static string ValToString(int val)
    {
        switch (val)
        {
            case 11:
                return "J";

            case 12:
                return "Q";

            case 13:
                return "K";

            case 14:
                return "A";

            default:
                return $"{val}";
        }
    }

    // CompareTo function for sorting based on value
    public int CompareTo(object obj)
    {
        if(obj == null) return 1;

        Card otherCard = obj as Card;

        if(otherCard != null)
            return value.CompareTo(otherCard.value);
        else
            throw new ArgumentException("Object is not a Card");
    }

}
