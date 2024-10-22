using System;
using System.Collections.Generic;
using Configs;
using UnityEngine;
using World;

namespace Mining
{
    public class ItemsController : MonoBehaviour
    {
        private const string ItemsConfigLoadPath = "Configs/ItemsConfig";
        private ItemsConfig _itemsConfig;
        public ItemsConfig ItemsConfig
        {
            get
            {
                if (_itemsConfig == null) _itemsConfig = Resources.Load<ItemsConfig>(ItemsConfigLoadPath);
                return _itemsConfig;
            }
        }
        
        private Dictionary<ItemType, ResourceItem> _loadedItemPrefabs;

        private string ItemLoadPath(ItemType itemType) => $"Items/{itemType}";

        public void Init()
        {
            _loadedItemPrefabs = new Dictionary<ItemType, ResourceItem>();
        }
        
        public ResourceItem GetResourceItemPrefab(ItemType itemType)
        {
            if (_loadedItemPrefabs.ContainsKey(itemType)) return _loadedItemPrefabs[itemType];
            
            var loadedItem = Resources.Load<ResourceItem>(ItemLoadPath(itemType));
            _loadedItemPrefabs.Add(itemType, loadedItem);
            return loadedItem;
        }
    }
}