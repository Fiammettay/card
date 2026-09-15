using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : InteractableBase
{
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private Transform deckArea;
    [SerializeField] private Transform playerArea;
    [SerializeField] private Transform enemyArea;

    public override void Interact()
    {
        GameApp.CardTableManager.BindTable(cardPrefab, deckArea, playerArea, enemyArea);

        GameApp.GameTurnManager.ChangeState(GameState.Enter);
    }

}
