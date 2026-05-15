using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Hotbar.Base
{
    public class UIBase : MonoBehaviour
    {
        [Header("[UI Setting]")]
        public bool isAnimation = false;

        [Header("[Cashing]")]
        private Image icon;

        public virtual void Initialize()
        {
            icon = GetComponent<Image>();
        }

        #region State
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
        public void DoGrayScale(System.Action onComplete = null, bool isVisible = false, float duration = 1.0f)
        {
            if (icon == null) return;

            var color = icon.color;
            var targetAlpha = isVisible ? 1.0f : 0.0f;

            icon.DOColor(new Color(color.r, color.g, color.b, targetAlpha), duration).onComplete += () => 
            {
                if(onComplete != null) onComplete();
            };
        }
        #endregion
    }
}

