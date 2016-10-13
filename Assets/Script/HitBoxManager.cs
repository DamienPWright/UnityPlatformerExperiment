using UnityEngine;
using System.Collections;
using System;

public class HitBoxManager : MonoBehaviour {

    public PolygonCollider2D attack1_hitbox;
    public PolygonCollider2D attack2_hitbox;
    public PolygonCollider2D attack3_hitbox;
    public PolygonCollider2D[] poly2dHitboxes;
    public CircleCollider2D[] circleHitboxes;
    public BoxCollider2D[] boxHitboxes;
    HitboxWrapper[] _hitboxes;
    public HitboxWrapper current_hitbox_wrapper;
    public PolygonCollider2D active_poly_collider;
    public PolygonCollider2D local_polycollider;
    public CircleCollider2D active_circle_collider;
    public CircleCollider2D local_circlecollider;
    public BoxCollider2D active_box_collider;
    public BoxCollider2D local_boxcollider;
    public bool hitbox_activated = false;
    int hitbox_lifecounter = 0;
    int hitbox_life = 3; //how many frames the hitbox will persist for.

    // Use this for initialization
    void Start () {
        //hitboxes = new PolygonCollider2D[] { attack1_hitbox, attack2_hitbox, attack3_hitbox };
        //_hitboxes = new HitboxWrapper[]
        //{
        //    new PolygonHitboxWrapper(attack1_hitbox, this),
        //    new PolygonHitboxWrapper(attack2_hitbox, this),
        //    new PolygonHitboxWrapper(attack3_hitbox, this)
        //};

        local_polycollider = gameObject.AddComponent<PolygonCollider2D>();
        local_polycollider.isTrigger = true;
        local_polycollider.enabled = false;
        local_polycollider.pathCount = 0;

        local_circlecollider = gameObject.AddComponent<CircleCollider2D>();
        local_circlecollider.isTrigger = true;
        local_circlecollider.enabled = false;
        local_circlecollider.offset = new Vector2(0, 0);
        local_circlecollider.radius = 0;

        local_boxcollider = gameObject.AddComponent<BoxCollider2D>();
        local_boxcollider.isTrigger = true;
        local_boxcollider.enabled = false;
        local_boxcollider.offset = new Vector2(0, 0);
        local_boxcollider.size = new Vector2(0, 0);

        _hitboxes = new HitboxWrapper[poly2dHitboxes.Length + circleHitboxes.Length + boxHitboxes.Length];

        for(int i=0; i < _hitboxes.Length; i++)
        {
            if(i < poly2dHitboxes.Length)
            {
                _hitboxes[i] = new PolygonHitboxWrapper(poly2dHitboxes[i], this);
            }
            else if(i < circleHitboxes.Length)
            {
                if(circleHitboxes.Length == 0)
                {
                    continue;
                }
                _hitboxes[i] = new CircleHitboxWrapper(circleHitboxes[i- poly2dHitboxes.Length], this);
            }
            else
            {
                if (boxHitboxes.Length == 0)
                {
                    continue;
                }
                _hitboxes[i] = new BoxHitboxWrapper(boxHitboxes[i - poly2dHitboxes.Length - circleHitboxes.Length], this);
            }
            
        }
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
        _hbm.active_poly_collider = _poly; //needs to be made active_poly_collider or something
        _hbm.local_polycollider.SetPath(0, _hbm.active_poly_collider.GetPath(0)); //needs to be made local_poly_collider or something
        _hbm.local_polycollider.enabled = true;
        _hbm.hitbox_activated = true;
    }

    public override void ClearHitbox()
    {
        _hbm.local_polycollider.pathCount = 0;
        _hbm.local_polycollider.enabled = false;
    }
}

public class BoxHitboxWrapper : HitboxWrapper
{
    BoxCollider2D _box;
    HitBoxManager _hbm;

    public BoxHitboxWrapper(BoxCollider2D box, HitBoxManager hbm)
    {
        _box = box;
        _hbm = hbm;
    }

    public override void SetHitbox()
    {
        _hbm.active_box_collider = _box;
        _hbm.local_boxcollider.size = _box.size;
        _hbm.local_boxcollider.offset = _box.offset;
        _hbm.local_boxcollider.enabled = true;
        _hbm.hitbox_activated = true;
    }

    public override void ClearHitbox()
    {
        _hbm.local_boxcollider.enabled = false;
    }
}

public class CircleHitboxWrapper : HitboxWrapper
{
    CircleCollider2D _circle;
    HitBoxManager _hbm;

    public CircleHitboxWrapper(CircleCollider2D circle, HitBoxManager hbm)
    {
        _circle = circle;
        _hbm = hbm;
    }

    public override void SetHitbox()
    {
        _hbm.active_circle_collider = _circle;
        _hbm.local_circlecollider.radius = _circle.radius;
        _hbm.local_circlecollider.offset = _circle.offset;
        _hbm.local_circlecollider.enabled = true;
        _hbm.hitbox_activated = true;
    }

    public override void ClearHitbox()
    {
        _hbm.local_circlecollider.enabled = false;
    }
}