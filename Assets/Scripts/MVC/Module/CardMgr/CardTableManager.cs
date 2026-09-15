using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTableManager
{
    private GameObject cardPrefab;

    private Transform deckArea;
    private Transform playerArea;
    private Transform enemyArea;

    private List<CardObj> playerCards;
    private List<CardObj> enemyCards;

    private float normalSpacing = 0.3f;
    private float maxWidth = 6f;
    private float stackOffset = 0.003f;

    private float drawDuration = 0.45f;
    private float layoutDuration = 0.25f;

    private CardObj deck;

    public CardTableManager()
    {
        playerCards = new List<CardObj>();
        enemyCards = new List<CardObj>();
    }

    public void BindTable(GameObject cardPrefab, Transform deckArea, Transform playerArea, Transform enemyArea)
    {
        this.cardPrefab = cardPrefab;
        this.deckArea = deckArea;
        this.playerArea = playerArea;
        this.enemyArea = enemyArea;
    }

    public void ShowDeck()
    {
        if(this.deck != null)
        {
            return;
        }

        GameObject deckVisual = GameObject.Instantiate(cardPrefab, deckArea);
        deck = deckVisual.GetComponent<CardObj>();
        deck.transform.localPosition = Vector3.zero;
        deck.transform.localRotation = Quaternion.identity;
        deck.InitDeckVisual(GameApp.CardManager.GetBackSprite());
    }

    public void DrawCard(CardData data, CardOwner owner, bool faceUp, System.Action callback)
    {
        GameObject cardObj = GameObject.Instantiate(cardPrefab, deckArea);
        cardObj.transform.localPosition = Vector3.zero;
        cardObj.transform.localRotation = Quaternion.identity;

        CardObj card = cardObj.GetComponent<CardObj>();
        Sprite front = GameApp.CardManager.GetFrontSprite(data);
        Sprite back = GameApp.CardManager.GetBackSprite();
        card.Init(data, front, back);



        if(owner == CardOwner.Player)
        {
            card.transform.SetParent(playerArea, true);
            playerCards.Add(card);
            RefreshLayout(playerCards, card, faceUp, callback);
        }
        else
        {
            card.transform.SetParent(enemyArea, true);
            enemyCards.Add(card);
            RefreshLayout(enemyCards, card, faceUp, callback);
        }
    }

    private void RefreshLayout(List<CardObj> cards, CardObj newCard, bool faceUp, System.Action callback)
    {
        if(cards.Count == 0)
        {
            return;
        }

        float spacing = CalSpacing(cards.Count);
        float width = (cards.Count - 1) * spacing;
        float startX = -width * 0.5f;

        for(int i = 0; i < cards.Count; i++)
        {
            Vector3 targetLocalPos = new Vector3(startX + i * spacing, i * stackOffset, 0f);

            Vector3 targetLocalEuler = Vector3.zero;

            cards[i].SetSortingOrder(i);

            if (cards[i] == newCard)
            {
                cards[i].MoveTo(targetLocalPos, targetLocalEuler, drawDuration, faceUp, callback);
            }
            else
            {
                cards[i].MoveTo(targetLocalPos, targetLocalEuler, layoutDuration, false, null);
            }
        }
    }

    private float CalSpacing(int count)
    {
        //之后牌多需要计算得到更小的间隔来适应
        return normalSpacing;
    }

    /// <summary>
    /// 结算时翻开牌
    /// </summary>
    /// <param name="callback"></param>
    public void RevealAllCards(System.Action callback)
    {
        List<CardObj> hiddenCards = new List<CardObj>();

        CollectHiddenCard(enemyCards, hiddenCards);

        if(hiddenCards.Count == 0)
        {
            callback?.Invoke();
            return;
        }

        RevealNext(hiddenCards, callback);
    }

    private void CollectHiddenCard(List<CardObj> source, List<CardObj> result)
    {
        foreach(var card in source)
        {
            if(card != null && !card.isFaceUp)
            {
                result.Add(card);
            }
        }
    }

    private void RevealNext(List<CardObj> cards, System.Action callback)
    {
        for(int i = 0; i < cards.Count; i++)
        {
            cards[i].Flip(true);
        }
        callback?.Invoke();
    }

    public void Clear()
    {
        foreach(var card in playerCards)
        {
            if(card != null)
            {
                GameObject.Destroy(card.gameObject);
            }
        }
        foreach(var card in enemyCards)
        {
            if(card != null)
            {
                GameObject.Destroy(card.gameObject);
            }
        }

        playerCards.Clear();
        enemyCards.Clear();

        if(deck != null)
        {
            GameObject.Destroy(deck.gameObject);
            deck = null;
        }
    }
}
