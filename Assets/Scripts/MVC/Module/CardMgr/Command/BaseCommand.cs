using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseCommand
{
    protected bool isFinish;//是否做完的标记

    public BaseCommand()
    {
        isFinish = false;
    }

    public virtual bool Update(float dt)
    {
        return isFinish;
    }

    //执行命令
    public virtual void Do()
    {

    }

    protected virtual void Finish()
    {
        isFinish = true;
    }

}
