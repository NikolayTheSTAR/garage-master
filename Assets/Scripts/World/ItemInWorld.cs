using System;
using UnityEngine;
using World;

public class ItemInWorld : MonoBehaviour, IDropSender
{
    [SerializeField] private ItemType itemType;
    public ItemType ItemType => itemType;
    public Transform SendPos => transform;

    public void OnStartDrop()
    {
        gameObject.SetActive(false);
    }

    public void OnCompleteDrop()
    {}
}