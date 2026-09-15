using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController
{
    private Dictionary<string, System.Action<object[]>> message;//事件字典

    protected BaseModel model;//数据

    public BaseController()
    {
        message = new Dictionary<string, System.Action<object[]>>();
    }

    public virtual void Init()
    {

    }

    public void RegisterFunc(string eventName, System.Action<object[]> callback)
    {
        if(message.ContainsKey(eventName))
        {
            message[eventName] += callback;
        }
        else
        {
            message.Add(eventName, callback);
        }
    }

    public void UnRegisterFunc(string eventName)
    {
        if( message.ContainsKey(eventName))
        {
            message.Remove(eventName);
        }
    }

    public void ApplyFunc(string eventName, params object[] args)
    {
        if(message.ContainsKey(eventName))
        {
            message[eventName]?.Invoke(args);
        }
    }

    //使用其他controller方法
    public void ApplyControllerFunc(int controllerKey, string eventName, params object[] args)
    {
        GameApp.ControllerManager.ApplyFunc(controllerKey, eventName, args);
    }

    public void ApplyControllerFunc(ControllerType type, string eventName, params object[] args)
    {
        ApplyControllerFunc((int)type, eventName, args);
    }

    public void SetModel(BaseModel model)
    {
        this.model = model;
    }

    public BaseModel GetModel()
    {
        return this.model;
    }

    public T GetModel<T>() where T : BaseModel
    {
        return model as T;
    }

    public BaseModel GetControllerModel(int controllerKey)
    {
        //得到其他controller的model
        return GameApp.ControllerManager.GetControllerModel(controllerKey);
    }


    public virtual void InitModuleEvent()
    {

    }

    public virtual void RemoveModuleEvent()
    {

    }

    public virtual void InitGlobalEvent()
    {

    }

    public virtual void RemoveGlobalEvent()
    {

    }

    public virtual void Destroy()
    {
        RemoveModuleEvent();
        RemoveGlobalEvent();
    }
}
