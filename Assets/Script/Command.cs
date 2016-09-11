﻿using UnityEngine;
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