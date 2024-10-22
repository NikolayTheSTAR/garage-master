using System;
using Configs;
using Mining;
using UnityEngine;
using UnityEngine.UIElements;

namespace World
{
    [Serializable]
    public class ResourceSource : CiObject, IDropSender
    {
        [SerializeField] private SourceType sourceType;
        [SerializeField] private GameObject prolificVisual;
        [SerializeField] private GameObject emptyVisual;
        [SerializeField] private Collider ciCollider;

        private int _animLTID = -1;
        private Action<IDropSender, ItemType> _dropItemAction;
        private Action<ResourceSource> _onEmptying;
        private Action<ResourceSource> _onRecovery;
        private int _health = 1;
        private SourceData _sourceData;
        
        public event Action OnCompleteFarmEvent;

        public override CiType GetCiType => CiType.Source;
        public override Collider Col => ciCollider;
        public override bool CanInteract => !IsEmpty;
        public bool IsEmpty { get; private set; }

        public SourceType SourceType => sourceType;
        public SourceData SourceData => _sourceData;

        public void Init(SourceData sourceData, Action<IDropSender, ItemType> dropItemAction, Action<ResourceSource> onEmptying, Action<ResourceSource> onRecovery)
        {
            _sourceData = sourceData;
            _dropItemAction = dropItemAction;
            _onEmptying = onEmptying;
            _onRecovery = onRecovery;
            _health = sourceData.MiningData.MaxHitsCount;
        }
        
        public void TakeHit()
        {
            if (_health <= 0) return;

            _health--;

            BreakAnim();
            
            _animLTID =
            LeanTween.scaleY(gameObject, 0.85f, 0.1f).setOnComplete(() =>
            {
                _animLTID =
                LeanTween.scaleY(gameObject, 1f, 0.2f).id;
            }).id;
            
            for (var i = 0; i < _sourceData.MiningData.OneHitDropCount; i++) _dropItemAction?.Invoke(this, _sourceData.DropItemType);
            
            // emptying
            if (_health <= 0) Empty();
        }

        private void BreakAnim()
        {
            if (_animLTID == -1) return;
            
            LeanTween.cancel(_animLTID);
            transform.localScale = Vector3.one;
        }

        private void Empty()
        {
            prolificVisual.SetActive(false);
            emptyVisual.SetActive(true);
            IsEmpty = true;
            _onEmptying?.Invoke(this);
        }

        public void Recovery()
        {
            prolificVisual.SetActive(true);
            emptyVisual.SetActive(false);
            IsEmpty = false;
            _health = _sourceData.MiningData.MaxHitsCount;
            _onRecovery?.Invoke(this);
            
            // anim
            
            BreakAnim();
            _animLTID =
                LeanTween.scaleY(gameObject, 1.1f, 0.1f).setOnComplete(() =>
                {
                    _animLTID =
                        LeanTween.scaleY(gameObject, 1f, 0.2f).id;
                }).id;
        }
        
        public void OnCompleteDrop()
        {
            OnCompleteFarmEvent?.Invoke();
        }
    }

    public enum SourceType
    {
        AppleTree,
        IronMine,
        CrystalMine,
        WheatField,
        LogTree
    }
}