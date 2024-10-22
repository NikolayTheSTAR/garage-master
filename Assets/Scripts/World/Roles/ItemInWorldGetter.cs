using System.Collections.Generic;
using UnityEngine;

public class ItemInWorldGetter
{
    private List<ItemInWorld> _availableItemsInWorld;

    private DropItemsContainer drop;

    public void Init(DropItemsContainer drop)
    {
        this.drop = drop;
        _availableItemsInWorld = new();
    }

    public void AddAvailableItem(ItemInWorld item)
    {
        if (_availableItemsInWorld.Contains(item)) return;
        _availableItemsInWorld.Add(item);
    }

    public void RemoveAvailableItem(ItemInWorld item)
    {
        if (!_availableItemsInWorld.Contains(item)) return;
        _availableItemsInWorld.Remove(item);
    }

    public void RetryInteract(out bool successful)
    {
        successful = false;

        if (_availableItemsInWorld.Count == 0) return;

        var item = _availableItemsInWorld[^1];
        drop.DropFromSenderToPlayer(item, item.ItemType);
        RemoveAvailableItem(item);

        successful = true;
    }
}