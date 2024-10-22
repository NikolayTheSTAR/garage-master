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
        [SerializeField] private Player playerPrefab;
        [SerializeField] private int levelIndex;
    
        public Player CurrentPlayer { get; private set; }

        private DropItemsContainer _dropItemsContainer;

        public void Init(DropItemsContainer dropItemsContainer)
        {
            _dropItemsContainer = dropItemsContainer;
            
            if (CurrentPlayer != null) Destroy(CurrentPlayer.gameObject);
            SpawnPlayer();
        }
    
        private void SpawnPlayer()
        {
            CurrentPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity, transform);
            CurrentPlayer.Init(_dropItemsContainer);
        }
    }
}