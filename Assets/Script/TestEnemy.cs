using UnityEngine;
using System.Collections;
using System;

public class TestEnemy : MonoBehaviour, IAttackableActor {

    int health = 3;
    int maxHealth = 3;

    public void takeDamage(int damage)
    {
        Debug.Log("Enemy took damage!");
        health -= damage;
        checkHealth();
    }

    void checkHealth()
    {
        if(health <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
