using Hotbar.Manager;
using Hotbar.Type;
using UnityEngine;

namespace Hotbar.Balance
{
    [CreateAssetMenu(fileName = "[Monster Balance]" , menuName = "Hotbar/ScriptableObject")]
    public class MonsterBalance : ScriptableObject
    {
        [Header("Monster Status")]
        [SerializeField] private HotbarDictionary<MonsterManager.MonsterType, MonsterData> monsterData;
    }

    [System.Serializable]
    public class MonsterData
    {
        private string name;
        private float speed;
        public short hp;
        public string Name => name;
        public float Speed => speed;
        public short Hp => hp;
    }
}

