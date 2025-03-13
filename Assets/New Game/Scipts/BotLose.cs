using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotLose : MonoBehaviour
{
    private float actionDelay = 0.5f;
    private BoardManager boardManager;
    List<int> itemOnBottom;
    int index = 0;

    void Start()
    {
        itemOnBottom = new List<int>();
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
        
        while (boardManager != null && !boardManager.isLose)
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

        GameObject chooseItem = boardManager.boardContainer.transform.GetChild(index).gameObject;

        if (chooseItem == null) yield break;

        ItemController item = chooseItem.GetComponent<ItemController>();

        if (itemOnBottom.Contains(item.item.id)) {
            index++;
            yield break;
        }
            
        boardManager.AddItem(chooseItem);
        itemOnBottom.Add(item.item.id);
        
    }

}
