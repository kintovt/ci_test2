using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonState { Disabled, Active, Error }

public class ReadyButton : MonoBehaviour
{
    [SerializeField] private Texture2D[] _textures;
    [SerializeField] private RawImage _buttonBody;
    [SerializeField] private ButtonState _buttonState;

    public void SetButtonState(ButtonState buttonState)
    {
        _buttonState = buttonState;
        _buttonBody.texture = _textures[(int)_buttonState];
        enabled = _buttonState == ButtonState.Active;
    }
}
