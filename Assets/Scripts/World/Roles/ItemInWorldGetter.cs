using System.Collections.Generic;
using UnityEngine;

public class ItemInWorldGetter
{
    private List<ItemInWorld> _availableItemsInWorld;

    public void Init()
    {
        _availableItemsInWorld = new();
    }

    public void AddAvailableItem(ItemInWorld item)
    {
        if (_availableItemsInWorld.Contains(item)) return;
        _availableItemsInWorld.Add(item);

        Debug.Log("Items: " + _availableItemsInWorld.Count);
    }

    public void RemoveAvailableItem(ItemInWorld item)
    {
        if (!_availableItemsInWorld.Contains(item)) return;
        _availableItemsInWorld.Remove(item);

        Debug.Log("Items: " + _availableItemsInWorld.Count);
    }

    public void GetItem(ItemInWorld item)
    {
        
    }
}