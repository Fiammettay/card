using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class CardObj : MonoBehaviour
{
    [SerializeField] private SpriteRenderer front;
    [SerializeField] private SpriteRenderer back;

    private SortingGroup sortingGroup;

    private Tween moveTween;
    private Tween flipTween;

    private Vector3 originalScale;

    public CardData data { get; private set; }

    public bool isFaceUp {  get; private set; }

    private void Awake()
    {
        originalScale = transform.localScale;

        sortingGroup = GetComponent<SortingGroup>();

        if (sortingGroup == null)
        {
            sortingGroup = gameObject.AddComponent<SortingGroup>();
        }      
    }

    public void Init(CardData data, Sprite frontSprite, Sprite backSprite)
    {
        this.data = data;

        front.sprite = frontSprite;
        back.sprite = backSprite;

        // 新生成的牌初始显示背面
        SetFaceUp(false);
    }

    public void InitDeckVisual(Sprite backSprite)
    {
        data = null;

        back.sprite = backSprite;
        SetFaceUp(false);
    }

    public void MoveTo(
        Vector3 targetPos,
        Vector3 targetEuler,
        float duration,
        bool faceUp,
        System.Action callback)
    {
        moveTween?.Kill();

        Sequence seq = DOTween.Sequence();

        // 移动和旋转同时执行
        seq.Append(
            transform
                .DOLocalMove(targetPos, duration)
                .SetEase(Ease.OutCubic)
        );

        seq.Join(
            transform
                .DOLocalRotate(targetEuler, duration)
                .SetEase(Ease.OutCubic)
        );

        if (faceUp)
        {
            // 移动完成后，开始翻牌
            seq.Append(
                transform
                    .DOScaleX(0f, 0.15f)
                    .SetEase(Ease.InQuad)
            );

            seq.AppendCallback(() =>
            {
                SetFaceUp(true);
            });

            seq.Append(
                transform
                    .DOScaleX(originalScale.x, 0.15f)
                    .SetEase(Ease.OutQuad)
            );
        }

        /*
         * 现在 OnComplete 会等待：
         * 移动 + 旋转 + 翻牌全部完成。
         */
        seq.OnComplete(() =>
        {
            moveTween = null;
            callback?.Invoke();
        });

        moveTween = seq;
    }

    /// <summary>
    /// 单独使用的翻牌方法
    /// </summary>
    public void Flip(
        bool faceUp,
        System.Action callback = null)
    {
        flipTween?.Kill();

        Sequence seq = DOTween.Sequence();

        seq.Append(
            transform
                .DOScaleX(0f, 0.15f)
                .SetEase(Ease.InQuad)
        );

        seq.AppendCallback(() =>
        {
            SetFaceUp(faceUp);
        });

        seq.Append(
            transform
                .DOScaleX(originalScale.x, 0.15f)
                .SetEase(Ease.OutQuad)
        );

        seq.OnComplete(() =>
        {
            flipTween = null;
            callback?.Invoke();
        });

        flipTween = seq;
    }

    private void SetFaceUp(bool faceUp)
    {
        isFaceUp = faceUp;

        front.gameObject.SetActive(faceUp);
        back.gameObject.SetActive(!faceUp);
    }

    public void SetSortingOrder(int order)
    {
        if (sortingGroup != null)
        {
            sortingGroup.sortingOrder = order;
        }
    }

    private void OnDestroy()
    {
        moveTween?.Kill();
        flipTween?.Kill();
    }
}