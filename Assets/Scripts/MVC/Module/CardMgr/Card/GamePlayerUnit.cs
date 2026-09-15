using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerUnit : GameUnitBase
{
    public override void Init()
    {
        base.Init();
        //GameApp.CommandManager.AddCommand(new ShowTipViewCommand("Íæ¼Ò»ØºÏ"));

        if(GameApp.GameTurnManager.Player.cards.Count == 0)
        {
            GameApp.CommandManager.AddCommand(new DrawCardCommand(CardOwner.Player));
        }
    }
}
