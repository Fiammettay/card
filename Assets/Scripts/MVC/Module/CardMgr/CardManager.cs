using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager
{
    private List<CardData> deck;

    private string FrontPathRoot = "Card/Front";

    private string BackPath = "Card/Back/Background";

    private Dictionary<string, Sprite> cardSpriteCache;

    private Sprite backSprite;

    public int remainingCount
    {
        get
        {
            return deck.Count;
        }
    }

    public CardManager()
    {
        deck = new List<CardData>();
        cardSpriteCache = new Dictionary<string, Sprite>();
    }

    public void CreateDeck()
    {
        deck.Clear();

        for(int value = 1; value <= 13; value ++)
        {
            deck.Add(new CardData(CardColor.red, value));
            deck.Add(new CardData(CardColor.black, value));
        }

        Shuffle();
    }

    public void Shuffle()
    {
        for(int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            CardData temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;//从0到i下标中选一个和deck[i]交换
        }
    }

    public CardData DrawCard()
    {
        if(deck.Count == 0)
        {
            return null;
        }

        CardData card = deck[deck.Count - 1];

        deck.RemoveAt(deck.Count - 1);

        return card;
    }

    /// <summary>
    /// 获取对应的正面图片
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public Sprite GetFrontSprite(CardData card)
    {
        if(cardSpriteCache.ContainsKey($"{card.color}{card.value}"))
        {
            return cardSpriteCache[$"{card.color}{card.value}"];
        }
        else
        {
            Sprite sprite = Resources.Load<Sprite>($"{FrontPathRoot}/{card.color}{card.value}");
            if(sprite == null)
            {
                Debug.Log("未找到卡牌正面图片");
                return null;
            }
            cardSpriteCache.Add($"{card.color}{card.value}", sprite);
            return sprite;
        }
    }

    public Sprite GetBackSprite()
    {
        if(backSprite != null)
        {
            return backSprite;
        }
        else
        {
            backSprite = Resources.Load<Sprite>($"{BackPath}");
            return backSprite;
        }
    }

    public void ClearCache()
    {
        cardSpriteCache.Clear();
        backSprite = null;
    }
}
