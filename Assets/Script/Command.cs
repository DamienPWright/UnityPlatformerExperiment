using UnityEngine;
using System.Collections;

public class Command {

    public virtual void execute(IControllableActor _actor) {
        
    }
    public virtual void execute(IControllableActor _actor, bool _boolean)
    {

    }
}

public class JumpCommand : Command
{
    override public void execute(IControllableActor _actor)
    {
        _actor.jump();
    }
}

public class JumpReleaseCommand : Command
{
    override public void execute(IControllableActor _actor)
    {
        _actor.jump_release();
    }
}

public class AttackCommand : Command
{
    override public void execute(IControllableActor _actor)
    {
        _actor.attack();
    }
}

public class AttackReleaseCommand : Command
{
    override public void execute(IControllableActor _actor)
    {
        _actor.attack_release();
    }
}

public class Attack2Command : Command
{
    override public void execute(IControllableActor _actor)
    {
        _actor.attack2();
    }
}

public class Attack2ReleaseCommand : Command
{
    override public void execute(IControllableActor _actor)
    {
        _actor.attack2_release();
    }
}

public class MoveCommand : Command
{
    public void execute(IControllableActor _actor, float axis)
    {
        _actor.move(axis);
    }
}