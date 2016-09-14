﻿using UnityEngine;
using System.Collections;

public class HitBoxManager : MonoBehaviour {

    public PolygonCollider2D attack1_hitbox;
    public PolygonCollider2D attack2_hitbox;
    public PolygonCollider2D attack3_hitbox;
    PolygonCollider2D[] hitboxes;
    PolygonCollider2D active_collider;
    PolygonCollider2D localcollider;
    public bool hitbox_activated = false;
    int hitbox_lifecounter = 0;
    int hitbox_life = 1; //how many frames the hitbox will persist for.

    // Use this for initialization
    void Start () {
        hitboxes = new PolygonCollider2D[] { attack1_hitbox, attack2_hitbox, attack3_hitbox };
        localcollider = gameObject.AddComponent<PolygonCollider2D>();
        localcollider.isTrigger = true;
        localcollider.pathCount = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if(hitbox_lifecounter >= hitbox_life)
        {
            hitbox_activated = false;
            hitbox_lifecounter = 0;
            ClearHitbox();
        }
        if (hitbox_activated)
        {
            hitbox_lifecounter++;
            //Debug.Log("hitbox alive");
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Attackable")
        {
            Debug.Log("Collider hit attackable");
            //there may be a better way to do this part... 
            MonoBehaviour script = other.GetComponent<MonoBehaviour>();
            if(script is IAttackableActor)
            {
                (script as IAttackableActor).takeDamage(1);
            }
        }
    }

    public void SetCollider(int hitboxID)
    {
        if(hitboxID >= hitboxes.Length)
        {
            return;
        }
        else
        {
            active_collider = hitboxes[hitboxID];
            localcollider.SetPath(0, active_collider.GetPath(0));
            hitbox_activated = true;
        }

    }

    public void ClearHitbox()
    {
        localcollider.pathCount = 0;
    }
}