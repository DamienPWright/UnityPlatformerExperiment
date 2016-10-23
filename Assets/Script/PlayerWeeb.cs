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
        parry_success,
        attack_special
    }
	// Use this for initialization
	void Start () {
        InitialiseCommon();
        state_idle = new PlayerWeebIdle(fsm, this);
        state_move = new PlayerWeebMove(fsm, this);
        state_airborn = new PlayerWeebAirborn(fsm, this);
        state_attack = new PlayerWeebAttack(fsm, this);
        state_airattack = new PlayerWeebAirAttack(fsm, this);

        fsm.ChangeState(state_idle);

        attack_combo_max = 2;

        impulses = new Vector2[]
        {
            new Vector2(0.0f, 5.0f),
            new Vector2(5.0f, 0.0f),
            new Vector2(15.0f, 0.0f),
            new Vector2(5.0f, 15.0f)
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

        if (_actor.attack_pressed && _actor._attack_manager.canDoANormalAttack())
        {
            _fsm.ChangeState(_actor.state_attack);
            return;
        }

        if (_actor.attack2_pressed && _actor._attack_manager.canDoASpecialAttack())
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

        if (_actor.attack_pressed && _actor._attack_manager.canDoANormalAttack())
        {
            _fsm.ChangeState(_actor.state_attack);
            return;
        }

        if (_actor.attack2_pressed && _actor._attack_manager.canDoASpecialAttack())
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

        if (_actor.attack_pressed && _actor._attack_manager.canDoAirNormalAttack())
        {
            _fsm.ChangeState(_actor.state_airattack);
            return;
        }

        if (_actor.attack2_pressed && _actor._attack_manager.canDoAirSpecialAttack())
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
        _actor.animationMonitor.reset();    
        _actor.setToAttackAccel();          
        _actor.is_attacking = true;
        
        if ((_actor.attack_pressed)){
            _actor._attack_manager.doNormalAttack();
        }else if ((_actor.attack2_pressed))
        {
            _actor._attack_manager.doSpecialAttack();
        }
        _actor.setAnimation(_actor._attack_manager.getAttackAnim());
        
    }

    public override void OnExit()
    {
        _actor.is_attacking = false;        
    }

    public override void Update()
    {
        if (_actor.animationMonitor.isAnimationComplete())
        {
            _fsm.ChangeState(_actor.state_idle);
            _actor._attack_manager.resetAttackCombo();
            return;
        }

        if (_actor.attack_pressed && _actor.animationMonitor.isInterruptable())
        {
            if (_actor._attack_manager.canDoANormalAttack())
            {
                _fsm.ChangeState(_actor.state_attack);
                _actor._attack_manager.endAttack();
                return;
            }
            
        }

        if (_actor.attack2_pressed && _actor.animationMonitor.isInterruptable())
        {
            if (_actor._attack_manager.canDoASpecialAttack())
            {
                _fsm.ChangeState(_actor.state_attack);
                _actor._attack_manager.endAttack();
            }
        }
    }

    public override void FixedUpdate()
    {
        if (_actor._attack_manager.isXinputLocked())
        {
            _actor.Horizontal_Movement(0.0f);
        }
        else
        {
            _actor.Horizontal_Movement(_actor.hor_move_axis);
        }

        if (_actor._attack_manager.isJumpInputLocked())
        {
            _actor.Vertical_Movement(false);
        }
        else
        {
            _actor.Vertical_Movement(_actor.jump_pressed);
        }
        
    }
}

public class PlayerWeebAirAttack : FSM_State
{
    PlayerWeeb _actor;
    int special_counter = 0;
    int special_counter_max = 3;

    public PlayerWeebAirAttack(FiniteStateMachine fsm, PlayerWeeb playerWeeb) : base(fsm)
    {
        _actor = playerWeeb;
    }

    public override void OnEnter()
    {
        //Debug.Log("Air Attack state entered");
        _actor.animationMonitor.reset();
        _actor.setToAirAttackAccel();

        _actor.animationMonitor.reset();
        //_actor.setToAttackAccel();
        _actor.is_attacking = true;

        if ((_actor.attack_pressed))
        {
            _actor._attack_manager.doAirNormalAttack();
        }
        else if ((_actor.attack2_pressed))
        {
            _actor._attack_manager.doAirSpecialAttack();
        }
        _actor.setAnimation(_actor._attack_manager.getAttackAnim());

        /*
        //General method for limited vertical impulse.
        float ycomp = 0.0f;

        if (!(_actor._rigidbody.velocity.y >= 0)) //do not apply an impulse if you're already moving up
        {
            ycomp = Mathf.Min(-_actor._rigidbody.velocity.y, _actor.impulses[0].y);
            _actor._rigidbody.AddForce(new Vector2(0.0f, ycomp - _actor._rigidbody.velocity.y), ForceMode2D.Impulse);
        }
        
        _actor.is_attacking = true;
        if (_actor.attack_pressed)
        {
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
            special_counter++;
            if(special_counter >= special_counter_max)
            {
                special_counter = 0;
            }
        }
        else if(_actor.attack2_pressed){
            _actor.setAnimation((int)PlayerWeeb.AnimType.air_attack_special);
            _actor.jump_pressed = false; //NEED ANOTHER WAY FOR THIS
        }
        _actor.AdvanceAttackCombo();
        */
    }

    public override void OnExit()
    {
        _actor.is_attacking = false;
    }

    public override void Update()
    {
        if (_actor.attack_pressed && _actor.animationMonitor.isInterruptable())
        {
            if (_actor._attack_manager.canDoAirNormalAttack())
            {
                _fsm.ChangeState(_actor.state_airattack);
                _actor._attack_manager.endAttack();
                return;
            }
        }

        if (_actor.attack2_pressed && _actor.animationMonitor.isInterruptable())
        {
            if (_actor._attack_manager.canDoASpecialAttack())
            {
                _fsm.ChangeState(_actor.state_airattack);
                _actor._attack_manager.endAttack();
            }
        }

        if (_actor.isOnGround)
        {
            _fsm.ChangeState(_actor.state_idle);
            _actor._attack_manager.endAttack();
            _actor._attack_manager.resetAttackCombo();
        }
    }

    public override void FixedUpdate()
    {
        if (_actor._attack_manager.isXinputLocked())
        {
            _actor.Horizontal_Movement(0.0f);
        }
        else
        {
            _actor.Horizontal_Movement(_actor.hor_move_axis);
        }

        if (_actor._attack_manager.isJumpInputLocked())
        {
            _actor.Vertical_Movement(false);
        }
        else
        {
            _actor.Vertical_Movement(_actor.jump_pressed);
        }
    }
}