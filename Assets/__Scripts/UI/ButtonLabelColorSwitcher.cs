using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLabelColorSwitcher : MonoBehaviour
{
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    [SerializeField] private Color _errorColor;

    private Button _button;
    private TextMeshProUGUI _label;

    private void Start()
    {
        _button = GetComponentInParent<Button>();
        _label = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_button.interactable) {
            _label.color = _activeColor;
        } else
        {
            if (_button.GetComponent<ButtonErrorState>() != null && _button.GetComponent<ButtonErrorState>().Error)
            {
                _label.color = _errorColor;
            } else
            {
                _label.color = _inactiveColor;
            }
        }
    }
}
