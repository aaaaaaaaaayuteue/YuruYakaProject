using UnityEngine;

namespace Hotbar.Base
{
    public class ObjectBase : MonoBehaviour
    {
        [Header("[Status]")]
        [SerializeField] private bool isInitialize = false; 
        public virtual void Initialize(System.Action onEnd = null)
        {
            Show();
            isInitialize = true;

            if (onEnd != null)
            {
                onEnd();
            }
        }
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}

