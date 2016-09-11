using UnityEngine;
using System.Collections;
using System;

public class TestPlayer : MonoBehaviour, IControllableActor
{

    //states
    FiniteStateMachine fsm;
    public TestPlayerIdle state_idle;
    public TestPlayerMove state_move;
    public TestPlayerAirborn state_airborn;
    public TestPlayerAttack state_attack;
    public TestPlayerAirAttack state_airattack;

    //references
    public Rigidbody2D _rigidbody;
    Collider2D _collider;
    Transform _transform;
    Sprite _sprite;
    Animator _animator;

    //movement variables
    const float GRAVITY_SCALE = 5.0f;
    const float AIR_GRAVITY_SCALE = 2.0f;

    const float MOVE_SPEED = 8.0f;
    const float AIR_ACCEL_TIME = 0.5f;
    const float GROUND_ACCEL_TIME = 0.2f;
    const float GROUND_DECEL_TIME = 0.1f;
    const float FALLSPEED = 30;
    const float JUMPSPEED = 15;

    public float gravity_scale = GRAVITY_SCALE;
    public float accel_time = GROUND_ACCEL_TIME;
    public float decel_time = GROUND_DECEL_TIME;
    public float fallspeed = FALLSPEED; // is this needed?
    public float jumpspeed = JUMPSPEED;
    public float movespeed = MOVE_SPEED;

    public float hor_move_axis = 0.0f;

    //control variables
    public bool isOnGround = true;
    public bool jump_pressed = false;
    public bool jump_locked = false;
    public bool attack_pressed = false;
    public bool attack_locked = false;
    public float attack_combo_window = 0.4f; //time in which the combo is held after the current attack ends. 
    public float attack_combo_timer = 0.0f; // No longer used. Attack times are decided by animations now.
    public byte attack_combo_count = 0; 
    byte attack_combo_max = 3; //set by weapon later
    public float attack_time = 0.4f; //set by weapon later
    public bool is_attacking = false;
    float distToGround;
    string currentAnimation = "";

    // Use this for initialization
    void Start () {
        fsm = new FiniteStateMachine();
        state_idle = new TestPlayerIdle(fsm, this);
        state_move = new TestPlayerMove(fsm, this);
        state_airborn = new TestPlayerAirborn(fsm, this);
        state_attack = new TestPlayerAttack(fsm, this);
        state_airattack = new TestPlayerAirAttack(fsm, this);

        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _transform = GetComponent<Transform>();
        _sprite = GetComponent<Sprite>();
        _animator = GetComponentInChildren<Animator>();

        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        distToGround = _collider.bounds.extents.y;

        fsm.ChangeState(state_idle);
	}
	
	// Update is called once per frame
	void Update () {
        fsm.Update();
        AttackComboHandler();
        UpdateAnimator();
	}

    void FixedUpdate()
    {
        isOnGround = IsGrounded();
        fsm.FixedUpdate();
        
    }

    bool IsGrounded()
    {
        Vector3 extent_A = new Vector3(_collider.bounds.extents.x, 0.0f, 0.0f);
        Vector3 extent_B = new Vector3(-_collider.bounds.extents.x, 0.0f, 0.0f);

        Debug.DrawRay(transform.position + extent_A, -Vector3.up * (distToGround + 0.1f), Color.green);
        Debug.DrawRay(transform.position + extent_B, -Vector3.up * (distToGround + 0.1f), Color.green);
        bool detector_A = Physics2D.Raycast(transform.position + extent_A, -Vector3.up, distToGround + 0.1f, LayerMask.GetMask("env_solid"));
        bool detector_B = Physics2D.Raycast(transform.position + extent_B, -Vector3.up, distToGround + 0.1f, LayerMask.GetMask("env_solid"));
        return (detector_A || detector_B);
    } 

    public void setToAirAccel()
    {
        accel_time = AIR_ACCEL_TIME;
        decel_time = AIR_ACCEL_TIME;
    }

    public void setToAttackAccel()
    {
        //will need to get these values from attacks in future.
        accel_time = 0.0f;
        decel_time = 0.3f;
    }

    public void setToNormalMoveSpeed()
    {
        movespeed = MOVE_SPEED;
    }

