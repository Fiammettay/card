using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public enum GameState
{
    Idle,
    Enter,
    Player,
    Enemy,
    Settlement,
    GameOver
}

public class GameTurnManager
{
    public GameState state = GameState.Idle;

    public GameUnitBase Current { get; private set; }//当前所处回合

    public CardCharacterData Player { get; private set; }

    public CardCharacterData Enemy { get; private set; }

    public CardRule Rule {  get; private set; }


    public GameTurnManager()
    {
        Player = new CardCharacterData();
        Enemy = new CardCharacterData();
        Rule = new CardRule();
    }

    public void Update(float dt)
    {
        if ((Current != null && Current.Update(dt) == true))
        {
            //当前战斗单元仍在运行
        }
        else
        {
            Current = null;
        }
    }

    public void ChangeState(GameState state)
    {
        GameUnitBase _current = Current;
        this.state = state;
        switch(this.state)
        {
            case GameState.Idle:
                _current = new GameIdleUnit();
                break;
            case GameState.Enter:
                _current = new GameEnterUnit();
                break;
            case GameState.Player:
                _current = new GamePlayerUnit();
                break;
            case GameState.Enemy:
                _current = new GameEnemyUnit();
                break;
            case GameState.Settlement:
                _current = new GameSettlementUnit();
                break;
            case GameState.GameOver:
                _current = new GameOverUnit();
                break;
        }

        _current.Init();

        Current = _current;
    }

    public void EnterGame()
    {
        Player.Reset();
        Enemy.Reset();

        GameApp.CardTableManager.Clear();

        GameApp.CardManager.CreateDeck();
        GameApp.CardTableManager.ShowDeck();
    }

    /// <summary>
    /// 处理抽完牌的下一个行动
    /// </summary>
    /// <param name="owner"></param>
    public void OnDrawFinished(CardOwner owner)
    {
        //检查花色，是否爆炸
        CardCharacterData data = (owner == CardOwner.Player) ? Player : Enemy;

        if(Rule.IsBust(data))
        {
            BeginSettle();
            return;
        }

        ContinueAction(owner);
    }

    /// <summary>
    /// 处理玩家或敌人选择停止抽牌的下一步行动
    /// </summary>
    /// <param name="owner"></param>
    public void StopCharacter(CardOwner owner)
    {
        CardCharacterData data;
        if(owner == CardOwner.Player)
        {
            data = Player;
        }
        else
        {
            data = Enemy;
        }

        data.Stop();

        GameApp.MsgCenter.PostEvent(Defines.OnFinishDrawCard, owner);

        ContinueAction(owner);
    }

    /// <summary>
    /// 某方完成后另一方动作
    /// </summary>
    /// <param name="owner"></param>
    private void ContinueAction(CardOwner owner)
    {
        //进入双方停止的正常结算
        if(Rule.CanSettle(Player, Enemy))
        {
            BeginSettle();
            return;
        }

        if(owner == CardOwner.Player)
        {
            BeginEnemyTurn();
        }
        else
        {
            BeginPlayerTurn();
        }
    }

    private void BeginSettle()
    {
        if(state == GameState.Settlement || state == GameState.GameOver)
        {
            return;
        }

        ChangeState(GameState.Settlement);

        GameApp.CommandManager.AddCommand(new RevealCardCommand());
    }

    private void BeginPlayerTurn()
    {
        //玩家停止了不再进入玩家回合
        if(Player.isStop)
        {
            BeginEnemyTurn();
            return;
        }
        ChangeState(GameState.Player);
    }


    private void BeginEnemyTurn()
    {
        if(Enemy.isStop)
        {
            BeginPlayerTurn();
            return;
        }
        ChangeState(GameState.Enemy);
    }

    /// <summary>
    /// 翻开所有牌后
    /// </summary>
    public void RevealFinished()
    {
        GameApp.MsgCenter.PostEvent(Defines.OnSettlementFinished);
        ChangeState(GameState.GameOver);
    }

    /// <summary>
    /// 当前是否需要玩家进行抽牌或停牌操作
    /// </summary>
    public bool CanPlayerAction()
    {
        // 必须已经真正进入玩家回合
        if (state != GameState.Player)
        {
            return false;
        }

        // 玩家已经停止，不应该再进行操作
        if (Player.isStop)
        {
            return false;
        }

        // 任意一方爆牌，或者双方都停止，应该进入结算
        if (Rule.CanSettle(Player, Enemy))
        {
            return false;
        }

        return true;
    }

    public void ReStartGame()
    {
        GameApp.CommandManager.Clear();

        GameApp.CardTableManager.Clear();

        Player.Reset();
        Enemy.Reset();

        GameApp.CardManager.CreateDeck();

        Current = null;
        state = GameState.Idle;

        GameApp.CardTableManager.ShowDeck();

        ChangeState(GameState.Player);
    }
}
