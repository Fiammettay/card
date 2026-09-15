using System.Collections;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;

public class CardGameController : BaseController
{
    public CardGameController()
    {
        GameApp.ViewManager.Register(ViewType.CardGameView, new ViewInfo()
        {
            PrefabName = "CardGameView",
            controller = this,
            parentTf = GameApp.ViewManager.canvasTf
        });
        GameApp.ViewManager.Register(ViewType.TipView, new ViewInfo()
        {
            PrefabName = "TipView",
            controller = this,
            parentTf = GameApp.ViewManager.canvasTf
        });

        InitModuleEvent();
        InitGlobalEvent();
    }

    public override void Init()
    {
        base.Init();
    }

    public override void InitModuleEvent()
    {
        base.InitModuleEvent();
        
        RegisterFunc(Defines.OnDrawPlayerCard, OnDrawPlayerCard);
        RegisterFunc(Defines.OnCharacterStop, OnCharacterStop);
        RegisterFunc(Defines.OnReStartGame, OnReStartGame);
    }

    public override void InitGlobalEvent()
    {
        base.InitGlobalEvent();
        GameApp.MsgCenter.AddEvent(Defines.OnInteractionCamera, OpenCardGameView);//等待摄像头完全就位
        GameApp.MsgCenter.AddEvent(Defines.OnFinishDrawCard, OnCardCharacterDataChanged);
        GameApp.MsgCenter.AddEvent(Defines.OnSettlementFinished, OnSettlementFinished);
        GameApp.MsgCenter.AddEvent(Defines.OnPlayerNeedAction, OnPlayerNeedAction);
    }


    private void OpenCardGameView(object arg)
    {
        GameApp.ViewManager.Open(ViewType.CardGameView);
    }



    public override void RemoveModuleEvent()
    {
        base.RemoveModuleEvent();
        UnRegisterFunc(Defines.OnDrawPlayerCard);
        UnRegisterFunc(Defines.OnCharacterStop);
        UnRegisterFunc(Defines.OnReStartGame);
    }

    public override void RemoveGlobalEvent()
    {
        base.RemoveGlobalEvent();
        GameApp.MsgCenter.RemoveEvent(Defines.OnInteractionCamera, OpenCardGameView);
        GameApp.MsgCenter.RemoveEvent(Defines.OnFinishDrawCard, OnCardCharacterDataChanged);
        GameApp.MsgCenter.RemoveEvent(Defines.OnSettlementFinished, OnSettlementFinished);
        GameApp.MsgCenter.RemoveEvent(Defines.OnPlayerNeedAction, OnPlayerNeedAction);
    }

    private void OnDrawPlayerCard(object[] arg)
    {
        if (GameApp.GameTurnManager.state != GameState.Player)
        {
            return;
        }
        if (GameApp.CommandManager.IsRunningCommand)
        {
            return;
        }


        GameApp.CommandManager.AddCommand(new DrawCardCommand(CardOwner.Player));
    }

    private void OnCardCharacterDataChanged(object arg)
    {
        if(arg is not CardOwner owner)
        {
            return;
        }

        CardGameView view = GameApp.ViewManager.GetView<CardGameView>((int)ViewType.CardGameView);

        if(owner == CardOwner.Player)
        {
            view.RefreshPlayerInfo();
        }
        else
        {
            view.RefreshEnemyInfo();
        }
        view.ShowScore(false);
    }

    private void OnCharacterStop(object[] arg)
    {
        if (GameApp.GameTurnManager.state != GameState.Player)
        {
            return;
        }

        if (GameApp.CommandManager.IsRunningCommand)
        {
            return;
        }

        GameApp.GameTurnManager.StopCharacter(CardOwner.Player);
    }

    private void OnSettlementFinished(object arg)
    {
        CardGameView view = GameApp.ViewManager.GetView<CardGameView>((int)ViewType.CardGameView);
        view.RefreshAllInfo();
        view.ShowScore(true);
    }

    private void OnReStartGame(object[] arg)
    {
        GameApp.GameTurnManager.ReStartGame();

        CardGameView view = GameApp.ViewManager.GetView<CardGameView>((int)ViewType.CardGameView);
        if(view != null)
        {
            view.RefreshAllInfo();
        }
    }

    private void OnPlayerNeedAction(object arg)
    {
        CardGameView view = GameApp.ViewManager.GetView<CardGameView>((int)ViewType.CardGameView);

        view.SetDrawCardBtnHighlighted(true);
    }
}
