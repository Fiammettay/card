using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageCenter
{
    private Dictionary<string, System.Action<object>> msgDic;//存储普通消息字典
    private Dictionary<string, System.Action<object>> tempMsgDic;//存储临时消息字典，执行后就移除
    private Dictionary<System.Object, Dictionary<string, System.Action<object>>> objMsgDic;//存储特定对象的消息

    public MessageCenter()
    {
        msgDic = new Dictionary<string, System.Action<object>>();
        tempMsgDic = new Dictionary<string, System.Action<object>>();
        objMsgDic = new Dictionary<object, Dictionary<string, System.Action<object>>>();
    }

    public void AddEvent(string eventName, System.Action<object> callback)
    {
        if (msgDic.ContainsKey(eventName))
        {
            msgDic[eventName] += callback;
        }
        else
        {
            msgDic.Add(eventName, callback);
        }
    }

    public void RemoveEvent(string eventName, System.Action<object> callback)
    {
        if (msgDic.ContainsKey(eventName))
        {
            msgDic[eventName] -= callback;
            if (msgDic[eventName] == null)
            {
                msgDic.Remove(eventName);
            }
        }
    }

    public void PostEvent(string eventName, System.Object arg = null)
    {
        if (msgDic.ContainsKey(eventName))
        {
            msgDic[eventName]?.Invoke(arg);
        }
    }


    //添加对象事件
    public void AddEvent(System.Object listenerObj, string eventName, System.Action<object> callback)
    {
        if (objMsgDic.ContainsKey(listenerObj))
        {
            if (objMsgDic[listenerObj].ContainsKey(eventName))
            {
                objMsgDic[listenerObj][eventName] += callback;
            }
            else
            {
                objMsgDic[listenerObj].Add(eventName, callback);
            }
        }
        else
        {
            Dictionary<string, System.Action<object>> _tempDic = new Dictionary<string, System.Action<object>>();
            _tempDic.Add(eventName, callback);
            objMsgDic.Add(listenerObj, _tempDic);
        }
    }

    public void RemoveEvent(System.Object listenerObj, string eventName, System.Action<object> callback)
    {
        if (objMsgDic.ContainsKey(listenerObj))
        {
            if (objMsgDic[listenerObj].ContainsKey(eventName))
            {
                objMsgDic[listenerObj][eventName] -= callback;
                if (objMsgDic[listenerObj][eventName] == null)
                {
                    objMsgDic[listenerObj].Remove(eventName);
                    if (objMsgDic[listenerObj].Count == 0)
                    {
                        objMsgDic.Remove(listenerObj);
                    }
                }
            }
        }
    }

    public void RemoveObjAllEvent(System.Object listenerObj)
    {
        if (objMsgDic.ContainsKey(listenerObj))
        {
            objMsgDic.Remove(listenerObj);
        }
    }

    public void PostEvent(System.Object listenerObj, string eventName, System.Object arg = null)
    {
        if (objMsgDic.ContainsKey(listenerObj))
        {
            if (objMsgDic[listenerObj].ContainsKey(eventName))
            {
                objMsgDic[listenerObj][eventName]?.Invoke(arg);
            }
        }
    }


    public void AddTempEvent(string eventName, System.Action<object> callback)
    {
        if (tempMsgDic.ContainsKey(eventName))
        {
            tempMsgDic[eventName] = callback;//覆盖掉之前的
        }
        else
        {
            tempMsgDic.Add(eventName, callback);
        }
    }

    public void PostTempEvent(string eventName, System.Object arg = null)
    {
        if (tempMsgDic.ContainsKey(eventName))
        {
            tempMsgDic[eventName]?.Invoke(arg);
            tempMsgDic.Remove(eventName);
        }
    }
}
