using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KC_Custom
{
    public class KCUtil : MonobehaviourSingleton<KCUtil>
    {
        [Header("Settings")]
        [SerializeField] private LayerMask m_mouseColliderLayerMask = new LayerMask();

        //===================================================================================================================

        protected override void Update()
        {
            base.Update();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999.0f, this.m_mouseColliderLayerMask))
            {
                this.transform.position = raycastHit.point;
            }
        }

        //===================================================================================================================

        /// <summary>
        /// Only use this method if you have instance.
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetMouseWorldPositionWithRaycast() => GetInstance().GetMouseWorldPosition_Instance();

        private Vector3 GetMouseWorldPosition_Instance()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit raycastHit, 999.0f, this.m_mouseColliderLayerMask))
            {
                return raycastHit.point;
            }
            else
            {
                return Vector3.zero;
            }
        }

        /// <summary>
        /// This will automatically use Camera.main as camera target. Without Z Position.
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0.0f;
            return vec;
        }

        /// <summary>
        /// This will automatically use Camera.main as camera target. With Z Position.
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            return worldCamera.ScreenToWorldPoint(screenPosition);
        }

        /// <summary>
        /// Get is currently pointing toward an UI.
        /// </summary>
        /// <returns></returns>
        public static bool IsPointerOverUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }
            else
            {
                PointerEventData pe = new PointerEventData(EventSystem.current);
                pe.position = Input.mousePosition;

                var results = new List<RaycastResult>();

                EventSystem.current.RaycastAll(pe, results);
                return results.Count > 0;
            }
        }
        //===================================================================================================================
    }
}