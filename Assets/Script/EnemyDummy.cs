using UnityEngine;
using System.Collections;
using System;

public class EnemyDummy : Enemy {

    public DummyIdle dummyIdle;
    public DummyDamage dummyDamage;
    public DummyDie dummyDie;

    public override void damageReaction()
    {
        //Debug.Log("Hit the dummy!");
        if (!is_dead)
        {
            fsm.ChangeState(dummyDamage);
        }
        
    }

    public override void onDeath()
    {
        fsm.ChangeState(dummyDie);
    }

    // Use this for initialization
    void Start () {
        //immortal = false;
        //_animator = GetComponentInChildren<Animator>();
        dummyIdle = new DummyIdle(fsm, this);
        dummyDamage = new DummyDamage(fsm, this);
        dummyDie = new DummyDie(fsm, this);
        curHP = 4;
        maxHP = 4;

        animationMonitor = GetComponentInChildren<AnimationMonitor>();
	}

}

public class DummyIdle : FSM_State
{
    EnemyDummy _dummy;

    public DummyIdle(FiniteStateMachine fsm, EnemyDummy dummy) : base(fsm)
    {
        _dummy = dummy;
    }

    public override void FixedUpdate()
    {
        
    }

    public override void OnEnter()
    {
        _dummy.setAnimation("idle");
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        
    }
}

public class DummyDamage : FSM_State
{
    EnemyDummy _dummy;

    public DummyDamage(FiniteStateMachine fsm, EnemyDummy dummy) : base(fsm)
    {
        _dummy = dummy;
    }

    public override void FixedUpdate()
    {

    }

    public override void OnEnter()
    {
        _dummy.setAnimation("hurt");
       
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (_dummy.animationMonitor.isAnimationComplete())
        {
            _fsm.ChangeState(_dummy.dummyIdle);
            _dummy.animationMonitor.reset();
        }
    }
}

public class DummyDie : FSM_State
{
    EnemyDummy _dummy;

    public DummyDie(FiniteStateMachine fsm, EnemyDummy dummy) : base(fsm)
    {
        _dummy = dummy;
    }

    public override void FixedUpdate()
    {

    }

    public override void OnEnter()
    {
        _dummy.setAnimation("die");
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }
}