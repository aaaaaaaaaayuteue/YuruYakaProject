using Hotbar.Base;
using Unity.VisualScripting;
using UnityEngine;

namespace Hotbar.Manager
{
    public class StageManager : SingletonMonobase<StageManager>
    {
        private int currentStage;
        public static int Stage => Instance.currentStage;
        public static void Initialize()
        {
            Instance.currentStage = 1;

            IsInitialize = true;
        }
        public static void ChangeStage(int stage)
        {
            Instance.currentStage = stage;

            //TODO
        }
        public static void StartGame()
        {
            //TODO
            Debug.LogError("[Game Start]");
        }
    }
}

