using UnityEngine;

namespace Hotbar.Base
{
    public class SingletonMonobase<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        private static bool _isShuttingDown = false;

        public static T Instance
        {
            get
            {
                if (_isShuttingDown)
                {
                    Debug.LogWarning($"[{typeof(T)}] Exiting...");
                    return null;
                }

                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        _instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return _instance;
            }
        }

        public virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _isShuttingDown = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
