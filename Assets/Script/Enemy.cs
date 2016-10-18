using UnityEngine;
using System.Collections;
using System;

public abstract class Enemy : Actor, IAttackableActor, IControllableActor
{
    public bool immortal = false;   //Cannot die, but reacts to hits
    public bool invulnerable = false;   //immune to everything
    public bool is_dead = false;
    public bool can_knockback = false;
    public bool can_launch = false;
    public bool can_knockdown = false;
    public bool super_armor = false;    //immune to all control effects, usually temporarily.
    
    public int maxHP = 0;
    public int curHP = 0;
    public FiniteStateMachine fsm;

    void Awake()
    {
        fsm = new FiniteStateMachine();
    }

    void Update()
    {
        fsm.Update();
        UpdateAnimator();
        ProcessHitStop();
    }

    public virtual void takeDamage(int damage)
    {
        if (invulnerable)
        {
            return;
        }

        if (!immortal)
        {
            //Debug.Log("Enemy took damage!");
            curHP -= damage;
            checkHealth();
        }

        damageReaction();
    }

    public abstract void onDeath();

    public abstract void damageReaction();

    void checkHealth()
    {
        if(curHP <= 0)
        {
            is_dead = true;
            onDeath();
        }
    }

    //IControllableActor
    public void jump()
    {
        throw new NotImplementedException();
    }

    public void jump_release()
    {
        throw new NotImplementedException();
    }

    public void attack()
    {
        throw new NotImplementedException();
    }

    public void attack_release()
    {
        throw new NotImplementedException();
    }

    public void move(float axis)
    {
        throw new NotImplementedException();
    }

    public void attack2()
    {
        throw new NotImplementedException();
    }

    public void attack2_release()
    {
        throw new NotImplementedException();
    }
}
