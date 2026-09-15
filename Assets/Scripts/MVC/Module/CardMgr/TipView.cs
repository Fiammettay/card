using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TipView : BaseView
{
    public override void Open(params object[] args)
    {
        base.Open(args);
        Find<TextMeshProUGUI>("content/txt").text = args[0].ToString();

        Action callback = null;
        if(args.Length > 1)
        {
            callback = args[1] as Action;
        }

        Sequence seq = DOTween.Sequence();
        seq.Append(Find("content").transform.DOScaleY(1, 0.5f)).SetEase(Ease.OutBack);//将该物体的y轴缩放从现在的值在0.15s内以outback的方式变为1
        seq.AppendInterval(0.75f);
        seq.Append(Find("content").transform.DOScaleY(0, 0.5f)).SetEase(Ease.Linear);

        seq.OnComplete(() =>
        {
            GameApp.ViewManager.Close(viewID);

            callback?.Invoke();
        });
    }
}
