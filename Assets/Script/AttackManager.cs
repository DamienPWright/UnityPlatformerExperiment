using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AttackManager{

    int combo_counter = 0;
    Dictionary<string, Attack> attacks;
    Dictionary<string, string> special_preattacks;
    string first_normal_attack;
    string first_air_normal_attack;
    string first_special_attack;
    string first_air_special_attack;
    Attack current_attack;

    public void Initialise()
    {
        attacks = new Dictionary<string, Attack>();

        //Hardcoded for now. Figure out a way to turn these into data sometime soon. Delete these when this happens.
        Attack attack1 = new Attack();
        Attack attack2 = new Attack();

        attack1.AttackKey = "Attack1";
        attack1.NextAttackKey = "Attack2";
        attack1.NextSpecialAttackKey = "";
        attack1.animation = (int)PlayerWeeb.AnimType.attack_1;
        attack1.isXinputLocked = true;
        attack1.isJumpInputLocked = true;
        attack1.Inflicts_knockback = false;
        attack1.Hitstop_frames = 2;
        attack1.Power = 1;

        attack2.AttackKey = "Attack2";
        attack2.NextAttackKey = "Attack1";
        attack2.NextSpecialAttackKey = "Special1";
        attack2.animation = (int)PlayerWeeb.AnimType.attack_2;
        attack2.isXinputLocked = true;
        attack2.isJumpInputLocked = true;
        attack2.Inflicts_knockback = false;
        attack2.Hitstop_frames = 3;
        attack2.Power = 3;

        Attack special1 = new Attack();

        special1.AttackKey = "Special1";
        special1.NextAttackKey = "Attack1";
        special1.animation = (int)PlayerWeeb.AnimType.attack_special;
        special1.isXinputLocked = true;
        special1.isJumpInputLocked = true;
        special1.Inflicts_knockback = false;
        special1.Hitstop_frames = 6;
        special1.Power = 1;

        Attack airattack1 = new Attack();
        Attack airattack2 = new Attack();
        Attack airspecial1 = new Attack();

        airattack1.AttackKey = "AirAttack1";
        airattack1.NextAttackKey = "AirAttack2";
        airattack1.NextSpecialAttackKey = "";
        airattack1.animation = (int)PlayerWeeb.AnimType.air_attack_1;
        airattack1.isXinputLocked = false;
        airattack1.isJumpInputLocked = false;
        airattack1.Inflicts_knockback = false;
        airattack1.Hitstop_frames = 2;
        airattack1.Power = 1;

        airattack2.AttackKey = "AirAttack2";
        airattack2.NextAttackKey = "AirAttack1";
        airattack2.NextSpecialAttackKey = "AirSpecial1";
        airattack2.animation = (int)PlayerWeeb.AnimType.air_attack_2;
        airattack2.isXinputLocked = false;
        airattack2.isJumpInputLocked = false;
        airattack2.Inflicts_knockback = false;
        airattack2.Hitstop_frames = 2;
        airattack2.Power = 2;

        airspecial1.AttackKey = "AirSpecial1";
        airspecial1.NextAttackKey = "AirAttack1";
        airspecial1.animation = (int)PlayerWeeb.AnimType.air_attack_special;
        airspecial1.isXinputLocked = false;
        airspecial1.isJumpInputLocked = true;
        airspecial1.Inflicts_knockback = false;
        airspecial1.Hitstop_frames = 5;
        airspecial1.Power = 2;

        attacks.Add("Attack1", attack1);
        attacks.Add("Attack2", attack2);
        attacks.Add("Special1", special1);
        attacks.Add("AirAttack1", airattack1);
        attacks.Add("AirAttack2", airattack2);
        attacks.Add("AirSpecial1", airspecial1);

        first_normal_attack = "Attack1";
        first_air_normal_attack = "AirAttack1";
        first_special_attack = "";
        first_air_special_attack = "";
    }
	
    // Update is called once per frame
	void Update () {
	    foreach(KeyValuePair<string, Attack> entry in attacks)
        {
            entry.Value.Update();
        }
	}

    void ComboHandler()
    {
        combo_counter++;
    }

    public void doNormalAttack()
    {
        if (combo_counter == 0)
        {
            current_attack = attacks[first_normal_attack];
        }
        else
        {
            current_attack = getAttackByKey(current_attack.NextAttackKey);
        }
        //Debug.Log("current attack: " + current_attack.AttackKey + " combo count: " + combo_counter);
        ComboHandler();
    }

    public void doAirNormalAttack()
    {
        if(combo_counter == 0)
        {
            
            current_attack = attacks[first_air_normal_attack];
        }
        else
        {
            current_attack = getAttackByKey(current_attack.NextAttackKey);
        }
        //Debug.Log("current air attack: " + current_attack.AttackKey + " combo count: " + combo_counter);
        ComboHandler();
    }

    public void doSpecialAttack()
    {

        //Attack new_attack = null;

        if (combo_counter == 0)
        {
            current_attack = getAttackByKey(first_special_attack);
        }
        else
        {
            current_attack = getAttackByKey(current_attack.NextSpecialAttackKey);
        }

        ComboHandler();
    }

    public void doAirSpecialAttack()
    {
        if (combo_counter == 0)
        {
            current_attack = getAttackByKey(first_air_special_attack);
        }
        else
        {
            current_attack = getAttackByKey(current_attack.NextSpecialAttackKey);
        }
        ComboHandler();
    }

    //Deprecated.
    string getNextAttackKey(string[] keys, Dictionary<string, Attack> dictionary)
    {
        
        string attack_key = "";

        if(current_attack.NextAttackKey != "")
        {
            return current_attack.NextAttackKey;
        }

        foreach (string key in keys)
        {
            //Debug.Log(dictionary[key].PrevAttackKey + " " + current_attack.AttackKey);
            if (dictionary[key].PrevAttackKey == current_attack.AttackKey)
            {
                attack_key = dictionary[key].AttackKey;
                break;
            }
            
        }

        return attack_key;
    }

    //currently unusued but potentially useful...
    Attack getAttackByKey(string key)
    {
        Attack next_attack = null;

        try
        {
            next_attack = attacks[key];
        }
        catch(KeyNotFoundException e)
        {
            Debug.Log(e.Message);
        }

        return next_attack;
    }



    public bool canDoANormalAttack()
    {
        if(combo_counter == 0)
        {
            if(attacks.ContainsKey(first_normal_attack))
            {
                return true;
            }
        }
        else if (current_attack.NextAttackKey != "")
        {
            if (attacks.ContainsKey(current_attack.NextAttackKey))
            {
                return true;
            };
        }

        return false;
    }

    public bool canDoAirNormalAttack()
    {
        if (combo_counter == 0)
        {
            if (attacks.ContainsKey(first_air_normal_attack))
            {
                return true;
            }
        }
        else if (current_attack.NextAttackKey != "")
        {
            if (attacks.ContainsKey(current_attack.NextAttackKey))
            {
                return true;
            };
        }

        return false;
    }

    public bool canDoASpecialAttack()
    {
        if (combo_counter == 0)
        {
            if (attacks.ContainsKey(first_special_attack))
            {
                return true;
            }
        }else if (current_attack.NextSpecialAttackKey != "")
        {
            if (attacks.ContainsKey(current_attack.NextSpecialAttackKey))
            {
                return true;
            };
        }

        return false;
    }

    public bool canDoAirSpecialAttack()
    {
        if (combo_counter == 0)
        {
            if (attacks.ContainsKey(first_air_special_attack))
            {
                return true;
            }
        }
        else if (current_attack.NextSpecialAttackKey != "")
        {
            if (attacks.ContainsKey(current_attack.NextSpecialAttackKey))
            {
                return true;
            };
        }

        return false;
    }

    public int getAttackAnim()
    {
        if(current_attack == null)
        {
            return 0;
        }

        return current_attack.animation;
    }

    public int getAttackPower()
    {
        if (current_attack == null)
        {
            return 0;
        }

        return current_attack.Power;
    }

    public bool isXinputLocked()
    {
        if (current_attack == null)
        {
            return false;
        }

        return current_attack.isXinputLocked;
    }

    public bool isJumpInputLocked()
    {
        if (current_attack == null)
        {
            return false;
        }

        return current_attack.isJumpInputLocked;
    }

    public int getHitStopFrames()
    {
        if (current_attack == null)
        {
            return 0;
        }

        return current_attack.Hitstop_frames;
    }

    public void endAttack()
    {
        current_attack.EndAttack();
        //ComboHandler();
    }

    public void resetAttackCombo()
    {
        combo_counter = 0;
    }
}
