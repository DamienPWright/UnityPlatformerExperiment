using UnityEngine;
using System.Collections;
using System;

public class PlayerWeeb : PlayerCommon {

    public PlayerWeebIdle state_idle;
    public PlayerWeebMove state_move;
    public PlayerWeebAirborn state_airborn;
    public PlayerWeebAttack state_attack;
    public PlayerWeebAirAttack state_airattack;

    public enum AnimType
    {
        idle=0,
        run,
        knockback_rise,
        knockback_fall,
        knockback_grounded,
        jump_rise,
        jump_fall,
        air_attack_1,
        air_attack_2,
        air_attack_special,
        attack_1,
        attack_2,
        parry_start,
        parry_success
    }
	// Use this for initialization
	void Start () {
        int test = (int)AnimType.air_attack_1;
        InitialiseCommon();
        Debug.Log(test);

        state_idle = new PlayerWeebIdle(fsm, this);
        state_move = new PlayerWeebMove(fsm, this);
        state_airborn = new PlayerWeebAirborn(fsm, this);
        state_attack = new PlayerWeebAttack(fsm, this);
        state_airattack = new PlayerWeebAirAttack(fsm, this);

        fsm.ChangeState(state_idle);

        attack_combo_max = 2;

        impulses = new Vector2[]
        {
            new Vector2(0.0f, 8.0f),
            new Vector2(3.0f, 0.0f)
        };
    }
	
}

public class PlayerWeebIdle : FSM_State
{
    PlayerWeeb _actor;

    public PlayerWeebIdle(FiniteStateMachine fsm, PlayerWeeb playerWeeb) : base(fsm)
    {
        _actor = playerWeeb;
    }

    public override void OnEnter()
    {
        //Debug.Log("Idle state entered");
        _actor.setAnimation((int)PlayerWeeb.AnimType.idle);
        _actor.setToGroundAccel();
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        _actor.Handle_inputs();

        if (_actor.hor_move_axis != 0)
        {
            _fsm.ChangeState(_actor.state_move);
            return;
        }

        if (!_actor.isOnGround)
        {
            _fsm.ChangeState(_actor.state_airborn);
            return;
        }

        if (_actor.attack_pressed && !_actor.jump_pressed)
        {
            _fsm.ChangeState(_actor.state_attack);
            return;
        }
    }

    public override void FixedUpdate()
    {
        _actor.Horizontal_Movement(_actor.hor_move_axis);
        _actor.Vertical_Movement(_actor.jump_pressed);
    }
}

public class PlayerWeebMove : FSM_State
{
    PlayerWeeb _actor;

    public PlayerWeebMove(FiniteStateMachine fsm, PlayerWeeb playerWeeb) : base(fsm)
    {
        _actor = playerWeeb;
    }

    public override void OnEnter()
    {
        //Debug.Log("Moving state entered");
        _actor.setAnimation((int)PlayerWeeb.AnimType.run);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        _actor.Handle_inputs();


        if (_actor.hor_move_axis == 0)
        {
            _fsm.ChangeState(_actor.state_idle);
            return;
        }

        if (!_actor.isOnGround)
        {
            _fsm.ChangeState(_actor.state_airborn);
            return;
        }

        if (_actor.attack_pressed)
        {
            _fsm.ChangeState(_actor.state_attack);
            return;
        }
    }

    public override void FixedUpdate()
    {
        _actor.Horizontal_Movement(_actor.hor_move_axis);
        _actor.Vertical_Movement(_actor.jump_pressed);
    }
}

public class PlayerWeebAirborn : FSM_State
{
    PlayerWeeb _actor;

    public PlayerWeebAirborn(FiniteStateMachine fsm, PlayerWeeb playerWeeb) : base(fsm)
    {
        _actor = playerWeeb;
    }

    public override void OnEnter()
    {
        //Debug.Log("Airborn state entered");
        _actor.setToAirAccel();
    }

    public override void OnExit()
    {
        //_actor.setToGroundAccel();
    }

    public override void Update()
    {
        _actor.Handle_inputs();

        if(_actor._rigidbody.velocity.y > 0)
        {
            _actor.setAnimation((int)PlayerWeeb.AnimType.jump_rise);
        }
        else
        {
            _actor.setAnimation((int)PlayerWeeb.AnimType.jump_fall);
        }

        if (_actor.isOnGround)
        {
            _fsm.ChangeState(_actor.state_idle);
            return;
        }

        if (_actor.attack_pressed)
        {
            _fsm.ChangeState(_actor.state_airattack);
            return;
        }
    }

