using System;
using UnityEngine;
using World;

public class ItemInWorld : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    public ItemType ItemType => itemType;

    public void OnGetItem()
    {
        gameObject.SetActive(false);
    }
}