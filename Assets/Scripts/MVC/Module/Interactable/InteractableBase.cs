using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("交互设置")]
    [SerializeField] private string text = "交互";
    [SerializeField] private bool canInteract = true;

    [Header("摄像机设置")]
    [Tooltip("执行交互后摄像机移动到的位置和角度")]
    [SerializeField] private Transform cameraPoint;

    public string Text
    {
        get
        {
            return text;
        }
    }

    public bool CanInteract
    {
        get
        {
            return canInteract;
        }
    }

    public Transform CameraPoint
    {
        get
        {
            return cameraPoint;
        }
    }

    public virtual void Interact()
    {
        
    }
}
