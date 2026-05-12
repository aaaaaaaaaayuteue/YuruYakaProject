using Hotbar.Base;
using UnityEngine;

namespace Hotbar.Manager
{
    public class MonsterManager : SingletonMonobase<MonsterManager>
    {
        public enum MonsterType
        {
            None,
            Angel,
            End = Angel,
        }

        public static void Initialize()
        {
            IsInitialize = true;
        }
    }
}

