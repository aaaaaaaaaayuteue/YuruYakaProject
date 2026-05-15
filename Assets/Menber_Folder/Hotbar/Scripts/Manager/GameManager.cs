using Hotbar.Base;
using Hotbar.Type;
using System.Collections;
using UnityEngine;

namespace Hotbar.Manager
{
    public class GameManager : SingletonMonobase<GameManager>
    {
        private HotbarDictionary<string, bool> initializeTask = new HotbarDictionary<string, bool>();

        #region Initialize Task
        public float GetInitializeTaskProgress()
        {
            var progress = 0f;
            var taskCount = initializeTask.Count;
            var currentCount = 0f;

            if (taskCount == 0)
                return 0;

            foreach (var task in initializeTask.ToDictionary())
            {
                if(task.Value == true)
                {
                    currentCount++;
                }
            }

            progress = (float)currentCount / taskCount;

            return progress;
        }

        public bool IsInitializedTask() => GetInitializeTaskProgress() >= 1;

        #endregion
        private void Awake() => StartCoroutine(Initialize());
        private void StartGame() => StageManager.StartGame();
        private void Update()
        {
            
        }
        public IEnumerator Initialize()
        {
            if(initializeTask != null)
            {
                initializeTask = new HotbarDictionary<string, bool>();
            }
            yield return null;
            Debug.Log("[GameManager] => Initialize Start");

            Debug.Log("[GameManager] => Initialize [ObjectPoolingManager]");
            ObjectPoolingManager.Initialize(() => 
            {
                Debug.Log("[GameManager] => Initialize End [ObjectPoolingManager]");
            });

            Debug.Log("[GameManager] => Initialize [CameraManager]");
            CameraManager.Initialize();
            Debug.Log("[GameManager] => Initialize End [CameraManager]");

            UIManager.Initialize();

            Debug.Log("[GameManager] => Initialize [PostProcessing]");
            PostProcessingManager.Initialize();
            Debug.Log("[GameManager] => Initialize End [PostProcessing]");

            Debug.Log("[GameManager] => Initialize [MonsterManager]");
            MonsterManager.Initialize();
            Debug.Log("[GameManager] => Initialize End [MonsterManager]");

            Debug.Log("[GameManager] => Initialize [StageManager]");
            StageManager.Initialize();
            Debug.Log("[GameManager] => Initialize End [StageManager]");

            yield return new WaitUntil(() => IsInitializedTask() == true);
            Debug.Log("[GameManager] => Initialize End");
            StartGame();
        }
    }
}

