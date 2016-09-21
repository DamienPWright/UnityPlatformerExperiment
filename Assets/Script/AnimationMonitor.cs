using UnityEngine;
using System.Collections;

public class AnimationMonitor : MonoBehaviour{

    bool animation_complete = false;
    bool interruptable = false;

    public void reset()
    {
        animation_complete = false;
        interruptable = false;
    }

    public bool isAnimationComplete()
    {
        return animation_complete;
    }

    public void setComplete()
    {
        animation_complete = true;
    }

    public void setInterruptable()
    {
        interruptable = true;
    }

    public bool isInterruptable()
    {
        return interruptable;
    }

    public void animationDebugMessage(string msg)
    {
        Debug.Log(msg);
    }
}
