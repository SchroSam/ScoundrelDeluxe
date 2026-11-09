using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
public class CardOnObj : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerMoveHandler
{
    public Card card = new Card();
    private bool isWiggling = false;
    private Tween hoverRotationTween;
    private Tween wiggleTween;
    private Quaternion originalRotation;
    private RectTransform rectTransform;
    private Vector3 tiltEuler;
    private float wiggleAngle;
    public float maxTilt = 15f;
    public void CardClicked()
    {
        ScoundrelGame.instance.CardSelected(this);
    }

    // Visual stuff begins
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalRotation = transform.localRotation;
    }

    public void RotateTowardMouse(PointerEventData eventData)
    {

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localMouse
        );

        Vector2 cardCenter = rectTransform.rect.center;
        float normalizedX = Mathf.Clamp(
            (localMouse.x - cardCenter.x) / (rectTransform.rect.width / 2f),
            -1f,
            1f);
        float normalizedY = Mathf.Clamp(
            (localMouse.y - cardCenter.y) / (rectTransform.rect.height / 2f),
            -1f,
            1f);

        float pitch = normalizedY * maxTilt;
        float yaw = -normalizedX * maxTilt;

        hoverRotationTween?.Kill();

        hoverRotationTween = DOTween.To(
                () => tiltEuler,
                value =>
                {
                    tiltEuler = value;
                    ApplyRotation();
                },
                new Vector3(pitch, yaw, 0f),
                0.12f)
            .SetEase(Ease.OutQuad);

    }

    public void ResetRotation()
    {
        hoverRotationTween?.Kill();
        hoverRotationTween = DOTween.To(
                () => tiltEuler,
                value =>
                {
                    tiltEuler = value;
                    ApplyRotation();
                },
                Vector3.zero,
                0.15f)
            .SetEase(Ease.OutQuad);
    }

    public void WiggleOnHover()
    {
        if(!isWiggling)
        {
            wiggleTween?.Kill();
            wiggleTween = DOTween.Sequence()
                .AppendCallback(() => isWiggling = true)
                .Append(DOTween.To(() => wiggleAngle, SetWiggleAngle, 7.5f, 0.1f))
                .Append(DOTween.To(() => wiggleAngle, SetWiggleAngle, -5f, 0.1f))
                .Append(DOTween.To(() => wiggleAngle, SetWiggleAngle, 3f, 0.1f))
                .Append(DOTween.To(() => wiggleAngle, SetWiggleAngle, 0f, 0.2f))
                .OnComplete(() => isWiggling = false);
        }

    }

    private void SetWiggleAngle(float angle)
    {
        wiggleAngle = angle;
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        transform.localRotation =
            originalRotation * Quaternion.Euler(tiltEuler.x, tiltEuler.y, wiggleAngle);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        WiggleOnHover();
        RotateTowardMouse(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetRotation();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        RotateTowardMouse(eventData);
    }
}