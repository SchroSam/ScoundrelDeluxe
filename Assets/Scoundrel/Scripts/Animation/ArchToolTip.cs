using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArchToolTip : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerMoveHandler
{
    public static bool animStarted = false;
    public GameObject toolTipObj;
    private Image GreyOut;
    private float origPosX;
    public float endPosX;
    public float duration = 0.5f;
    public bool primary = true;

    void Awake()
    {
        if(primary)
        {
            origPosX = toolTipObj.transform.position.x;

            if(endPosX == 0)
                endPosX = transform.position.x;
        }
    }

    void Start()
    {
        GreyOut = GameObject.Find("StartCanvas").transform.Find("GreyOut").GetComponent<Image>();

        if(!primary)
            origPosX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        /// <summary>
    /// Starts the hover animation when the pointer enters the button.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(primary && !animStarted)
        {
            animStarted = true;
            GreyOut.enabled = true;
            GreyOut.DOFade(0.8f, duration);
            toolTipObj.transform.DOMoveX(endPosX, duration);
        }
    }

    // public void ReturnToOrig()
    // {
    //     toolTipObj.transform.DOMoveX(origPos.x, duration);
    // }

    /// <summary>
    /// Resets the button rotation when the pointer leaves the button.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if(!primary)
        {
            GreyOut.DOFade(0, duration).OnComplete(() => GreyOut.enabled = false);
            transform.DOMoveX(origPosX, duration).OnComplete(() => animStarted = false);
        }
    }

    /// <summary>
    /// Updates the button tilt while the pointer moves across the button.
    /// </summary>
    public void OnPointerMove(PointerEventData eventData)
    {
        
    }
}
