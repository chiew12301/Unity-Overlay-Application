using UnityEngine;

namespace COREGAME
{
    public class CubeDragObject : MonoBehaviour, IDragObect
    {
        [SerializeField] private Animator m_animator = null;
        [SerializeField] private MeshRenderer m_meshRenderer = null;

        private Color m_defaultColor = Color.white;

        public bool isDragging { get; private set; }

        public Vector3 position { get
            {
                return this.transform.position;
            }
        }

        //===============================================================

        private void Start()
        {
            if(this.m_animator == null)
            {
                this.m_animator = this.GetComponent<Animator>();
            }

            if(this.m_meshRenderer == null)
            {
                this.m_meshRenderer = this.GetComponent<MeshRenderer>();
            }
            this.m_defaultColor = this.m_meshRenderer.material.GetColor("_Color");
        }

        //===============================================================
        public void OnBeginDrag(Vector3 worldPosition, Vector3 offset)
        {
            this.isDragging = true;
            this.transform.position = worldPosition + offset;
            this.m_meshRenderer.material.SetColor("_Color", Color.red);
        }

        public void OnBeginRelease()
        {
            this.isDragging = false;
            this.m_meshRenderer.material.SetColor("_Color", this.m_defaultColor);
        }

        public void OnDrag(Vector3 targetPosition)
        {
            this.transform.position = targetPosition;
        }

        public void OnRelease()
        {
        }

        //===============================================================

    }
}