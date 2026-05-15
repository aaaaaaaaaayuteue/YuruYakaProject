using DG.Tweening;
using Hotbar.Base;
using UnityEngine;

namespace Hotbar.Manager
{
    public class CameraManager : SingletonMonobase<CameraManager>
    {
        public static void Initialize()
        {
            IsInitialize = true;
        }
        private void LateUpdate()
        {
            
        }
        public void ShakePosition(System.Action onEnd = null, float duration = 1.0f, float strength = 1.0f, int vibrato = 10, int randomness = 90, bool snapping = false, bool fadeOut = true, ShakeRandomnessMode randomMode = ShakeRandomnessMode.Full)
        {
            transform.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut, randomMode).onComplete += () =>
            {
                if(onEnd != null) onEnd();
            };
        }
    }
}

