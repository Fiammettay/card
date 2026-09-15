using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTipViewCommand : BaseCommand
{
    private string message;

    public ShowTipViewCommand(string message)
    {
        this.message = message;
    }

    public override void Do()
    {
        base.Do();
        GameApp.ViewManager.Open(ViewType.TipView, message, (Action)Finish);
    }
}
