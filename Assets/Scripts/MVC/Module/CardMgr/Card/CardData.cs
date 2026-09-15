using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardOwner
{
    Player,
    Enemy
}

public enum CardColor
{
    red,
    black
}

public class CardData
{
    public CardColor color;
    public int value;

    public CardData(CardColor color, int value)
    {
        this.color = color;
        this.value = value;
    }
}
