public enum Suit{Hearts = 1, Diamonds, Clubs, Spades,}

public class Card
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
        switch(suit){
            
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


}
