using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using World;
using Random = UnityEngine.Random;
using TheSTAR.Utility;
using Mining;

public class DropItemsContainer : MonoBehaviour
{
    private IDropSender _playerDropSender;
    private IDropReceiver _playerDropReceiver;
    private ItemsController items;
    private TransactionsController _transactions;
    
    private Dictionary<ItemType, List<ResourceItem>> _itemPools;

    private float _dropWaitAfterCreateTime = 0.2f;
    private const float FlyToReceiverTime = 0.5f;
    private readonly Vector3 _standardDropOffset = new Vector3(0, 3, -2);
    
    private float _randomOffsetRange = 0.4f;

    private Action _onFailDropToFactoryAction;

    private Vector3 CreateItemPosOffset => _standardDropOffset +
        new Vector3(
            Random.Range(-_randomOffsetRange, _randomOffsetRange), 
            Random.Range(-_randomOffsetRange, _randomOffsetRange),
            Random.Range(-_randomOffsetRange, _randomOffsetRange));

    public void Init(
        TransactionsController transactions, 
        ItemsController miningController, 
        IDropSender playerDropSender, 
        IDropReceiver playerDropReceiver, 
        Action onFailDropToFactoryAction)
    {
        _transactions = transactions;
        _playerDropSender = playerDropSender;
        _playerDropReceiver = playerDropReceiver;
        _itemPools = new Dictionary<ItemType, List<ResourceItem>>();
        items = miningController;

        _onFailDropToFactoryAction = onFailDropToFactoryAction;
    }
    
    public void DropFromSenderToPlayer(IDropSender sender, ItemType dropItemType)
    {
        var offset = CreateItemPosOffset;
        DropItemTo(dropItemType, sender, _playerDropReceiver, () =>
        {   
            _transactions.AddItem(dropItemType);
            sender.OnCompleteDrop();
        });
    }
    
    private void DropItemTo(ItemType itemType, IDropSender sender, IDropReceiver receiver, Action completeAction = null)
    {
        var startPos = sender.SendPos.position;

        sender.OnStartDrop();
        receiver.OnStartReceiving();
        
        var item = GetItemFromPool(itemType, startPos);
        item.transform.localScale = Vector3.zero;
        
        LeanTween.scale(item.gameObject, Vector3.one, 0.2f).setOnComplete(() => TimeUtility.Wait(_dropWaitAfterCreateTime, FlyToReceiver));

        void FlyToReceiver()
        {
            LeanTween.value(0, 1, FlyToReceiverTime).setOnUpdate((value) =>
            {
                var way = receiver.transform.position - startPos;
                item.transform.position = startPos + value * (way);
                
                // physic imitation
                var impulseForce = items.ItemsConfig.Items[(int)itemType].PhysicalImpulse;
                var dopValueY = Math.Abs((value * value - value) * impulseForce);
                item.transform.position += new Vector3(0, dopValueY, 0);

            }) .setOnComplete(() =>
            {
                item.gameObject.SetActive(false);
                completeAction?.Invoke();
                receiver.OnCompleteReceiving();
            });
        }
    }

    public void DropToStorage(ItemType itemType, Storage storage)
    {
        _transactions.ReduceItem(itemType, 1, true,
        () => DropItemTo(itemType, _playerDropSender, storage));
    }
    
    private ResourceItem GetItemFromPool(ItemType itemType, Vector3 startPos, bool autoActivate = true)
    {
        if (_itemPools.ContainsKey(itemType))
        {
            var pool = _itemPools[itemType];
            var itemInPool = pool?.Find(info => !info.gameObject.activeSelf);
            if (itemInPool != null)
            {
                if (autoActivate) itemInPool.gameObject.SetActive(true);
                itemInPool.transform.position = startPos;
                return itemInPool;
            }
            
            var newItem = CreateItem();
            pool.Add(newItem);
            return newItem;
        }
        else
        {
            var newItem = CreateItem();
            _itemPools.Add(itemType, new List<ResourceItem>(){newItem});
            return newItem;
        }

        ResourceItem CreateItem() => Instantiate(items.GetResourceItemPrefab(itemType), startPos, quaternion.identity, transform);
    }
}

public interface IDropSender
{
    Transform SendPos {get;}
    void OnStartDrop();
    void OnCompleteDrop();
}

public interface IDropReceiver
{
    Transform transform { get; }

    void OnStartReceiving();
    void OnCompleteReceiving();
}