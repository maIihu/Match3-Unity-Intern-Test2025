using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomController : MonoBehaviour
{
    private static BottomController _instance;
    public static BottomController Instance { get { return _instance; } }

    Dictionary<int, int> items;


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

    public void AddAnyItem()
    {
        for (int i = 0; i < transform.childCount; i++) {
            ItemController item = transform.GetChild(i).GetComponent<ItemController>();
            if (item != null)
            {
                if (items.ContainsKey(item.item.id)) {
                    items[item.item.id]++;
                    continue;
                }
                items.Add(item.item.id, 1);
            }
        }
    }
}
