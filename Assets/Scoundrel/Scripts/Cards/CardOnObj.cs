using UnityEngine;
public class CardOnObj : WigglyButton
{
    public Card card = new Card();

    public void CardClicked()
    {
        ScoundrelGame.instance.CardSelected(this);
    }
}