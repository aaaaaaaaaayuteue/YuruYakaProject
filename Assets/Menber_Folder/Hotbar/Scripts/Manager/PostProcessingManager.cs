using Hotbar.Base;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hotbar.Manager
{
    public class PostProcessingManager : SingletonMonobase<PostProcessingManager>
    {
        [Header("Postprocessing Volume")]
        [SerializeField] private Volume volume;

        public static void Initialize()
        {

        }
    }
}
