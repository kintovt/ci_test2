using UnityEngine;
using DG.Tweening;

public class AnimatorSpeedController : MonoBehaviour
{
    [SerializeField] private float defaultTransitionTime = 0.5f;
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
    }

    public void EnableSlowMotion(float targetSpeed, float duration)
    {
        SetAnimator();
        DOTween.To(() => _animator.speed, x => _animator.speed = x, targetSpeed, defaultTransitionTime).onComplete = () => DOTween.To(() => _animator.speed, x => _animator.speed = x, 1f, defaultTransitionTime).SetDelay(duration);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) EnableSlowMotion(0.1f, 3f);
    }

    private void SetAnimator()
    {
        if (_animator == null || !_animator.isActiveAndEnabled)
        {
            if (GetComponent<Animator>() != null)
            {
                _animator = GetComponent<Animator>();
            }
            else
            {
                _animator = GetComponent<AvatarSpawner>().Animator;
            }
        }
    }
}
