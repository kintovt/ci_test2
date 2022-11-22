using System.Collections;
using UnityEngine;

public class AnimationMockWithSpawner : AnimationMock
{
    [SerializeField] private AvatarSpawner avatarSpawner;

    protected override void OnEnable()
    {
        _animator = avatarSpawner.Animator;
        base.OnEnable();   
    }

    protected override IEnumerator PlaySequence()
    {
        _animator.SetTrigger("3");
        _animator.SetBool("Bool3", true);
        yield return new WaitForSeconds(3.85f);
        _animator.SetBool("Bool3", false);
        _animator.SetTrigger("4");
        _animator.SetBool("Bool4", true);
        yield return new WaitForSeconds(5.45f);
        _animator.SetBool("Bool4", false);
        _animator.SetBool("Bool3", true);
        _animator.SetTrigger("3");
        yield return new WaitForSeconds(3.85f);
        _animator.SetBool("Bool3", false);
        _animator.SetTrigger("6");
        _animator.SetBool("Bool6", true);
        yield return new WaitForSeconds(18.25f);
        _animator.SetBool("Bool6", false);
        _animator.SetTrigger("4");
        _animator.SetBool("Bool4", true);
        yield return new WaitForSeconds(5.45f);
        _animator.SetBool("Bool4", false);
        _animator.SetTrigger("3");
        _animator.SetBool("Bool3", true);
        yield return new WaitForSeconds(3.85f);
        _sequenceFinishEvent.Invoke();
        _animator.SetBool("Bool3", false);
        _animator.SetTrigger("Talking");
    }
}