using Hotbar.Base;
using System.Collections;
using UnityEngine;

namespace Hotbar.Manager
{
    public class GameManager : SingletonMonobase<GameManager>
    {
        private void Awake() => StartCoroutine(Initialize());

        public IEnumerator Initialize()
        {
            yield return null;

            ObjectPoolingManager.Initialize(() =>
            {
                PostProcessingManager.Initialize();
                MonsterManager.Initialize();
                StageManager.Initialize();

                StartGame();
            });
        }

        private void StartGame()
        {
            StageManager.StartGame();
        }
    }
}

