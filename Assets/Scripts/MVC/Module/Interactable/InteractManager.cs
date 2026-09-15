using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractManager
{
    /// <summary>
    /// 当前选中的交互对象
    /// </summary>
    public InteractableBase CurrentInteractable { get; private set; }

    /// <summary>
    /// 请求进入交互摄像机
    /// </summary>
    public event Action<Transform> EnterCameraRequested;

    /// <summary>
    /// 设置当前交互对象
    /// </summary>
    public void SetCurrent(InteractableBase interactable)
    {
        if (CurrentInteractable == interactable)
        {
            return;
        }

        CurrentInteractable = interactable;

        GameApp.MsgCenter.PostEvent(Defines.CurrentInteractableChanged, CurrentInteractable);
    }

    /// <summary>
    /// 清除当前交互对象
    /// </summary>
    public void ClearCurrent()
    {
        SetCurrent(null);
    }

    /// <summary>
    /// 执行当前交互
    /// </summary>
    public void InteractCurrent()
    {
        InteractableBase interactable = CurrentInteractable;

        if (interactable == null)
        {
            return;
        }

        if (!interactable.CanInteract)
        {
            return;
        }

        // 提前保存机位，避免 Interact 中改变或销毁对象
        Transform cameraPoint = interactable.CameraPoint;

        // 执行物体自己的交互逻辑
        interactable.Interact();

        // 通知摄像机进入交互视角
        if (cameraPoint != null)
        {
            GameApp.MsgCenter.PostEvent(Defines.OnEnterInteractionCamera, cameraPoint);
        }
    }
}
