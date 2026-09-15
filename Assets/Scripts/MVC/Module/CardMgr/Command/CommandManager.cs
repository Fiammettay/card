using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager
{
    private Queue<BaseCommand> willDoCommandQueue;//将要执行的命令，先进先执行    
    private BaseCommand current;//当前执行的命令

    public CommandManager()
    {
        willDoCommandQueue = new Queue<BaseCommand>();
    }

    public bool IsRunningCommand
    {
        get
        {
            return current != null || willDoCommandQueue.Count > 0;
        }
    }

    //添加命令
    public void AddCommand(BaseCommand cmd)
    {
        willDoCommandQueue.Enqueue(cmd);
    }

    //执行
    public void Update(float dt)
    {
        if (current == null)
        {
            if (willDoCommandQueue.Count > 0)
            {
                current = willDoCommandQueue.Dequeue();
                current.Do();
            }
        }
        else
        {
            if (current.Update(dt) == true)//执行完后
            {
                current = null;
            }
        }
    }

    public void Clear()
    {
        willDoCommandQueue.Clear();
        current = null;
    }

}