    public void setToGroundAccel()
    {
        accel_time = GROUND_ACCEL_TIME;
        decel_time = GROUND_DECEL_TIME;
    }

    public void AdvanceAttackCombo()
    {
        attack_combo_count++;
        attack_combo_timer = 0;
        if(attack_combo_count >= attack_combo_max)
        {
            ResetAttackCombo();
        }
    }

    public void ResetAttackCombo()
    {
        attack_combo_count = 0;
    }

    void AttackComboHandler()
    {
        if (attack_combo_timer >= attack_combo_window)
        {
            ResetAttackCombo();
        }
        else if(!is_attacking)
        {
            attack_combo_timer += Time.deltaTime;
        }
    }

    public byte getAttackComboCount()
    {
        return attack_combo_count;
    }

    public void setGravityScale(float grav)
    {
        _rigidbody.gravityScale = grav;
    }

    public void resetGravityScale()
    {
        _rigidbody.gravityScale = GRAVITY_SCALE;
    }

    public void suspendGravity()
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
    }

    public void setAnimation(string anim)
    {
        Debug.Log("Setting Anim to : " + anim);
        if (currentAnimation == anim)
        {
            
        }
        else
        {
            if(currentAnimation != "") {
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

    void UpdateAnimator()
    {
        _animator.SetBool("IsOnGround", isOnGround);
        _animator.SetBool("IsAttacking", is_attacking);
    }

    public bool getCurrentAnimationComplete()
    {
        return (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !_animator.IsInTransition(0));
    }

    public void Handle_inputs()
    {
        hor_move_axis = Input.GetAxisRaw("Horizontal");
    }

    public void Horizontal_Movement(float direction)
    { 
        if (direction != 0)
        {
            float movement = direction * (movespeed * Time.fixedDeltaTime) / accel_time;
            if(direction > 0)
            {
                _rigidbody.velocity = new Vector2(Mathf.Min(_rigidbody.velocity.x + movement, movespeed), _rigidbody.velocity.y);
                _transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else
            {
                _rigidbody.velocity = new Vector2(Mathf.Max(_rigidbody.velocity.x + movement, -movespeed), _rigidbody.velocity.y);
                _transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            }
            
        }
        else
        {
            float movement = (movespeed * Time.fixedDeltaTime) / decel_time;

            if (_rigidbody.velocity.x > 0)
            {
                movement *= -1;
            }
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x + movement, _rigidbody.velocity.y);
            
            if(_rigidbody.velocity.x > -movement * 2 && _rigidbody.velocity.x < movement * 2)
            {
                _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
            }
            
        }
    }

    public void Vertical_Movement(bool jumping)
    {
        if (isOnGround && jumping)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpspeed);
        }

        if (_rigidbody.velocity.y > 0 && jumping)
        {
            //lower gravity's effect
            _rigidbody.gravityScale = AIR_GRAVITY_SCALE;
        }

        if (_rigidbody.velocity.y < 0 || !jumping)
        {
            //lower gravity's effect
            _rigidbody.gravityScale = GRAVITY_SCALE;
            //disable the jump key
            //may want an additional condition to allow for multi-jump
            jump_locked = true;
            
        }

        if(_rigidbody.velocity.y == 0 || isOnGround)
        {
            jump_locked = false;
        }
    }

    //Interface

    public void jump()
    {
        if (!jump_locked)
        {
            jump_pressed = true;
        }
    }

    public void jump_release()
    {
        jump_pressed = false;
    }

    public void move()
    {
        throw new NotImplementedException();
    }

    public void attack()
    {
        attack_pressed = true;
    }

    public void attack_release()
    {
        attack_pressed = false;
    }
}

public class TestPlayerIdle : FSM_State
{
    TestPlayer _actor;

    public TestPlayerIdle(FiniteStateMachine fsm, TestPlayer testplayer) : base(fsm)
    {
        _actor = testplayer;
    }

