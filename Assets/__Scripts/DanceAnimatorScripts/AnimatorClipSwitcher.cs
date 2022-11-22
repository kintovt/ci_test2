using UnityEngine;
using _Scripts.UI;

public class AnimatorClipSwitcher : MonoBehaviour
{
    [SerializeField] private MotionCardData[] _danceAnimations;
    [SerializeField] private Animator _animator;
    public AnimationClip[] clips;
    private AnimatorOverrideController _animatorOverrideController;
    public ViewController viewController;
    private int _currentClipCounter;

    private void Awake()
    {
        Debug.Log("AnimatorClipSwitcher.Awake()");
        GetAnimatorOverrideController();
    }

    public void StartPlaying()
    {
        _currentClipCounter = 0;
        SetClipFromArrayByIndex(_currentClipCounter);
        viewController.battleScreen.cardsAnimator.ShowCardProgress(_currentClipCounter);
    }

    public void PlayNextClip()
    {
        viewController.battleScreen.cardsAnimator.HideCardProgress(_currentClipCounter);
        _currentClipCounter++;
        if (_currentClipCounter >= _danceAnimations.Length)
        {
            _currentClipCounter = 0;
            SetIdleClip(clips[2]);
            viewController.danceController.FinishDance();
            viewController.battleScreen.cardsAnimator.animator.speed = 0.29f;
            return;
        }
        viewController.battleScreen.cardsAnimator.ShowCardProgress(_currentClipCounter);
        SetClipFromArrayByIndex(_currentClipCounter);
    }

    public void SetDanceAnimations(MotionCardData[] danceAnimations)
    {
        _danceAnimations = danceAnimations;
    }

    public void SetClipFromArrayByIndex(int index)
    {
        //if (GetAnimatorOverrideController())
        //{
            _animatorOverrideController["Dance"] = _danceAnimations[index].clip;
        //}
    }

    public void SetClipFromResourcesByName(string name)
    {
        if (GetAnimatorOverrideController())
        {
            _animatorOverrideController["Dance"] = Resources.Load<AnimationClip>(name);
        }
    }

    public void SetRandomClipFromArray()
    {
        if (GetAnimatorOverrideController())
        {
            _animatorOverrideController["Dance"] = _danceAnimations[Random.Range(0, _danceAnimations.Length)].clip;
        }
    }

    public void SetDanceClip(AnimationClip clip)
    {
       if (GetAnimatorOverrideController())
       {
            _animatorOverrideController["Dance"] = clip;
            _animator.SetTrigger("DanceTrigger");
        }
    }

    public void SetIdleClip(AnimationClip clip)
    {
        //if (GetAnimatorOverrideController())
        //{
            _animatorOverrideController["Idle"] = clip;
            _animator.SetTrigger("IdleTriggerSmooth");
        //}
    }

    public bool GetAnimatorOverrideController()
    {
        if (_animator == null || !_animator.isActiveAndEnabled)
        {
            _animator = GetComponent<AvatarSpawner>().Animator;
        }
        if (_animator.runtimeAnimatorController == null)
        {
            Debug.Log($"no runtime animator controller given to animator {_animator.gameObject.name}");
            return false;
        }
        else
        {
            _animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _animatorOverrideController;
            return true;
        }
    }
}
