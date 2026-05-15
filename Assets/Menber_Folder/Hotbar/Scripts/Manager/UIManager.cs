using Hotbar.Base;
using Hotbar.Type;
using Mono.Cecil;
using UnityEngine;

namespace Hotbar.Manager
{
    public class UIManager : SingletonMonobase<UIManager>
    {
        #region Enum
        public enum UIType
        {
            None,
        }

        #endregion

        [Header("[Cashing]")]
        private HotbarDictionary<UIType, UIBase> uiDic = new HotbarDictionary<UIType, UIBase>(); 

        public static void Initialize(System.Action onEnd = null)
        {
            #region Find UI In Scene

            var list = GameObject.FindObjectsOfType<UIBase>();

            foreach (var ui in list)
            {
                var name = ui.name;
                var type = (UIType)System.Enum.Parse(typeof(UIType), name);

                if(Instance.uiDic.ContainsKey(type) == false)
                {
                    Instance.uiDic.Add(type, ui);
                }
            }

            #endregion

            IsInitialize = true;

            if(onEnd != null)
            {
                onEnd();
            }
        }
        public static UIBase GetUI(UIType Type)
        {
            if (Instance.uiDic.ContainsKey(Type) == false)
                return null;

            return Instance.uiDic[Type];
        }
    }
}

