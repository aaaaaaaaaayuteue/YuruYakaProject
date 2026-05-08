using Hotbar.Procedural;
using Hotbar.Type;
using UnityEngine;

namespace Hotbar.Procedural
{
    public class Light : ProceduralObject
    {
        [Header("Dictionary")]
        public HotbarDictionary<ProceduralManager.LightType, GameObject> lightDictionary = new HotbarDictionary<ProceduralManager.LightType, GameObject>();

        [Header("Type")]
        public ProceduralManager.LightType lightType;

        public void ChangeLight(ProceduralManager.LightType lightType)
        {
            if (lightDictionary.ContainsKey(lightType) == false)
            {
                Debug.LogError($"[Change Light] => The light type {lightType} is not exist!");
                return;
            }

            this.lightType = lightType;

            switch (lightType)
            {
                case ProceduralManager.LightType.Default:
                    lightDictionary[lightType].transform.localScale = new Vector3(1, 0.1f, 0.5f);
                    break;
                default:
                    break;
            }

            foreach (var target in lightDictionary.ToDictionary())
            {
                lightDictionary[target.Key].gameObject.SetActive(false);
            }

            lightDictionary[lightType].SetActive(true);
        }
    }
}

