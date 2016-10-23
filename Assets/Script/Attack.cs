using UnityEngine;
using System.Collections;

public class Attack {

    AnimationMonitor _animation_monitor;

    protected int _animation;

    protected bool _xinput_lock;
    protected bool _jumpinput_lock;

    protected float _internal_cooldown = 0.0f;
    protected float _internal_cooldown_timer = 0.0f;

    protected bool _on_cooldown;

    protected string _attackKey = "";
    protected string _prevAttackKey = "";
    protected string _nextNormalAttackKey = "";
    private string _nextSpecialAttackKey = "";
    protected bool _inflicts_knockback = false;
    protected Vector2 _knockback_vector;
    protected int hitstop_frames = 0;
    protected int power = 0;

    public int animation{
        set { _animation = value; }
        get { return _animation;}
    }

    
    public bool isXinputLocked
    {
        set { _xinput_lock = value; }
        get { return _xinput_lock; }
    }

    
    public bool isJumpInputLocked
    {
        set { _jumpinput_lock = value; }
        get { return _jumpinput_lock; }
    }

    

    public bool isOnCooldown
    {
        set { _on_cooldown = value; }
        get { return _on_cooldown; }
    }

    

    public string AttackKey
    {
        set { _attackKey = value; }
        get
        {
            return _attackKey;
        }
    }

    public string PrevAttackKey
    {
        set { _prevAttackKey = value; }
        get
        {
            return _prevAttackKey;
        }
    }

    public string NextAttackKey
    {
        set { _nextNormalAttackKey = value; }
        get
        {
            return _nextNormalAttackKey;
        }
    }

    public int Hitstop_frames
    {
        set { hitstop_frames = value; }
        get {
            return hitstop_frames;
        }
    }

    public int Power
    {
        set { power = value; }
        get
        {
            return power;
        }
    }

    public bool Inflicts_knockback
    {
        set { _inflicts_knockback = value; }
        get
        {
            return _inflicts_knockback;
        }
    }

    public Vector2 Knockback_vector
    {
        set { _knockback_vector = value; }
        get
        {
            return _knockback_vector;
        }
    }

    public string NextSpecialAttackKey
    {
        get
        {
            return _nextSpecialAttackKey;
        }

        set
        {
            _nextSpecialAttackKey = value;
        }
    }

    public Attack()
    {

    }

    // Update is called once per frame
    public void Update() {
        if (_on_cooldown)
        {
            _internal_cooldown += Time.deltaTime;
            if(_internal_cooldown >= _internal_cooldown_timer)
            {
                _internal_cooldown_timer = 0.0f;
                _on_cooldown = false;
            }
        }
    }

    public void EndAttack()
    {
        //set cooldowns etc
    }
}
