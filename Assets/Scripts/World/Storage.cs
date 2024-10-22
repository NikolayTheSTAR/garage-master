using System;
using UnityEngine;

namespace World
{
    [Serializable]
    public class Storage : MonoBehaviour, IDropReceiver
    {
        [SerializeField] private Collider ciCollider;

        public Collider Col => ciCollider;

        private int _itemsInStorageCount = 0;
        
        public event Action<int, int> OnAddItemToStorageEvent;
        public event Action<int> OnEmptyStorageEvent;

        public void OnStartReceiving() 
        {}

        public void OnCompleteReceiving()
        {
            _itemsInStorageCount++;
        }
    }
}