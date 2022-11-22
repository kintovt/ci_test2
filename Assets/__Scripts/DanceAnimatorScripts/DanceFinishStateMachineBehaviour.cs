using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceFinishStateMachineBehaviour : StateMachineBehaviour
{
    DanceEvents danceEvents;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("Sequence"))
        {
            danceEvents = animator.transform.parent.gameObject.GetComponent<DanceEvents>();
            danceEvents.OnDanceStarted.Invoke();
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("Sequence") && danceEvents)
        {
            danceEvents.OnDanceUpdated.Invoke(stateInfo.normalizedTime);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("Sequence") && danceEvents)
        {
            danceEvents.OnDanceFinished.Invoke();
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
