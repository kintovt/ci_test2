using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaSetter : MonoBehaviour
{
    [SerializeField] private Canvas _targetCanvas;
    private RectTransform _safeAreaRectTransform;
    private Rect _currentSafeArea;
    private ScreenOrientation _currentOrientation = ScreenOrientation.AutoRotation;


    private void Start()
    {
        _safeAreaRectTransform = GetComponent<RectTransform>();
        _currentOrientation = Screen.orientation;
        _currentSafeArea = Screen.safeArea;

        ApplySafeArea();
    }

    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= _targetCanvas.pixelRect.width;
        anchorMin.y /= _targetCanvas.pixelRect.height;

        anchorMax.x /= _targetCanvas.pixelRect.width;
        anchorMax.y /= _targetCanvas.pixelRect.height;

        _safeAreaRectTransform.anchorMin = anchorMin;
        _safeAreaRectTransform.anchorMax = anchorMax;

        _currentOrientation = Screen.orientation;
        _currentSafeArea = Screen.safeArea;
    }

    private void Update()
    {
        if (_currentOrientation != Screen.orientation || _currentSafeArea != Screen.safeArea)
        {
            ApplySafeArea();
        }
    }
}
