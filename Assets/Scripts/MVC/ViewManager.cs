using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewInfo
{
    public string PrefabName;
    public Transform parentTf;//所在父级
    public BaseController controller;//所属controller
    public int sort;//显示层级
}

public class ViewManager
{
    public Transform canvasTf;
    public Transform worldCanvasTf;

    Dictionary<int, IBaseView> _opens;//开启的面板
    Dictionary<int, IBaseView> _viewCache;//缓存的面板
    Dictionary<int, ViewInfo> _views;//注册的面板信息

    public ViewManager()
    {
        canvasTf = GameObject.Find("Canvas").transform;
        worldCanvasTf = GameObject.Find("Canvas").transform;
        _opens = new Dictionary<int, IBaseView>();
        _viewCache = new Dictionary<int, IBaseView>();
        _views = new Dictionary<int, ViewInfo>();
    }

    //注册视图
    private void Register(int key, ViewInfo info)
    {
        if(!_views.ContainsKey(key))
        {
            _views.Add(key, info);
        }
    }

    public void Register(ViewType type, ViewInfo info)
    {
        Register((int)type, info);
    }

    private void UnRegister(int key)
    {
        if( _views.ContainsKey(key))
        {
            _views.Remove(key);
        }
    }

    public void UnRegister(ViewType type)
    {
        UnRegister((int)type);
    }

    //是否开启
    private bool IsOpen(int key)
    {
        return _opens.ContainsKey(key);
    }

    public bool IsOpen(ViewType type)
    {
        return IsOpen((int)type);
    }

    //获得面板
    public IBaseView GetView(int key)
    {
        if(_opens.ContainsKey(key))
        {
            return _opens[key];
        }
        if(_viewCache.ContainsKey(key))
        {
            return _viewCache[key];
        }
        return null;
    }

    public T GetView<T>(int key) where T: class, IBaseView 
    {
        IBaseView view = GetView(key);
        if(view != null)
        {
            return view as T;
        }
        return null;
    }

    //销毁视图
    public void Destroy(int key)
    {
        IBaseView view = GetView(key);
        if(view != null)
        {
            UnRegister(key);
            view.DestroyView();
            if(_opens.ContainsKey(key))
            {
                _opens.Remove(key);
            }
            if(_viewCache.ContainsKey(key))
            {
                _viewCache.Remove(key);
            }
        }
    }

    private void Open(int key, params object[] args)
    {
        ViewInfo info = _views[key];
        IBaseView view = GetView(key);

        if (view== null)//如果已打开和缓存都没有
        {
            string type = ((ViewType)key).ToString();//获取类型名,脚本名和type同名
            GameObject uiObj = GameObject.Instantiate(Resources.Load<GameObject>($"View/{info.PrefabName}"), info.parentTf);

            Canvas canvas = uiObj.GetComponent<Canvas>();
            if(canvas == null)
            {
                canvas = uiObj.AddComponent<Canvas>();
            }

            if(uiObj.GetComponent<GraphicRaycaster>() == null)
            {
                uiObj.AddComponent<GraphicRaycaster>();
            }

            canvas.overrideSorting = true;//允许手动设置层级
            canvas.sortingOrder = info.sort;

            view = uiObj.AddComponent(Type.GetType(type)) as IBaseView;

            view.viewID = key;
            view.controller = info.controller;

            _viewCache.Add(key, view);
        }

        if(_opens.ContainsKey(key))
        {
            return;
        }

        _opens.Add(key, view);

        if(view.IsInit())
        {
            view.Open(args);
        }
        else
        {
            view.Init();
            view.Open(args);
        }
    }

    public void Open(ViewType type, params object[] args)
    {
        Open((int)type, args);
    }

    public void Close(int key, params object[] args)
    {
        if(!_opens.ContainsKey(key))
        {
            return;
        }
        IBaseView view = GetView(key);
        if(view != null)
        {
            _opens.Remove(key);
            view.Close(args);
        }
    }

    public void Close(ViewType type, params object[] args)
    {
        Close((int)type, args);
    }
}
