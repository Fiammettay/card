using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseView
{
    bool IsInit();//视图是否已初始化

    bool IsShow();//是否显示


    void Init();//初始化数据

    void Open(params object[] args);//打开

    void Close(params object[] args);

    void DestroyView();//删除面板

    void ApplyFunc(string eventName, params object[] args);//触发本controller事件

    void ApplyControllerFunc(int controllerKey, string eventName, params object[] args);//触发其他controller事件

    void SetVisible(bool Value);

    int viewID { get; set; }

    BaseController controller { get; set; }//面板所属控制器
}
