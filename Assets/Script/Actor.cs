using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour {
    //animator
    public string currentAnimation = "";
    public Animator _animator; //Assign this in the Unity editor manually.

    public virtual void updateStat(string statname)
    {

    }

    public void setAnimation(string anim)
    {
        //Debug.Log("Setting Anim to : " + anim);
        if (currentAnimation == anim)
        {

        }
        else
        {
            if (currentAnimation != "")
            {
                _animator.ResetTrigger(anim);
            }
            _animator.SetTrigger(anim);
            currentAnimation = anim;
        }

    }

    public void resetAnimation(string anim)
    {
        _animator.ResetTrigger(anim);
    }

    public void setAnimationFloatVariable(string variable, float value)
    {
        _animator.SetFloat(variable, value);
    }

    public virtual void UpdateAnimator()
    {
        //use this to update animator variables that need updating in realtime for whatever reason.
    }

    public bool getCurrentAnimationComplete()
    {
        return (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !_animator.IsInTransition(0));
    }
}
