using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Hotbar.Base
{
    public class JellyBase : MonoBehaviour
    {
        public float huwahuwaMove = 1.0f;
        public float huwahuwaValue = 1.0f;

        RectTransform rect;
        Vector2 basePos;

        Vector3 cashingScale = Vector3.one;

        void Start()
        {
            rect = GetComponent<RectTransform>();
            basePos = rect.anchoredPosition;

            Do();
        }

        private void Do()
        {
            cashingScale = rect.localScale;

            Sequence seq = DOTween.Sequence();
            float moveY = Random.Range(-30f * huwahuwaMove, 5f * huwahuwaMove);
            float scaleX = Random.Range(1 + (0.02f * huwahuwaValue), 1 + (0.07f  * huwahuwaValue));
            float scaleY = Random.Range(1 - (0.07f * huwahuwaValue), 1 - (0.02f * huwahuwaValue));
            float duration = Random.Range(1.0f, 1.3f);

            seq.Append(
                rect.DOAnchorPosY(basePos.y + moveY, duration)
                    .SetEase(Ease.OutQuad)
            );

            seq.Join(
                rect.DOScale(new Vector3(cashingScale.x *  scaleX, cashingScale.y * scaleY, 1f), duration)
                    .SetEase(Ease.OutQuad)
            );

            seq.Append(
                rect.DOAnchorPosY(basePos.y, duration)
                    .SetEase(Ease.InQuad)
            );

            seq.Join(
                rect.DOScale(new Vector3(cashingScale.x, cashingScale.y, 0), duration)
                    .SetEase(Ease.InQuad)
            );

            seq.OnComplete(Do);
        }
    }

}
