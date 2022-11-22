using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MotionCardViewer : MonoBehaviour
{

    [SerializeField] private TMP_Text _textLabel;

    public void SetLabelText(string text)
    {
        _textLabel.text = text;
    }
}
