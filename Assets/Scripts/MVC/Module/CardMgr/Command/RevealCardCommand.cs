using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealCardCommand : BaseCommand
{
    public override void Do()
    {
        base.Do();

        GameApp.CardTableManager.RevealAllCards(Finish);
    }

    protected override void Finish()
    {
        base.Finish();

        GameApp.GameTurnManager.RevealFinished();
    }
}
