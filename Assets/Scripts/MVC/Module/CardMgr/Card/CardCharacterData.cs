using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCharacterData
{
    public List<CardData> cards {  get; private set; }

    //获取第一张牌
    public CardData FirstCard
    {
        get
        {
            if(cards.Count == 0)
            {
                return null;
            }
            return cards[0];
        }
    }

    public int totalScore { get; private set; }

    public int redCount {  get; private set; }

    public int blackCount {  get; private set; }

    public bool isStop { get; private set; }

    public CardCharacterData()
    {
        cards = new List<CardData>();
    }

    public void DrawCard(CardData card)
    {
        if (cards == null)
        {
            return;
        }

        cards.Add(card);
        totalScore += card.value;

        if(card.color == CardColor.red)
        {
            redCount++;
        }
        else
        {
            blackCount++;
        }
    }

    public void Stop()
    {
        isStop = true;
    }

    public void Reset()
    {
        cards.Clear();

        totalScore = 0;
        redCount = 0;
        blackCount = 0;
        isStop = false;

    }
}
