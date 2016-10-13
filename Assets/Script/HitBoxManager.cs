using UnityEngine;
using System.Collections;
using System;

public class HitBoxManager : MonoBehaviour {

    public PolygonCollider2D attack1_hitbox;
    public PolygonCollider2D attack2_hitbox;
    public PolygonCollider2D attack3_hitbox;
    PolygonCollider2D[] hitboxes;
    HitboxWrapper[] _hitboxes;
    public PolygonCollider2D active_collider;
    public HitboxWrapper current_hitbox_wrapper;
    public PolygonCollider2D localcollider;
    public bool hitbox_activated = false;
    int hitbox_lifecounter = 0;
    int hitbox_life = 3; //how many frames the hitbox will persist for.

    // Use this for initialization
    void Start () {
        //hitboxes = new PolygonCollider2D[] { attack1_hitbox, attack2_hitbox, attack3_hitbox };
        _hitboxes = new HitboxWrapper[]
        {
            new PolygonHitboxWrapper(attack1_hitbox, this),
            new PolygonHitboxWrapper(attack2_hitbox, this),
            new PolygonHitboxWrapper(attack3_hitbox, this)
        };
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
            current_hitbox_wrapper.ClearHitbox();
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
            //Debug.Log("Collider hit attackable");
            //there may be a better way to do this part... 
            MonoBehaviour script = other.GetComponent<MonoBehaviour>();
            if(script is IAttackableActor)
            {
                (script as IAttackableActor).takeDamage(1);
            }
        }
    }

    public void setHitBox(int hitboxID)
    {
        if(hitboxID >= _hitboxes.Length)
        {
            return;
        }
        else
        {
            _hitboxes[hitboxID].SetHitbox();
        }
    }

    public void SetCollider(int hitboxID)
    {
        /*
        if (hitboxID >= hitboxes.Length)
        {
            return;
        }
        else
        {
            active_collider = hitboxes[hitboxID];
            localcollider.SetPath(0, active_collider.GetPath(0));
            hitbox_activated = true;
        }
        */
        if (hitboxID >= _hitboxes.Length)
        {
            return;
        }
        else
        {
            current_hitbox_wrapper = _hitboxes[hitboxID];
            current_hitbox_wrapper.SetHitbox();
        }
    }

    public void ClearHitbox()
    {
        current_hitbox_wrapper.ClearHitbox();
    }
}

public abstract class HitboxWrapper
{
    public abstract void SetHitbox();
    public abstract void ClearHitbox();
}

public class PolygonHitboxWrapper: HitboxWrapper
{
    PolygonCollider2D _poly;
    HitBoxManager _hbm;

    public PolygonHitboxWrapper(PolygonCollider2D poly, HitBoxManager hbm)
    {
        _poly = poly;
        _hbm = hbm;
    }

    public override void SetHitbox()
    {
        _hbm.active_collider = _poly; //needs to be made active_poly_collider or something
        _hbm.localcollider.SetPath(0, _hbm.active_collider.GetPath(0)); //needs to be made local_poly_collider or something
        _hbm.hitbox_activated = true;
    }

    public override void ClearHitbox()
    {
        _hbm.localcollider.pathCount = 0;
    }
}

public class BoxHitboxWrapper : HitboxWrapper
{
    BoxCollider2D box;

    public BoxHitboxWrapper(BoxCollider2D _box)
    {
        box = _box;
    }

    public override void SetHitbox()
    {
        throw new NotImplementedException();
    }

    public override void ClearHitbox()
    {
        throw new NotImplementedException();
    }
}

public class CircleHitboxWrapper : HitboxWrapper
{
    CircleCollider2D circle;

    public CircleHitboxWrapper(CircleCollider2D _circle)
    {
        circle = _circle;
    }

    public override void SetHitbox()
    {
        throw new NotImplementedException();
    }

    public override void ClearHitbox()
    {
        throw new NotImplementedException();
    }
}