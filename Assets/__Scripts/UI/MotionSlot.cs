using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MotionSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private RawImage _motionImage;
    [SerializeField] private RawImage _lightImage;

    public int slotNum;
    public UnityEvent<int, int> slotFilled = new();

    private bool _isFilled;
    private MotionPreview _motionPreview;

    public void OnDrop(PointerEventData eventData)
    {
        _motionPreview = eventData.pointerDrag.GetComponent<MotionPreview>();
        if (_motionPreview != null)
        {
            _lightImage.enabled = false;
            _motionImage.enabled = true;
            _motionImage.texture = _motionPreview.GetTexture();
            if (!_isFilled)
            {
                _isFilled = true;
                slotFilled?.Invoke(slotNum, _motionPreview.ClipId);
            }
        }
    }

    public MotionPreview GetMotionPreview()
    {
        return _motionPreview;
    }
}
