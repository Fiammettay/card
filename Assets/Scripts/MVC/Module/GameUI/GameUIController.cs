using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : BaseController
{
    public GameUIController()
    {
        SetModel(new GameUIModel());

        GameApp.ViewManager.Register(
            ViewType.TestView,
            new ViewInfo()
            {
                PrefabName = "TestView",
                controller = this,
                parentTf = GameApp.ViewManager.canvasTf
            });

        GameApp.ViewManager.Register(
            ViewType.InteractView,
            new ViewInfo()
            {
                PrefabName = "InteractView",
                controller = this,
                parentTf = GameApp.ViewManager.canvasTf
            });

        InitGlobalEvent();
    }

    public override void Init()
    {
        base.Init();
        model.Init();
    }

    public override void InitGlobalEvent()
    {
        GameApp.MsgCenter.AddEvent(Defines.CurrentInteractableChanged, OnCurrentInteractableChanged);
    }

    public override void RemoveGlobalEvent()
    {
        GameApp.MsgCenter.RemoveEvent(Defines.CurrentInteractableChanged, OnCurrentInteractableChanged);
    }

    private void OnCurrentInteractableChanged(object arg)
    {
        InteractableBase interactable = arg as InteractableBase;

        if (interactable == null)
        {
            GameApp.ViewManager.Close(
                ViewType.InteractView
            );

            return;
        }

        GameApp.ViewManager.Open(
            ViewType.InteractView,
            interactable
        );
    }
}
