using System;
using Configs;
using TheSTAR.Input;
using UnityEngine;
using UnityEngine.AI;
using World;

public class Player : MonoBehaviour, ICameraFocusable, IJoystickControlled, IDropReceiver, IDropSender
{
    [SerializeField] private NavMeshAgent meshAgent;
    [SerializeField] private Transform visualTran;
    [SerializeField] private EntranceTrigger trigger;

    private ItemInWorldGetter itemInWorldGetter;
    private Storage currentStorage;
    public Transform SendPos => transform;

    private bool _isMoving = false;
    public event Action OnMoveEvent;

    private const string CharacterConfigPath = "Configs/CharacterConfig";
    
    private CharacterConfig _characterConfig;

    private CharacterConfig CharacterConfig
    {
        get
        {
            if (_characterConfig == null) _characterConfig = Resources.Load<CharacterConfig>(CharacterConfigPath);
            return _characterConfig;
        }
    }

    private DropItemsContainer drop;

    // todo use DI
    public void Init(DropItemsContainer drop)
    {
        this.drop = drop;

        itemInWorldGetter = new();
        itemInWorldGetter.Init(drop);

        trigger.Init(OnEnter, OnExit);
        trigger.SetVisionDistance(CharacterConfig.TriggerRadius);
    }

    #region Move

    public void JoystickInput(Vector2 input)
    {
        Vector3 finalMoveDirection;

        if (input.x != 0 || input.y != 0)
        {
            var tempMoveDirection = new Vector3(input.x, 0, input.y);
            finalMoveDirection = transform.position + tempMoveDirection;
            
            if (!_isMoving) OnStartMove();

            OnMove();

            // rotate

            var lookRotation = Quaternion.LookRotation(tempMoveDirection);
            var euler = lookRotation.eulerAngles;
            visualTran.rotation = Quaternion.Euler(0, euler.y, 0);
        }
        else
        {
            finalMoveDirection = transform.position;
            
            if (_isMoving) OnStopMove();
        }

        meshAgent.SetDestination(finalMoveDirection);
    }

    private void OnStartMove()
    {
        _isMoving = true;
    }
    
    private void OnMove() => OnMoveEvent?.Invoke();
    
    private void OnStopMove()
    {
        _isMoving = false;
        RetryInteract();
    }

    #endregion

    #region Interactions

    private void OnEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            var i = other.GetComponent<ItemInWorld>();
            itemInWorldGetter.AddAvailableItem(i);
        }
        else if (other.CompareTag("Storage"))
        {
            var s = other.GetComponent<Storage>();
            currentStorage = s;
        }
    }

    private void OnExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            var i = other.GetComponent<ItemInWorld>();
            itemInWorldGetter.RemoveAvailableItem(i);
        }
        else if (other.CompareTag("Storage"))
        {
            currentStorage = null;
        }
    }

    public void StopInteract()
    {
    }

    public void RetryInteract()
    {
        if (currentStorage != null)
        {
            drop.DropToStorage(ItemType.Log, currentStorage);
            return;
        }

        itemInWorldGetter.RetryInteract(out bool successful);
        if (successful) return;
    }

    #endregion

    public void OnStartReceiving() {}

    public void OnCompleteReceiving() {}

    public void OnStartDrop() {}

    public void OnCompleteDrop() {}
}