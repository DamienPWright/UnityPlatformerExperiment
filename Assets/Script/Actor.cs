using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour {
    //animator
    public string currentAnimation = "";
    public Animator _animator; //Assign this in the Unity editor manually.

    //references
    public Rigidbody2D _rigidbody;
    public Collider2D _collider;
    public Transform _transform;
    public Sprite _sprite;
    public AnimationMonitor animationMonitor;
    public AttackManager _attack_manager;

    public Vector2 stored_velocity;
    public float stored_animspeed;

    public int hitstop_frames = 0;
    public bool hitstop = false;

    public virtual void updateStat(string statname)
    {

    }

    public virtual void applyImpulse(int index)
    {

    }

    public virtual void applyControlledImpulse(int index)
    {
        
    }

    public void ApplyHitStop(int frames)
    {
        if (hitstop)
        {
            return;
        }

        hitstop = true;
        hitstop_frames = frames;
        if(_rigidbody != null)
        {
            stored_velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y);
            _rigidbody.velocity = new Vector2(0, 0);
            _rigidbody.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        }
        
        stored_animspeed = _animator.speed;
        _animator.speed = 0;
    }

    protected void ProcessHitStop()
    {
        if (!hitstop)
        {
            return;
        }

        if (hitstop_frames > 0)
        {
            hitstop_frames--;
        }
        else
        {
            ResetHitStop();
            hitstop = false;
        }
    }

    public void ResetHitStop()
    {
        if (_rigidbody != null)
        {
            _rigidbody.velocity = new Vector2(stored_velocity.x, stored_velocity.y);
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        _animator.speed = stored_animspeed;
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

    public void setAnimation(int anim)
    {
        //Debug.Log("Setting Anim to : " + anim);
        _animator.SetInteger("PlayAnim", anim);
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
