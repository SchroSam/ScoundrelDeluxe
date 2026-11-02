using UnityEngine;
using DG.Tweening;
public class CardOnObj : MonoBehaviour
{
    public Card card = new Card();
    private bool isWiggling = false;
    public void CardClicked()
    {
        ScoundrelGame.instance.CardSelected(this);
    }

    public void WiggleOnHover()
    {
        if(!isWiggling)
        {
            Sequence sq = DOTween.Sequence();

            sq.AppendCallback(() => isWiggling = true);
            sq.Append(transform.DOPunchRotation(new Vector3(0.0f, 0.0f, 15.0f), 0.50f));
            sq.OnComplete(()=> isWiggling = false);
        }

    }
}