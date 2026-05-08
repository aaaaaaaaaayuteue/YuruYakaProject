using Hotbar.Base;
using Hotbar.Manager;
using UnityEngine;
using UnityEngine.Pool;

namespace Hotbar.Procedural
{
    public class ProceduralManager : SingletonMonobase<ProceduralManager>
    {
        public enum ProceduralType
        {
            None,
            Light,
            Wall,
            End,
        }

        public enum WallType
        {
            Up,
            Down,
            Front,
            Back,
            Left,
            Right,
        }

        public enum LightType
        {
            Default,
        }

        public enum TextureType
        {
            WALL_DEFAULT = 0,
            WALL_RED = 1,
            WALL_GREEN = 2,
            WALL_BLUE = 3,
        }


        public enum DecalType
        {
            Default,
        }

        public static ProceduralObject Spawn(ProceduralType proceduralType)
        {
            var key = proceduralType.ToString();
            var instance = ObjectPoolingManager.Instance.Do_SpawnFromPool(key);
            var script = instance.GetComponent<ProceduralObject>();

            if(script == null)
            {
                script = instance.AddComponent<ProceduralObject>();
            }

            script.SetDefault()
            .Build();

            return script;
        }
    }
}
