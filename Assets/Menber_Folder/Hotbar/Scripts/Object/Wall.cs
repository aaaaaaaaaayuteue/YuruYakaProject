using Hotbar.Type;
using UnityEngine;

namespace Hotbar.Procedural
{
    [System.Serializable]
    public class Wall : ProceduralObject
    {
        [Header("Materials")]
        public HotbarDictionary<ProceduralManager.TextureType, Material> materialDic = new HotbarDictionary<ProceduralManager.TextureType, Material>();

        #region Editor
        public void ChangeMaterial(ProceduralManager.TextureType materialType)
        {
            if(materialDic.ContainsKey(materialType) == false)
            {
                Debug.LogError("Not Exist Key : " + materialType.ToString());
                return;
            }

            var meshRenderer = GetComponent<MeshRenderer>();
            if(meshRenderer != null)
            {
                meshRenderer.material = materialDic[materialType];
            }
        }
        #endregion
    }
}

