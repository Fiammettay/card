using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUnitBase
{
    public virtual void Init()
    {

    }

    public virtual bool Update(float dt)
    {
        return false;
    }
}
