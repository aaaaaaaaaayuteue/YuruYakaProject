using Hotbar.Base;
using Hotbar.Type;
using System.Collections.Generic;
using UnityEngine;

namespace Hotbar.Manager
{
    public class MonsterManager : SingletonMonobase<MonsterManager>
    {
        [Header("Cashing")]
        [SerializeField] private HotbarDictionary<MonsterType, MonsterBase> monsterDic = new HotbarDictionary<MonsterType, MonsterBase>();

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
        public MonsterBase GetMonster(MonsterType monsterType)
        {
            if (monsterDic.ContainsKey(monsterType) == false)
                return null;

            return monsterDic[monsterType];
        }
    }
}

