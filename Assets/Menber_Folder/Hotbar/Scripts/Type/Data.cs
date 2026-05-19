using Hotbar.Manager;
using System;
using UnityEngine;

namespace Hotbar.Data
{
    [System.Serializable]
    public class Data
    {
        public string timeLog;
        public int stage;

        public void Initialize()
        {
            timeLog = DataManager.GetCurrentTimeStr();
            stage = 1;
        }
    }
}
