using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private float _swipeAmount;
    [SerializeField] private float _swipeDuration;

    private enum DraggedDirection { Up, Down, Right, Left }

    [SerializeField] private float _width;

    private bool _isSwiping;
    private Vector3 _canvasScale => GetComponentInParent<Canvas>().transform.localScale;

    public async void OnEndDrag(PointerEventData eventData)
    {
        if (!_isSwiping)
        {
            Debug.Log("SwipableObject.OnEndDrag()");
            Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
            var dragDirection = GetDragDirection(dragVectorDirection);
            var position = transform.position;
            switch (dragDirection)
            {
                case DraggedDirection.Left:
                    position.x -= _swipeAmount * _canvasScale.x;
                    break;
                case DraggedDirection.Right:
                    position.x += _swipeAmount * _canvasScale.x;
                    break;
            }

            Debug.Log("swiping from " + transform.position.x + " to " + position.x);
            if (position.x <= _swipeAmount * _canvasScale.x && position.x >= _swipeAmount * _canvasScale.x - _width * _canvasScale.x)
            {
                _isSwiping = true;
                transform.DOMove(position, _swipeDuration);
                await Task.Delay(Mathf.RoundToInt(_swipeDuration * 1000));
                _isSwiping = false;
            }
        }
    }

    public void Update()
    {

    }

    private DraggedDirection GetDragDirection(Vector3 dragVector)
    {
        float positiveX = Mathf.Abs(dragVector.x);
        float positiveY = Mathf.Abs(dragVector.y);
        DraggedDirection draggedDir;
        if (positiveX > positiveY)
        {
            draggedDir = (dragVector.x > 0) ? DraggedDirection.Right : DraggedDirection.Left;
        }
        else
        {
            draggedDir = (dragVector.y > 0) ? DraggedDirection.Up : DraggedDirection.Down;
        }
        //Debug.Log(draggedDir);
        return draggedDir;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("SwipableObject.OnDrag()");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _width = GetComponent<RectTransform>().rect.width;
        //Debug.Log("SwipableObject.OnBeginDrag()");
    }
}
