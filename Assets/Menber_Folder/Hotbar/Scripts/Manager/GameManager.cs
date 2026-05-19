using Hotbar.Base;
using Hotbar.Type;
using System.Collections;
using UnityEngine;

namespace Hotbar.Manager
{
    public class GameManager : SingletonMonobase<GameManager>
    {
        public enum TaskType
        {
            Data,
            Resource,
            Stage,
        }

        private HotbarDictionary<TaskType, bool> initializeTask = new HotbarDictionary<TaskType, bool>();

        #region Unity FSM
        private void Awake() => StartCoroutine(Initialize());

        private void Update()
        {
            var progress = GetInitializeTaskProgress() * 100 + "%";
            Debug.Log("Current Progress => " + progress);
        }
        #endregion

        #region Initialize
        private void StartGame() => StageManager.StartGame();
        public IEnumerator Initialize()
        {
            AddTask(TaskType.Data);
            AddTask(TaskType.Resource);
            AddTask(TaskType.Stage);
            yield return null;

            Debug.Log("[GameManager] => Initialize Start");

            DataManager.Initialize(() => 
            {
                ChangeTaskState(TaskType.Data, true);

                ObjectPoolingManager.Initialize();
                PostProcessingManager.Initialize();
                CameraManager.Initialize();
                UIManager.Initialize();
                ChangeTaskState(TaskType.Resource, true);

                MonsterManager.Initialize();
                StageManager.Initialize();
                ChangeTaskState(TaskType.Stage, true);
            });

            yield return new WaitUntil(() => IsInitializedTask() == true);
            Debug.Log("[GameManager] => Initialize Finished");

            StartGame();
        }
        #endregion

        #region Task
        public void AddTask(TaskType key)
        {
            if (initializeTask.ContainsKey(key))
                return;

            initializeTask.Add(key, false);
        }
        public void ChangeTaskState(TaskType key, bool state)
        {
            if (initializeTask.ContainsKey(key) == false)
            {
                Debug.LogError("Not Exist Key => " + key.ToString());
                return;
            }

            initializeTask[key] = true; 
        }

        public float GetInitializeTaskProgress()
        {
            var progress = 0f;
            var taskCount = initializeTask.Count;
            var currentCount = 0f;

            if (taskCount == 0)
                return 0;

            foreach (var task in initializeTask.ToDictionary())
            {
                if (task.Value == true)
                {
                    currentCount++;
                }
            }

            progress = (float)currentCount / taskCount;

            return progress;
        }
        public bool IsInitializedTask() => GetInitializeTaskProgress() >= 1;
        #endregion

    }
}

