using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyLabel : MonoBehaviour
{
    [SerializeField] private TMP_Text _textLabel;
    public RawImage label;

    public void SetEnergyValue(int value)
    {
        _textLabel.text = value.ToString();
    }

}
