using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnemyUnit : GameUnitBase
{
    private int targetScore = 33;
    private int stopDistance = 7;

    public override void Init()
    {
        base.Init();

        //GameApp.CommandManager.AddCommand(new ShowTipViewCommand("µÐ·½»ØºÏ"));

        GameApp.CommandManager.AddCommand(new WaitCommand(1f));

        SetEnemyAction();
    }

    private void SetEnemyAction()
    {
        CardCharacterData enemy = GameApp.GameTurnManager.Enemy;

        if (ShouldDraw(enemy))
        {
            GameApp.CommandManager.AddCommand(new DrawCardCommand(CardOwner.Enemy));
        }
        else
        {
            GameApp.GameTurnManager.StopCharacter(CardOwner.Enemy);
        }
    }

    private bool ShouldDraw(CardCharacterData data)
    {
        int score = data.totalScore;

        if(data.cards.Count == 0)
        {
            return true;
        }
     
        if(score > targetScore)
        {
            return false;
        }

        int distance = targetScore - score;

        return distance >= stopDistance;
    }

}
