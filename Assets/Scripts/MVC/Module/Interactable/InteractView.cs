using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractView : BaseView
{
    private Button interactButton;

    public override void Init()
    {
        base.Init();

        interactButton = Find<Button>("bg/interactBtn");
        interactButton.onClick.AddListener(OnInteractBtn);
    }

    private void OnInteractBtn()
    {
        GameApp.ViewManager.Close(viewID);
        GameApp.InteractManager.InteractCurrent();
    }

    private void OnDestroy()
    {
        if (interactButton != null)
        {
            interactButton.onClick.RemoveListener(OnInteractBtn);
        }
    }
}
