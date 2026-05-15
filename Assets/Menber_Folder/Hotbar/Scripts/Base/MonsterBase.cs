using Hotbar.Manager;
using System;
using UnityEngine;

namespace Hotbar.Base
{
    public class MonsterBase : ObjectBase
    {
        [Space(10)]
        [Header("Monster State")]
        [SerializeField] private MonsterManager.MonsterType monsterType;

        public MonsterManager.MonsterType MonsterType => monsterType;
        public override void Initialize(Action onEnd = null)
        {
            base.Initialize(onEnd);
        }
    }
}

