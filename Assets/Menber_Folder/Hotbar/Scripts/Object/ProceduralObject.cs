using UnityEngine;

namespace Hotbar.Procedural
{
    public class ProceduralObject : MonoBehaviour
    {
        #region Public Member
        [Header("Procedural Type")]
        public ProceduralManager.ProceduralType proceduralType;

        [Space(10), Header("Bound Size")]
        public Vector3 boundSize;
        #endregion

        #region Private Member
        private bool isInitialized = false;
        private MeshRenderer meshRenderer;
        #endregion

        #region Initialize

        public virtual void Awake()
        {
            var meshRenderer = GetComponent<MeshRenderer>();

            if(meshRenderer != null)
            {
                boundSize = meshRenderer.bounds.size;
            }
            else
            {
                boundSize = Vector3.zero;
            }

        }


        public virtual ProceduralObject Initialize(ProceduralManager.ProceduralType proceduralType = ProceduralManager.ProceduralType.None)
        {
            this.proceduralType = proceduralType;
            isInitialized = true;
            return this;
        }

        public virtual ProceduralObject SetDefault()
        {
            proceduralType = ProceduralManager.ProceduralType.None;
            isInitialized = false;

            return this;
        }



        public virtual void Build()
        {

        }
        #endregion

        #region Bound

        public virtual void SetBoundSize(Vector3 boundSize)
        {
            transform.localScale = boundSize;
        }
        public Vector3 GetBoundSize()
        {
            if (meshRenderer)
            {
                return meshRenderer.bounds.size;
            }
            else
            {
                meshRenderer = GetComponent<MeshRenderer>();

                if(meshRenderer)
                    return meshRenderer.bounds.size;
                else
                    return Vector3.zero;
            }
        }

        public Vector3 GetBoundsCenter()
        {
            if(meshRenderer != null)
            {
                return meshRenderer.bounds.center;
            }
            else
            {
                meshRenderer = GetComponent<MeshRenderer>();

                if(meshRenderer != null)
                {
                    return meshRenderer.bounds.center;
                }
                else
                {
                    return Vector3.zero;
                }
            }
        }
        #endregion

    }
}

