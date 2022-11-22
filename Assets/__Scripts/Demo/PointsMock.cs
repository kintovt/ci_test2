using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class PointsMock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TMP_Text prefab;
    [SerializeField] private TMP_Text finishText;
    private int _value = 0;

    private void OnEnable()
    {
        StartCoroutine(IncrementValue());
    }

    public void StopIncrement()
    {
        //_text.color = new Color32(26, 31, 0, 77);
        finishText.text = _value.ToString();
        StopAllCoroutines();
    }

    public void ResetScore()
    {
        _text.text = 0.ToString();
    }

    public void SetActive(bool active) => gameObject.SetActive(active);

    private IEnumerator IncrementValue()
    {
        _text.color = Color.black;
        while (enabled)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            var pointsToAdd = Random.Range(1, 15);
            _value += pointsToAdd;
            AddAnimation(pointsToAdd);
            _text.text = _value.ToString();
        }
    }

    private void AddAnimation(int points)
    {
        var go = Instantiate(prefab, transform);
        go.rectTransform.anchoredPosition = Vector2.zero;
        go.alpha = 0f;
        go.text = $"+{points.ToString()}";
        var sequence = DOTween.Sequence();
        sequence.Append(go.DOFade(1f, 0.2f));
        sequence.Append(go.DOFade(0f, 2.8f));
        sequence.Insert(0f,go.rectTransform.DOAnchorPosY(250f, 3f));
        sequence.Insert(0.2f,go.transform.DOScale(0.2f, 2.5f));
        sequence.OnComplete(() => Destroy(go.gameObject));
        sequence.Play();
    }
}