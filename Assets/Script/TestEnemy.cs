using UnityEngine;
using System.Collections;
using System;

public class TestEnemy : Enemy {

    public void Start()
    {
        maxHP = 3;
        curHP = maxHP;
    }

    public override void onDeath()
    {
        this.gameObject.SetActive(false);
    }
	
    public override void damageReaction()
    {
        //throw new NotImplementedException();
    }
}
