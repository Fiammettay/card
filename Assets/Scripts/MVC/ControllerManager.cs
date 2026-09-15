using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllerManager
{
    private Dictionary<int, BaseController> _modules;

    public ControllerManager()
    {
        _modules = new Dictionary<int, BaseController>();
    }

    private void Register(int controllerKey, BaseController ctl)
    {
        if (!_modules.ContainsKey(controllerKey))
        {
            _modules.Add(controllerKey, ctl);
        }
    }

    public void Register(ControllerType type, BaseController ctl)
    {
        Register((int)type, ctl);
    }

    private void UnRegister(int controllerKey)
    {
        if(_modules.ContainsKey(controllerKey))
        {
            _modules.Remove(controllerKey);
        }
    }

    public void UnRegister(ControllerType type)
    {
        UnRegister((int)type);
    }

    //为所有controller初始化
    public void InitAllModules()
    {
        foreach(var item in _modules.Values)
        {
            item.Init();
        }
    }

    public void ClearAllModules()
    {
        List<int> keys = _modules.Keys.ToList();
        for(int i = 0; i < keys.Count; i++)
        {
            _modules[keys[i]].Destroy();
            _modules.Remove(keys[i]);
        }
    }

    public void ApplyFunc(int controllerKey, string eventName, params object[] args)
    {
        if(_modules.ContainsKey(controllerKey))
        {
            _modules[controllerKey].ApplyFunc(eventName, args);
        }
    }

    public BaseModel GetControllerModel(int controllerKey)
    {
        if (_modules.ContainsKey(controllerKey))
        {
            return _modules[controllerKey].GetModel();
        }
        else
        {
            return null;
        }
    }

}