    public override void FixedUpdate()
    {
        _actor.Horizontal_Movement(_actor.hor_move_axis);
        _actor.Vertical_Movement(_actor.jump_pressed);
    }
}

public class PlayerWeebAttack : FSM_State
{
    PlayerWeeb _actor;

    public PlayerWeebAttack(FiniteStateMachine fsm, PlayerWeeb playerWeeb) : base(fsm)
    {
        _actor = playerWeeb;
    }

    public override void OnEnter()
    {
        //Debug.Log("Attack state entered");
        //Debug.Log("Attack combo count: " + _actor.getAttackComboCount());
        _actor.setToAttackAccel();
        _actor.is_attacking = true;
        switch (_actor.getAttackComboCount())
        {
            case 0:
                _actor.setAnimation((int)PlayerWeeb.AnimType.attack_1);
                break;
            case 1:
                _actor.setAnimation((int)PlayerWeeb.AnimType.attack_2);
                break;
            default:
                break;
                //do nothing
        }

    }

    public override void OnExit()
    {
        //_actor.setToGroundAccel();
        //start combo counter.
        _actor.AdvanceAttackCombo();
        //Debug.Log("Attack combo count: " + _actor.getAttackComboCount());
        _actor.is_attacking = false;
        _actor.animationMonitor.reset();
    }

    public override void Update()
    {
        _actor.Handle_inputs();

        //if (attack_counter > _actor.attack_time)
        if (_actor.animationMonitor.isAnimationComplete())
        {
            _fsm.ChangeState(_actor.state_idle);
            _actor.animationMonitor.reset();
            return;
        }

        if (_actor.attack_pressed && _actor.animationMonitor.isInterruptable())
        {
            _fsm.ChangeState(_actor.state_attack);
            _actor.animationMonitor.reset();
            return;
        }
    }

    public override void FixedUpdate()
    {
        _actor.Horizontal_Movement(0.0f);
        _actor.Vertical_Movement(false);
    }
}

public class PlayerWeebAirAttack : FSM_State
{
    PlayerWeeb _actor;

    public PlayerWeebAirAttack(FiniteStateMachine fsm, PlayerWeeb playerWeeb) : base(fsm)
    {
        _actor = playerWeeb;
    }

    public override void OnEnter()
    {
        //Debug.Log("Air Attack state entered");
        _actor.setToAirAttackAccel();

        float ycomp = 0.0f;

        if (!(_actor._rigidbody.velocity.y >= 0)) //do not apply an impulse if you're already moving up
        {
            ycomp = Mathf.Min(-_actor._rigidbody.velocity.y, _actor.impulses[0].y);
            _actor._rigidbody.AddForce(new Vector2(0.0f, ycomp - _actor._rigidbody.velocity.y), ForceMode2D.Impulse);
        }

        _actor.is_attacking = true;

        switch (_actor.getAttackComboCount())
        {
            case 0:
                _actor.setAnimation((int)PlayerWeeb.AnimType.air_attack_1);
                break;
            case 1:
                _actor.setAnimation((int)PlayerWeeb.AnimType.air_attack_2);
                break;
            default:
                break;
            //do nothing
        }
    }

    public override void OnExit()
    {
        //start combo counter.
        _actor.AdvanceAttackCombo();
        _actor.animationMonitor.reset();
        _actor.is_attacking = false;
    }

    public override void Update()
    {
        /*
        if (_actor.animationMonitor.isAnimationComplete())
        {
            _fsm.ChangeState(_actor.state_airborn);
            _actor.animationMonitor.reset();
            return;
        }
        */
        if (_actor.attack_pressed && _actor.animationMonitor.isInterruptable())
        {
            _fsm.ChangeState(_actor.state_airattack);
            _actor.animationMonitor.reset();
            return;
        }

        if (_actor.isOnGround)
        {
            _fsm.ChangeState(_actor.state_idle);
            
        }
    }

    public override void FixedUpdate()
    {
        _actor.Horizontal_Movement(0.0f);
        _actor.Vertical_Movement(_actor.jump_pressed);
    }
}