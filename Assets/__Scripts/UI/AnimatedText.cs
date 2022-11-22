using System;
using TMPro;
using UnityEngine;

namespace _Scripts
{
    [RequireComponent(typeof(TMP_Text), typeof(Animation))]
    public class AnimatedText : MonoBehaviour
    {
        private TMP_Text _text;
        private Animation _animation;

        public string Text => _text.text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _animation = GetComponent<Animation>();
        }

        private void OnEnable()
        {
            _animation.Play();
        }

        public void SetActive(bool active) => gameObject.SetActive(active);
    }
}