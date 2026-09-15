using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Result
{
    None,
    PlayerWin,
    EnemyWin,
    Draw//平局
}

public class CardRule
{
    private int targetScore = 33;
    private int bustColorCount = 4;

    /// <summary>
    /// 判断游戏是否还需要继续,双方停止就结算
    /// </summary>
    /// <param name="player"></param>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public bool CanSettle(CardCharacterData player, CardCharacterData enemy)
    {
        return player.isStop && enemy.isStop;
    }

    /// <summary>
    /// 结算时调用是否爆炸
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool IsBust(CardCharacterData data)
    {
        return data.redCount >= bustColorCount || data.blackCount >= bustColorCount;
    }


    public Result GetResult(CardCharacterData player, CardCharacterData enemy)
    {
        if(IsBust(player) && IsBust(enemy))
        {
            return Result.Draw;
        }

        if(IsBust(player))
        {
            return Result.EnemyWin;
        }

        if(IsBust(enemy))
        {
            return Result.PlayerWin;
        }

        int playerDistance = Mathf.Abs(targetScore - player.totalScore);

        int enemyDistance = Mathf.Abs(targetScore - enemy.totalScore);

        if (playerDistance < enemyDistance)
        {
            return Result.PlayerWin;
        }
        if (playerDistance > enemyDistance)
        {
            return Result.EnemyWin;
        }
        return Result.Draw;

    }
}
