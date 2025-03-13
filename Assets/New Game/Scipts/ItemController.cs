using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public ItemObject item;
    public void DestroyItem() {
        Destroy(this.gameObject);
    }
}
