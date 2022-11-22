using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMock : MonoBehaviour
{
    [SerializeField] private AnimationMock _animationMock;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _timeText;
    private float _currentTimer;

    public void StartSync()
    {
        _currentTimer = _animationMock.TotalAnimationTime;
    }

    private void LateUpdate()
    {
        if (_currentTimer > 0)
        {
            _currentTimer -= Time.deltaTime;
            SyncTimeBar();
        }
    }

    private void SyncTimeBar()
    {
        var totalTime = _animationMock.TotalAnimationTime;
        _slider.value = totalTime > 0 ? _currentTimer / totalTime : 0;
        _timeText.text = System.TimeSpan.FromSeconds(_currentTimer).ToString(@"ss\:ff");
    }
}
