using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameManager : MonoBehaviour {

    [SerializeField]
    public MonoBehaviour startingGameObject;
    public IControllableActor _active_controllable_Actor;

    public JumpCommand jump_command;
    public JumpReleaseCommand jump_release_command;
    public AttackCommand attack_command;
    public AttackReleaseCommand attack_release_command;
    public Attack2Command attack2_command;
    public Attack2ReleaseCommand attack2_release_command;
    public MoveCommand move_command;

	// Use this for initialization
	void Start () {
        jump_command = new JumpCommand();
        jump_release_command = new JumpReleaseCommand();
        attack_command = new AttackCommand();
        attack_release_command = new AttackReleaseCommand();
        attack2_command = new Attack2Command();
        attack2_release_command = new Attack2ReleaseCommand();

        move_command = new MoveCommand();
        setActiveControllableActor(startingGameObject);
	}
	
	// Update is called once per frame
	void Update () {
        Handle_Inputs();
	}

    void Handle_Inputs()
    {
        if (Input.GetKeyDown("space"))
        {
            jump_command.execute(_active_controllable_Actor);
        }

        if (Input.GetKeyUp("space"))
        {
            jump_release_command.execute(_active_controllable_Actor);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            attack_command.execute(_active_controllable_Actor);
        }

        if (Input.GetButtonUp("Fire1"))
        {
            attack_release_command.execute(_active_controllable_Actor);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            attack2_command.execute(_active_controllable_Actor);
        }

        if (Input.GetButtonUp("Fire2"))
        {
            attack2_release_command.execute(_active_controllable_Actor);
        }
        move_command.execute(_active_controllable_Actor, Input.GetAxisRaw("Horizontal"));
    }

    public void setActiveControllableActor(MonoBehaviour newActor)
    {
        IControllableActor test = newActor as IControllableActor;

        if(test != null)
        {
            _active_controllable_Actor = test;
        }
        else
        {
            Debug.Log("Supplied actor is not using the IControllableActor interface");
        }
    }
}
