using DG.Tweening;
using UnityEngine;

public static class TweenHelper
{
    //EXAMPLE FUNC
    public static Sequence PunchUp(Transform _transform)
    {
        Sequence sq = DOTween.Sequence();

        sq.Append(_transform.DOLocalMoveY(1.0f, 1.0f));
        sq.Join(_transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 0.7f));

        return sq;
    }

}
