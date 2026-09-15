using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseView : MonoBehaviour, IBaseView
{
    public int viewID { get; set; }

    public BaseController controller { get ; set; }

    protected Canvas _canvas;

    protected Dictionary<string, GameObject> m_cache_gos = new Dictionary<string, GameObject>();//缓存的面板组件

    private bool _isInit = false;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        
    }


    public virtual void Init()
    {
        _isInit = true;
    }

    public void ApplyControllerFunc(int controllerKey, string eventName, params object[] args)
    {
        controller.ApplyControllerFunc(controllerKey, eventName, args);
    }

    public void ApplyFunc(string eventName, params object[] args)
    {
        controller.ApplyFunc(eventName, args);
    }

    public virtual void Open(params object[] args)
    {
        SetVisible(true);
    }

    public virtual void Close(params object[] args)
    {
        SetVisible(false);
    }

    public void DestroyView()
    {
        controller = null;
        Destroy(gameObject);
    }

    public bool IsInit()
    {
        return _isInit;
    }

    public bool IsShow()
    {
        return _canvas.enabled == true;
    }

    public void SetVisible(bool Value)
    {
        _canvas.enabled = Value;
    }

    public GameObject Find(string res)
    {
        if (m_cache_gos.ContainsKey(res))
        {
            return m_cache_gos[res];
        }
        m_cache_gos.Add(res, transform.Find(res).gameObject);
        return m_cache_gos[res];
    }

    public T Find<T>(string res)
    {
        GameObject obj = Find(res);
        return obj.GetComponent<T>();
    }
}
