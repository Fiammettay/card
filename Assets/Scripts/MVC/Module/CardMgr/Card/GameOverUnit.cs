using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUnit : GameUnitBase
{
    public override void Init()
    {
        base.Init();

        Result result = GameApp.GameTurnManager.Rule.GetResult(GameApp.GameTurnManager.Player, GameApp.GameTurnManager.Enemy);

        string message = null;

        switch (result)
        {
            case Result.PlayerWin:
                message = "玩家胜利";
                break;
            case Result.EnemyWin:
                message = "敌方胜利";
                break;
            case Result.Draw:
                message = "平局";
                break;
        }

        GameApp.CommandManager.AddCommand(new ShowTipViewCommand(message));
    }
}
