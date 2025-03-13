using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotWin : MonoBehaviour
{
    private float actionDelay = 0.5f;
    private BoardManager boardManager;

    void Start()
    {
        boardManager = BoardManager.Instance;
        StartCoroutine(WaitForBoardToLoad());
    }

    private IEnumerator WaitForBoardToLoad()
    {
        yield return new WaitUntil(() => boardManager.itemOnBoard.Count > 0);
        StartCoroutine(AutoPlay());
    }

    private IEnumerator AutoPlay()
    {
        while (boardManager != null && !boardManager.isWin)
        {
            yield return StartCoroutine(SelectAndAddItem());
            yield return new WaitForSeconds(actionDelay);
        }
    }

    private IEnumerator SelectAndAddItem()
    {
        if (boardManager.boardContainer.transform.childCount == 0)
        {
            yield break;
        }

        GameObject chooseItem = boardManager.boardContainer.transform.GetChild(0).gameObject;
        if (chooseItem == null) yield break;

        boardManager.AddItem(chooseItem);
        yield return new WaitForSeconds(actionDelay);

        List<GameObject> matchingItems = new List<GameObject>();
        for (int i = 1; i < boardManager.boardContainer.transform.childCount; i++)
        {
            GameObject item = boardManager.boardContainer.transform.GetChild(i).gameObject;
            if (item == null || chooseItem == null) continue;
            if (item.GetComponent<ItemController>().item.id == chooseItem.GetComponent<ItemController>().item.id)
            {
                matchingItems.Add(item);
                if (matchingItems.Count == 2) break;
            }
        }

        if (matchingItems.Count < 2) yield break;

        foreach (GameObject item in matchingItems)
        {
            boardManager.AddItem(item);
            yield return new WaitForSeconds(actionDelay);
        }

        yield return new WaitForSeconds(actionDelay);
        boardManager.RemoveItem();
    }
}
