using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using World;
//using Newtonsoft;
//using Newtonsoft.Json;

namespace TheSTAR.Data
{
    public sealed class DataController : MonoBehaviour
    {
        private const string GameDataFileName = "game_data.json";
        
        public GameData gameData = new();

        [SerializeField] private bool clearData = false;

        #region Unity Methodes

        private void Awake()
        {
            if (clearData) LoadDefault();
            else Load();
        }
        
        #endregion

        private string GameDataPath => Path.Combine(Application.persistentDataPath, GameDataFileName);
        
        [ContextMenu("Save")]
        public void Save()
        {
            //JsonSerializerSettings settings = new() { TypeNameHandling = TypeNameHandling.Objects };
            //string jsonString = JsonConvert.SerializeObject(gameData, Formatting.Indented, settings);
            //File.WriteAllText(GameDataPath, jsonString);
        }

        [ContextMenu("Load")]
        private void Load()
        {
            if (File.Exists(GameDataPath))
            {
                //string jsonString = File.ReadAllText(GameDataPath);
                //gameData = JsonConvert.DeserializeObject<GameData>(jsonString, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
            }
            else LoadDefault();
        }

        [ContextMenu("ClearData")]
        private void LoadDefault()
        {        
            gameData = new();
            Save();
        }

        [Serializable]
        public class GameData
        {
            public Dictionary<ItemType, int> items = new();
            public Dictionary<string, bool> completedTutorials = new();
            public Dictionary<int, Dictionary<int, int>> levelFactoryData = new();

            public void AddItems(ItemType itemType, int count, out int result)
            {
                if (items.ContainsKey(itemType)) items[itemType] += count;
                else items.Add(itemType, count);

                result = (int)items[itemType];
            }

            public int GetItemCount(ItemType itemType)
            {
                if (items.ContainsKey(itemType)) return items[itemType];
                else return 0;
            }

            public void CompleteTutorial(string id)
            {
                if (!IsTutorialComplete(id)) completedTutorials.Add(id, true);
            }

            public bool IsTutorialComplete(string id)
            {
                bool contains = completedTutorials.ContainsKey(id);

                if (contains) return completedTutorials[id];

                return false;
            }

            private Dictionary<int, int> GetFactoriesInLevelData(int levelIndex)
            {
                if (!levelFactoryData.ContainsKey(levelIndex)) levelFactoryData.Add(levelIndex, new Dictionary<int, int>());
                return levelFactoryData[levelIndex];
            }

            public void AddItemToFactoryStorage(int levelIndex, int factoryIndex, int value)
            {
                var factoriesInLevelData = GetFactoriesInLevelData(levelIndex);

                if (factoriesInLevelData.ContainsKey(factoryIndex)) factoriesInLevelData[factoryIndex] += value;
                else factoriesInLevelData.Add(factoryIndex, value);
            }

            public void EmptyFactoryStorage(int levelIndex, int factoryIndex) => SetItemToFactoryStorage(levelIndex, factoryIndex, 0);

            public void SetItemToFactoryStorage(int levelIndex, int factoryIndex, int value)
            {
                var factoriesInLevelData = GetFactoriesInLevelData(levelIndex);

                if (factoriesInLevelData.ContainsKey(factoryIndex)) factoriesInLevelData[factoryIndex] = value;
                else factoriesInLevelData.Add(factoryIndex, value);
            }

            public int GetFactoryStorageValue(int levelIndex, int factoryIndex)
            {
                var factoriesInLevelData = GetFactoriesInLevelData(levelIndex);

                if (factoriesInLevelData.ContainsKey(factoryIndex)) return factoriesInLevelData[factoryIndex];
                else return 0;
            }
        }

        [Serializable]
        public class ItemCountData
        {
            public ItemType ItemType;
            public int Count;

            public ItemCountData(ItemType itemType, int count)
            {
                ItemType = itemType;
                Count = count;
            }
        }
    }
}