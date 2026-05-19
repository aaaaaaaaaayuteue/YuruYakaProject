using Hotbar.Base;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Hotbar.Manager
{
    public class PostProcessingManager : SingletonMonobase<PostProcessingManager>
    {
        [System.Serializable]
        public class PostProcessData
        {
            [Header("Volume")]
            [SerializeField] private Volume volume;

            [Header("Override Option")]
            private Bloom bloom;
            private WhiteBalance whiteBalance;
            private ColorAdjustments colorAdjustments;

            public void Initialize()
            {
                volume.profile.TryGet(out colorAdjustments);
                volume.profile.TryGet(out whiteBalance);
                volume.profile.TryGet(out bloom);
            }

            public Volume GetVolume() => volume;
            public Bloom GetBloom() => bloom;
            public WhiteBalance GetWhiteBalance() => whiteBalance;
            public ColorAdjustments GetColorAdjustments() => colorAdjustments;
        }

        [Header("Postprocessing Volume")]
        [SerializeField] private PostProcessData postProcessData;

        public static void Initialize()
        {
            Instance.postProcessData.Initialize();
        }
    }
}
