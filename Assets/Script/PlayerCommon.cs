using UnityEngine;
using System.Collections;
using System;

public class PlayerCommon : Actor, IControllableActor{

    //states
    protected FiniteStateMachine fsm;

    

    //movement variables
    const float GRAVITY_SCALE = 5.0f;
    const float AIR_GRAVITY_SCALE = 2.0f;

    const float MOVE_SPEED = 8.0f;
    const float AIR_ACCEL_TIME = 0.5f;
    const float GROUND_ACCEL_TIME = 0.2f;
    const float GROUND_DECEL_TIME = 0.1f;
    const float FALLSPEED = 30;
    const float JUMPSPEED = 15;
    const float JUMPHEIGHT = 5.3f;

    public float gravity_scale = GRAVITY_SCALE;
    public float accel_time = GROUND_ACCEL_TIME;
    public float decel_time = GROUND_DECEL_TIME;
    public float fallspeed = FALLSPEED; // is this needed?
    public float jumpspeed = JUMPSPEED;
    public float movespeed = MOVE_SPEED;

    public float hor_move_axis = 0.0f;

    //control variables
    public bool facing_right = true; //false = left. true = right.
    public bool isOnGround = true;
    public bool jump_pressed = false;
    public bool jump_locked = false;
    public bool attack_pressed = false;
    public bool attack_locked = false;
    public bool attack2_pressed = false;
    public float attack_combo_window = 0.4f; //time in which the combo is held after the current attack ends. 
    public float attack_combo_timer = 0.0f; // No longer used. Attack times are decided by animations now.
    public byte attack_combo_count = 0;
    public byte attack_combo_max = 3; //set by weapon later
    public float attack_time = 0.4f; //set by weapon later
    public bool is_attacking = false;
    float distToGround;

    public Vector2[] impulses =
    {

    };


    //hitbox stuff
    public HitBoxManager hitBoxManager;

    // Use this for initialization
    public void InitialiseCommon()
    {
        fsm = new FiniteStateMachine();

        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _transform = GetComponent<Transform>();
        _sprite = GetComponent<Sprite>();

        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        distToGround = _collider.bounds.extents.y;

        animationMonitor = GetComponentInChildren<AnimationMonitor>();
        _attack_manager = new AttackManager();
        _attack_manager.Initialise();
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
        AttackComboHandler();
        UpdateAnimator();
        ProcessHitStop();
        //attack_manager.Update();
    }

    void FixedUpdate()
    {
        isOnGround = IsGrounded();
        fsm.FixedUpdate();

    }

    void calculateJumpSpeed()
    {
        jumpspeed = Mathf.Sqrt(-2 * JUMPHEIGHT * Physics2D.gravity.y);
        Debug.Log("Jumpspeed set to:" + jumpspeed);
    }

    bool IsGrounded()
    {
        Vector3 extent_A = new Vector3(_collider.bounds.extents.x, 0.0f, 0.0f);
        Vector3 extent_B = new Vector3(-_collider.bounds.extents.x, 0.0f, 0.0f);

        Debug.DrawRay(transform.position + extent_A, -Vector3.up * (distToGround + 0.1f), Color.green);
        Debug.DrawRay(transform.position + extent_B, -Vector3.up * (distToGround + 0.1f), Color.green);
        bool detector_A = Physics2D.Raycast(transform.position + extent_A, -Vector3.up, distToGround + 0.07f, LayerMask.GetMask("env_solid"));
        bool detector_B = Physics2D.Raycast(transform.position + extent_B, -Vector3.up, distToGround + 0.07f, LayerMask.GetMask("env_solid"));
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

    public void setToAirAttackAccel()
    {
        //will need to get these values from attacks in future.
        accel_time = AIR_ACCEL_TIME * 4f;
        decel_time = AIR_ACCEL_TIME * 4f;
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
        //animationMonitor.reset();
        if (attack_combo_count >= attack_combo_max)
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
        else if (!is_attacking)
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


    public override void UpdateAnimator()
    {
        //_animator.SetBool("IsOnGround", isOnGround);
        //_animator.SetBool("IsAttacking", is_attacking);
    }

    public void Handle_inputs()
    {
        //hor_move_axis = Input.GetAxisRaw("Horizontal");
    }

    public void Horizontal_Movement(float direction)
    {
        if (direction != 0)
        {
            float movement = direction * (movespeed * Time.fixedDeltaTime) / accel_time;
            if (direction > 0)
            {
                _rigidbody.velocity = new Vector2(Mathf.Min(_rigidbody.velocity.x + movement, movespeed), _rigidbody.velocity.y);
                _transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                facing_right = true;
            }
            else
            {
                _rigidbody.velocity = new Vector2(Mathf.Max(_rigidbody.velocity.x + movement, -movespeed), _rigidbody.velocity.y);
                _transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                facing_right = false;
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
            
            if (_rigidbody.velocity.x > -movement * 2 && _rigidbody.velocity.x < movement * 2)
            {
                _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
            }
            
            
        }
    }

    public void Vertical_Movement(bool jumping)
    {
        //TODO - Find some way of enforcing explicit pressing of the jump key so holding jump wont make you bounce.
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

        if (_rigidbody.velocity.y == 0 || isOnGround)
        {
            jump_locked = false;
        }

        //cap fallspeed
        if (_rigidbody.velocity.y < -fallspeed)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -fallspeed);
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

    public void move(float axis)
    {
        hor_move_axis = axis;
    }

    public override void applyImpulse(int index)
    {
        Vector2 imp;
        
        //x component

        if (facing_right)
        {
            imp = new Vector2(impulses[index].x, 0);
        }
        else
        {
            imp = new Vector2(impulses[index].x * -1, 0);
        }

        _rigidbody.AddForce(imp, ForceMode2D.Impulse);

        //y component

        //If capping y is needed..it might not be?
        
        if((_rigidbody.velocity.y < 0))
        {
            if ((_rigidbody.velocity.y > imp.y) || (_rigidbody.velocity.y < -imp.y))
            {
                _rigidbody.AddForce(new Vector2(0.0f, -(_rigidbody.velocity.y - imp.y)), ForceMode2D.Impulse);
            }
        }
        
    }

    public override void applyControlledImpulse(int index)
    {

        Vector2 imp;
        float dir = Input.GetAxisRaw("Horizontal");
        float ycomp = 0.0f;
        //Debug.Log(dir);

        if ((hor_move_axis > 0.0f && facing_right) || (hor_move_axis < 0.0f && !facing_right))
        {
            imp = new Vector2(impulses[index].x * dir, impulses[index].y);
        }
        else
        {
            imp = new Vector2(0, impulses[index].y);
        }

        _rigidbody.AddForce(new Vector2(imp.x, 0.0f), ForceMode2D.Impulse);

        //veritcal component
        if (_rigidbody.velocity.y >= 0 && _rigidbody.velocity.y < imp.y) 
        {
            ycomp = imp.y - _rigidbody.velocity.y;
        }else if(_rigidbody.velocity.y < 0)
        {
            ycomp = imp.y + -_rigidbody.velocity.y;
        }
        _rigidbody.AddForce(new Vector2(0.0f, ycomp), ForceMode2D.Impulse);

        

        //Debug.Log(_rigidbody.velocity.x);
    }

    public void attack()
    {
        if (!hitstop)
        {
            attack_pressed = true;
        }
        
    }

    public void attack_release()
    {
        attack_pressed = false;
    }

    public void attack2()
    {
        if (!hitstop)
        {
            attack2_pressed = true;
        }
        
    }

    public void attack2_release()
    {
        attack2_pressed = false;
    }
}
