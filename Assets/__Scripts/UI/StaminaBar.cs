using System.Collections;
using System.Collections.Generic;
using _Scripts;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StaminaBar : MonoBehaviour
{
    [Inject] private UserProgressHolder progressHolder;
    
    [SerializeField] private Image filledImage;
    [SerializeField] private TMP_Text text;

    private TweenerCore<float,float,FloatOptions> tween;
    
    public void UpdateValue(int i)
    {
        tween?.Kill();
        if (i > 48)
        {
            text.DOColor(Color.black, 0.5f);
        }
        else
        {
            text.DOColor(Color.white, 0.5f);
        }
        tween = filledImage.DOFillAmount(i / 100f, 0.95f);
        text.text = $"{i}%";
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

}
