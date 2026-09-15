using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardGameView : BaseView
{
    private TextMeshProUGUI playerInfo;
    private TextMeshProUGUI enemyInfo;

    private Button startBtn;
    private Button restartBtn;
    private Button drawCardsBtn;
    private Button stopBtn;

    private Image drawCardsBtnHighlight;

    private Tween drawCardsHighlightTween;

    public float HighlightMinAlpha = 0f;
    public float HighlightMaxAlpha = 1f;
    public float HighlightFadeDuration = 0.55f;

    public override void Init()
    {
        base.Init();
        playerInfo = Find<TextMeshProUGUI>("bg/playerInfo/txt");
        enemyInfo = Find<TextMeshProUGUI>("bg/enemyInfo/txt");
        startBtn = Find<Button>("bg/startBtn");
        restartBtn = Find<Button>("bg/restartBtn");
        drawCardsBtn = Find<Button>("bg/drawCardsBtn");
        stopBtn = Find<Button>("bg/stopBtn");
        drawCardsBtnHighlight = Find<Image>("bg/drawCardsBtn/highlight");

        enemyInfo.gameObject.SetActive(false);
        startBtn.onClick.AddListener(OnStartBtn);
        drawCardsBtn.onClick.AddListener(OnDrawCardsBtn);
        restartBtn.onClick.AddListener(OnRestartBtn);
        stopBtn.onClick.AddListener(OnStopBtn);
    }


    private void OnStartBtn()
    {
        if (GameApp.CommandManager.IsRunningCommand)
        {
            return;
        }
        startBtn.gameObject.SetActive(false);
        restartBtn.gameObject.SetActive(true);

        GameApp.GameTurnManager.ChangeState(GameState.Player);
    }

    private void OnRestartBtn()
    {
        if (GameApp.CommandManager.IsRunningCommand)
        {
            return;
        }
  
        ApplyFunc(Defines.OnReStartGame);
        ShowScore(false);
    }

    private void OnDrawCardsBtn()
    {
        SetDrawCardBtnHighlighted(false);
        ApplyFunc(Defines.OnDrawPlayerCard);
    }

    private void OnStopBtn()
    {
        SetDrawCardBtnHighlighted(false);
        ApplyFunc(Defines.OnCharacterStop);
    }

    public void RefreshAllInfo()
    {
        RefreshPlayerInfo();
        RefreshEnemyInfo();
    }

    public void RefreshPlayerInfo()
    {
        CardCharacterData data = GameApp.GameTurnManager.Player;

        playerInfo.text = $"{data.totalScore}";
    }

    public void RefreshEnemyInfo()
    {
        CardCharacterData data = GameApp.GameTurnManager.Enemy;

        enemyInfo.text = $"{data.totalScore}";
    }
    
    public void ShowScore(bool isShow)
    {
        if(isShow)
        {
            enemyInfo.gameObject.SetActive(true);
        }
        else
        {
            enemyInfo.gameObject.SetActive(false);
        }
    }

    public void SetDrawCardBtnHighlighted(bool enabled)
    {
        if(enabled)
        {
            StartDrawCardsHighlight();
        }
        else
        {
            StopDrawCardsHighlight();
        }
    }

    private void StartDrawCardsHighlight()
    {
        StopDrawCardsHighlight();

        drawCardsBtnHighlight.gameObject.SetActive(true);
        SetHighlightAlpha(HighlightMinAlpha);

        drawCardsHighlightTween = drawCardsBtnHighlight
        .DOFade(HighlightMaxAlpha, HighlightFadeDuration)
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo)
        .SetUpdate(true)//让动画忽略 Time.timeScale，使用非缩放时间（unscaled time） 来更新。默认情况下，DOTween 动画会受游戏暂停影响：当 Time.timeScale = 0（例如打开暂停菜单、游戏暂停）时，动画也会停止。
        //加上 SetUpdate(true) 后，该动画会使用 Time.unscaledDeltaTime 更新，即使游戏暂停，动画依然会继续播放。

        .SetLink(drawCardsBtnHighlight.gameObject, LinkBehaviour.KillOnDestroy);//将 Tween 的生命周期绑定到某个 GameObject 上。当该 GameObject 被销毁时，自动执行指定的行为。
    }

    private void StopDrawCardsHighlight()
    {
        if (drawCardsBtnHighlight == null)
        {
            return;
        }

        drawCardsHighlightTween?.Kill();
        drawCardsHighlightTween = null;

        SetHighlightAlpha(0f);
        drawCardsBtnHighlight.gameObject.SetActive(false);
    }

    private void SetHighlightAlpha(float alpha)
    {
        Color color = drawCardsBtnHighlight.color;
        color.a = alpha;
        drawCardsBtnHighlight.color = color;
    }
}
