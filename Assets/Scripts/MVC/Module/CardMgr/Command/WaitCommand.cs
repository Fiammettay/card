using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitCommand : BaseCommand
{
    private float time;
    private Action onWaitFinished;

    public WaitCommand(float time, Action onWaitFinished = null)
    {
        this.time = time;
        this.onWaitFinished = onWaitFinished;
    }

    public override bool Update(float dt)
    {
        time -= dt;
        if(time < 0)
        {
            onWaitFinished?.Invoke();
            return true;
        }
        return false;
    }
}
