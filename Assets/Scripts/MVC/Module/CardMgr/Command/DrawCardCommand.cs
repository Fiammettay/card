using System.Collections;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;

public class DrawCardCommand : BaseCommand
{
    private CardOwner owner;

    public DrawCardCommand(CardOwner owner)
    {
        this.owner = owner;
    }

    public override void Do()
    {
        base.Do();

        CardCharacterData characterData;
        if(owner == CardOwner.Player)
        {
            characterData = GameApp.GameTurnManager.Player;
        }
        else
        {
            characterData= GameApp.GameTurnManager.Enemy;
        }

        if(characterData.isStop)
        {
            Finish();
            return;
        }

        //1.从卡牌管理器取出抽到的牌数据
        CardData cardData = GameApp.CardManager.DrawCard();

        if(cardData == null )
        {
            Finish();
            return;
        }

        bool faceUp = owner == CardOwner.Player || characterData.cards.Count == 0;//说明为第一张牌，正面朝上;玩家牌全部朝上，敌方只有第一张朝上

        //2.更新抽牌方的数据
        characterData.DrawCard(cardData);

        //3.播放抽牌表现
        GameApp.CardTableManager.DrawCard(cardData, owner, faceUp, Finish);

    }

    protected override void Finish()
    {
        base.Finish();
        //通知抽牌结束，ui变化，轮到ai抽卡
        GameApp.MsgCenter.PostEvent(Defines.OnFinishDrawCard, owner);

        GameApp.CommandManager.AddCommand(new WaitCommand(1f, NotifyPlayerAction));

        GameApp.GameTurnManager.OnDrawFinished(owner);

    }

    private void NotifyPlayerAction()
    {
        if (!GameApp.GameTurnManager.CanPlayerAction())
        {
            return;
        }

        GameApp.MsgCenter.PostEvent(
            Defines.OnPlayerNeedAction);
    }
}
