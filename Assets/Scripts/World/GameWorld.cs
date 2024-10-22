using System.Collections.Generic;
using System.Linq;
using Configs;
using Mining;
using UnityEngine;
using TheSTAR.Data;

namespace World
{
    public class GameWorld : MonoBehaviour
    {
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private ResourceSource[] sources = new ResourceSource[0];
        [SerializeField] private Player playerPrefab;
        [SerializeField] private int levelIndex;
    
        public Player CurrentPlayer { get; private set; }

        private MiningController _miningController;
        private DropItemsContainer _dropItemsContainer;
        private TransactionsController _transactions;

        public void Init(DropItemsContainer dropItemsContainer, MiningController miningController, TransactionsController transactions, DataController data)
        {
            _miningController = miningController;
            _dropItemsContainer = dropItemsContainer;
            _transactions = transactions;
            
            if (CurrentPlayer != null) Destroy(CurrentPlayer.gameObject);
            SpawnPlayer();

            SourceType sourceType;
            SourceData sourceData;
            foreach (var source in sources)
            {
                sourceType = source.SourceType;
                sourceData = _miningController.SourcesConfig.SourceDatas[(int)sourceType];
                source.Init(sourceData, dropItemsContainer.DropFromSenderToPlayer, (s) =>
                {
                    CurrentPlayer.StopInteract();
                    _miningController.StartSourceRecovery(s);
                },
                CurrentPlayer.RetryInteract);
            }
        }
    
        private void SpawnPlayer()
        {
            CurrentPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity, transform);
            CurrentPlayer.Init(_dropItemsContainer);
        }
    }
}