    public override void OnEnter()
    {
        Debug.Log("Idle state entered");
        _actor.setAnimation("BeginIdle");
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        _actor.Handle_inputs();

        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            _fsm.ChangeState(_actor.state_move);
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

public class TestPlayerMove : FSM_State
{
    TestPlayer _actor;

    public TestPlayerMove(FiniteStateMachine fsm, TestPlayer testplayer) : base(fsm)
    {
        _actor = testplayer;
    }

    public override void OnEnter()
    {
        Debug.Log("Moving state entered");
        _actor.setAnimation("BeginRun");
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        _actor.Handle_inputs();
 

        if (Input.GetAxisRaw("Horizontal") == 0)
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

public class TestPlayerAirborn : FSM_State
{
    TestPlayer _actor;

    public TestPlayerAirborn(FiniteStateMachine fsm, TestPlayer testplayer) : base(fsm)
    {
        _actor = testplayer;
    }

    public override void OnEnter()
    {
        Debug.Log("Airborn state entered");
        _actor.setToAirAccel();
        _actor.setAnimation("BeginJump");
    }

    public override void OnExit()
    {
        _actor.setToGroundAccel();
    }

    public override void Update()
    {
        _actor.Handle_inputs();

        _actor.setAnimationFloatVariable("PlayerYVelocity", _actor._rigidbody.velocity.y);

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

public class TestPlayerAttack : FSM_State
{
    TestPlayer _actor;

    float attack_counter = 0.0f;

    public TestPlayerAttack(FiniteStateMachine fsm, TestPlayer testplayer) : base(fsm)
    {
        _actor = testplayer;
    }

    public override void OnEnter()
    {
        Debug.Log("Attack state entered");
        Debug.Log("Attack combo count: " + _actor.getAttackComboCount());
        _actor.setToAttackAccel();
        _actor.is_attacking = true;
        switch (_actor.getAttackComboCount())
        {
            case 0:
                _actor.setAnimation("BeginAttack1");
                break;
            case 1:
                _actor.setAnimation("BeginAttack2");
                break;
            case 2:
                _actor.setAnimation("BeginAttack3");
                break;
            default:
                break;
                //do nothing
        }

    }

    public override void OnExit()
    {
        _actor.setToGroundAccel();
        attack_counter = 0.0f;
        //start combo counter.
        _actor.AdvanceAttackCombo();
        Debug.Log("Attack combo count: " + _actor.getAttackComboCount());
        _actor.is_attacking = false;
    }

    public override void Update()
    {
        _actor.Handle_inputs();

        //if (attack_counter > _actor.attack_time)
        if (_actor.getCurrentAnimationComplete())
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

public class TestPlayerAirAttack : FSM_State
{
    TestPlayer _actor;

    float attack_counter = 0.0f;

    public TestPlayerAirAttack(FiniteStateMachine fsm, TestPlayer testplayer) : base(fsm)
    {
        _actor = testplayer;
    }

    public override void OnEnter()
    {
        Debug.Log("Air Attack state entered");
        _actor.suspendGravity();
        _actor.setGravityScale(0.1f); //This needs to be set by the weapon
        _actor.setToAirAccel();
        _actor.is_attacking = true;
        
        switch (_actor.getAttackComboCount())
        {
            case 0:
                _actor.setAnimation("BeginAttack1");
                break;
            case 1:
                _actor.setAnimation("BeginAttack2");
                break;
            case 2:
                _actor.setAnimation("BeginAttack3");
                break;
            default:
                break;
                //do nothing
        }
        
        //_actor._rigidbody.AddForce(new Vector2(5.0f, 5.0f), ForceMode2D.Impulse);
        //This interestingly seems to add an up-left movement to the attack, though its only on the first frame.
    }

    public override void OnExit()
    {
        _actor.setToGroundAccel();
        attack_counter = 0.0f;
        _actor.resetGravityScale();
        //start combo counter.
        _actor.AdvanceAttackCombo();
        _actor.is_attacking = false;
    }

    public override void Update()
    {
        _actor.Handle_inputs();

        //attack_counter += Time.deltaTime;
        //Debug.Log(_actor.decel_time);

        //if (attack_counter > _actor.attack_time)
        if (_actor.getCurrentAnimationComplete())
        {
            _fsm.ChangeState(_actor.state_airborn);
        }

    }

    public override void FixedUpdate()
    {
        _actor.Horizontal_Movement(0.0f);
        //_actor.Vertical_Movement(_actor.jump_pressed);
    }
}