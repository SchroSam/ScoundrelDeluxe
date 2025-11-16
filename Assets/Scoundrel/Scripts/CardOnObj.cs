using UnityEngine;
public class CardOnObj : MonoBehaviour
{
    public Card card = new Card();
    public void CardClicked()
    {
        FindFirstObjectByType<ScoundrelGame>().CardSelected(this);
    } 
}