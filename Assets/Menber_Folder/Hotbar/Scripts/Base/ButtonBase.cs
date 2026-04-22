using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class ButtonBase : Button, IPointerDownHandler, IPointerUpHandler
{
    private const float c_pointerSize = 0.85f;

    public System.Action pointerDownEvent = null;
    public System.Action pointerUpEvent = null;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if(interactable == true)
        {
            transform.DOKill();
            transform.DOScale(new Vector3(transform.localScale.x >= 0 ? c_pointerSize : -c_pointerSize, c_pointerSize, c_pointerSize), 0.1f);
        }

        if (pointerDownEvent != null)
        {
            pointerDownEvent();
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        transform.DOKill();
        transform.DOScale(new Vector3(transform.localScale.x >= 0 ? 1f : -1f, 1f, 1f), 0.1f);

        if (pointerUpEvent != null)
        {
            pointerUpEvent();
        }
    }

    protected override void OnDisable()
    {
        transform.DOKill();
        transform.DOScale(new Vector3(transform.localScale.x >= 0 ? 1f : -1f, 1f, 1f), 0f);
    }
}
