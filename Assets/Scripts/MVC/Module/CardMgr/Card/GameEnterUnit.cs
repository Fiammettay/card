using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnterUnit : GameUnitBase
{
    public override void Init()
    {
        base.Init();
        GameApp.GameTurnManager.EnterGame();
    }
}
