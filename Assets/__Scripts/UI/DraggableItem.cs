using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform _rectTransform;
    private Transform _initialParent;
    private int _initialChildIndex;
    private CanvasGroup _canvasGroup;
    private Vector2 _mouseShift;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _initialParent = _rectTransform.parent;
        _initialChildIndex = transform.GetSiblingIndex();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("begin drag from " + _initialChildIndex);
        _rectTransform.SetParent(transform.parent.parent.parent.parent, true);
        transform.localScale = Vector3.one * 0.3333f;
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("drag");
        _rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("end drag");
        _rectTransform.SetParent(_initialParent, true);
        transform.SetSiblingIndex(_initialChildIndex);
        transform.localPosition = Vector3.zero;
        _canvasGroup.blocksRaycasts = true;
        transform.localScale = Vector3.one * 1.0f;
    }
}
