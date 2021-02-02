using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    #region Attributes

    private Animator animator;

    private const string IDLE_ANIMATION_BOOL = "Idle";
    private const string RUN_ANIMATION_BOOL = "Run";
    private const string DASH_ANIMATION_BOOL = "Dash";
    private const string FALL_ANIMATION_BOOL = "Fall";
    private const string STOP_ANIMATION_BOOL = "Stop";

    #endregion

    #region Animate Functions
  
    public void AnimateIdle()
    {
        Animate(IDLE_ANIMATION_BOOL);
    }

    public void AnimateRun()
    {
        Animate(RUN_ANIMATION_BOOL);
    }

    public void AnimateDash()
    {
        Animate(DASH_ANIMATION_BOOL);
    }

    public void AnimateFall()
    {
        Animate(FALL_ANIMATION_BOOL);
    }

    public void AnimateStop()
    {
        if (!animator.GetBool(IDLE_ANIMATION_BOOL))
        {
            Animate(STOP_ANIMATION_BOOL);
        }
       
    }

    #endregion

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Animate(string boolName)
    {
        DisableOtherAnimations(animator, boolName);
        animator.SetBool(boolName, true);
    }

    private void DisableOtherAnimations(Animator animator, string animation)
    {
        foreach(AnimatorControllerParameter parameter in animator.parameters)
        {
            if(parameter.name != animation)
            {
                animator.SetBool(parameter.name, false);
            }
        }
    }
    
}
