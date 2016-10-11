using UnityEngine;
using System.Collections;
using System;

public class PlayerWeeb : PlayerCommon {

    enum AnimType
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
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class PlayerWeebIdle : FSM_State
{
    public PlayerWeebIdle(FiniteStateMachine fsm, PlayerWeeb playerWeeb) : base(fsm)
    {

    }

    public override void FixedUpdate()
    {
        throw new NotImplementedException();
    }

    public override void OnEnter()
    {
        throw new NotImplementedException();
    }

    public override void OnExit()
    {
        throw new NotImplementedException();
    }

    public override void Update()
    {
        throw new NotImplementedException();
    }
}
