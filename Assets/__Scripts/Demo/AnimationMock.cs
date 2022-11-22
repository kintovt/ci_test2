using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationMock : MonoBehaviour
{
    public float TotalAnimationTime { get => _totalAnimationTime; }
    [SerializeField] private float _totalAnimationTime = 40.7f;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected UnityEvent _sequenceFinishEvent;
    
    protected virtual void OnEnable()
    {
        StartCoroutine(PlaySequence());
    }

    protected virtual IEnumerator PlaySequence()
    {
        _animator.SetBool("IsIdle", false);
        _animator.Play("1");
        yield return new WaitForSeconds(3.85f);
        _animator.Play("2");
        yield return new WaitForSeconds(5.45f);
        _animator.Play("1");
        yield return new WaitForSeconds(3.85f);
        _animator.Play("3");
        yield return new WaitForSeconds(18.25f);
        _animator.Play("2");
        yield return new WaitForSeconds(5.45f);
        _animator.Play("1");
        yield return new WaitForSeconds(3.85f);
        _sequenceFinishEvent.Invoke();
        _animator.Play("Idle");
        _animator.SetBool("IsIdle", true);
    }
}
