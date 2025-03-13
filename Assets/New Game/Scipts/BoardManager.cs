using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    private static BoardManager _instance;
    public static BoardManager Instance { get { return _instance; } }

    [SerializeField] private List<GameObject> itemPrefabs;
    public GameObject boardContainer;

    [SerializeField] private Transform bottomContainer;

    private int maxSlots = 5; 
    private float spacing = 2.0f;

    Dictionary<int, int> items;
    public List<GameObject> itemOnBoard;
    private Dictionary<GameObject, Vector3> previousPos = new Dictionary<GameObject, Vector3>();


    public bool isWin;
    public bool isLose;

    public static event Action IsWin;
    public static event Action IsLose;

    public bool timeAttackMode;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    private void Start()
    {
        items = new Dictionary<int, int>();
        itemOnBoard = new List<GameObject>();
        isWin = false;
        isLose = false;
        RenderBoard();
    }

    private void RenderBoard()
    {
        Tilemap tilemap = gameObject.GetComponentInChildren<Tilemap>();
        BoundsInt bound = tilemap.cellBounds;
        List<Vector3> availablePositions = new List<Vector3>();

        for (int x = bound.min.x; x <= bound.max.x; x++)
        {
            for (int y = bound.min.y; y <= bound.max.y; y++)
            {
                Vector3Int gridLocation = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(gridLocation))
                {
                    availablePositions.Add(tilemap.GetCellCenterWorld(gridLocation));
                }
            }
        }

        List<GameObject> itemList = new List<GameObject>();
        Dictionary<GameObject, int> itemCount = new Dictionary<GameObject, int>();
        foreach (GameObject itemPrefab in itemPrefabs)
        {
            for (int i = 0; i < 3; i++)
            {
                itemList.Add(itemPrefab);
            }
            itemCount[itemPrefab] = 3;
        }
        for (int i = 0; i < 3; i++)
        {
            GameObject randomItem = itemPrefabs[UnityEngine.Random.Range(0, itemPrefabs.Count)];
            itemList.Add(randomItem);
            itemList.Add(randomItem);
            itemList.Add(randomItem);
        }

        ShuffleList(itemList);
        for (int i = 0; i < itemList.Count; i++)
        {
            GameObject itemClone = Instantiate(itemList[i], availablePositions[i], Quaternion.identity, boardContainer.transform);
            itemOnBoard.Add(itemClone);
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]); // Hoán đổi vị trí
        }
    }



    private void Update()
    {
        if (timeAttackMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                if (hit.collider != null)
                {
                    GameObject gameObjectHit = hit.collider.gameObject;

                    if (gameObjectHit.transform.parent == bottomContainer)
                        UndoItem(gameObjectHit);                    
                    else
                        AddItem(gameObjectHit); 
                }
            }

            if (items.Values.Any(value => value >= 3))
            {
                RemoveItem();
            }
        }
    }
    public void UndoItem(GameObject obj)
    {
        if (!bottomContainer.GetChild(0)) return;

        obj.transform.SetParent(boardContainer.transform); 
        itemOnBoard.Add(obj); 

        if (previousPos.ContainsKey(obj))
        {
            obj.transform.position = previousPos[obj];
            previousPos.Remove(obj); 
        }

        ItemController item = obj.GetComponent<ItemController>();

        if (items.ContainsKey(item.item.id))
        {
            items[item.item.id]--;
            if (items[item.item.id] <= 0)
            {
                items.Remove(item.item.id);
            }
        }
    }


    public void RemoveItem() {
        List<int> keysToRemove = new List<int>();

        foreach (var item in items)
        {
            if (item.Value >= 3)
            {
                for (int i = 0; i < bottomContainer.childCount; i++)
                {
                    ItemController itemClone = bottomContainer.GetChild(i).GetComponent<ItemController>();
                    if (itemClone.item.id == item.Key)
                        itemClone.DestroyItem();
                }
                keysToRemove.Add(item.Key);
            }
        }

        foreach (int key in keysToRemove)
        {
            items.Remove(key);
        }
    }

    public void AddItem(GameObject obj)
    {
        if (bottomContainer.childCount >= maxSlots && !timeAttackMode)
        {
            isLose = true;
            IsLose?.Invoke();
            return;
        }

        if (!previousPos.ContainsKey(obj))
            previousPos[obj] = obj.transform.position; 

        obj.transform.SetParent(bottomContainer.transform);

        Vector3 newPosition = bottomContainer.transform.position + new Vector3((bottomContainer.childCount - 3) * spacing, 0, 0);
        obj.transform.position = newPosition;

        itemOnBoard.Remove(obj);
        if (itemOnBoard.Count <= 0)
        {
            isWin = true;
            IsWin?.Invoke();
            return;
        }

        ItemController item = obj.GetComponent<ItemController>();

        if (items.ContainsKey(item.item.id))
            items[item.item.id]++;
        else
            items.Add(item.item.id, 1);
    }

}
