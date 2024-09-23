using KC_Custom;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

namespace COREGAME
{
    public class InputDragHandler : MonobehaviourSingleton<InputDragHandler>
    {
        private Camera m_mainCamera;
        private Vector3 m_offset;

        private IDragObect m_currentDragObect = null;

        //===============================================================

        protected override void Start()
        {
            base.Start();
            this.m_mainCamera = Camera.main;
        }

        protected override void Update()
        {
            base.Update();
            // Check for mouse input and drag the object
            if (Input.GetMouseButtonDown(0)) // On left click
            {
                Ray ray = this.m_mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                IDragObect detectedObject = null;

                // Perform a raycast to check if we hit any object with a collider
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.TryGetComponent<IDragObect>(out detectedObject))
                    {
                        if(this.m_currentDragObect != null)
                        {
                            if(this.m_currentDragObect != detectedObject)
                            {
                                this.m_currentDragObect.OnBeginRelease();
                                this.m_currentDragObect = null;
                                this.m_currentDragObect = detectedObject;
                                this.m_offset = hit.transform.position - this.GetMouseWorldPosition(hit.transform.position);
                                this.m_currentDragObect.OnBeginDrag(this.GetMouseWorldPosition(hit.transform.position), this.m_offset);
                            }
                            else
                            {
                                //Do nothing
                            }
                        }
                        else
                        {
                            this.m_currentDragObect = detectedObject;
                            this.m_offset = hit.transform.position - this.GetMouseWorldPosition(hit.transform.position);
                            this.m_currentDragObect.OnBeginDrag(this.GetMouseWorldPosition(hit.transform.position), this.m_offset);
                        }
                    }
                    else
                    {
                        if (this.m_currentDragObect != null)
                        {
                            this.m_currentDragObect.OnBeginRelease();
                        }
                        this.m_currentDragObect = null;
                    }
                }
            }

            if (Input.GetMouseButton(0) & this.m_currentDragObect != null)
            {
                Vector3 tarpos = this.GetMouseWorldPosition(this.m_currentDragObect.position) + this.m_offset;
                this.m_currentDragObect.OnDrag(this.ClampPositionToCamera(tarpos));
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (this.m_currentDragObect != null)
                {
                    this.m_currentDragObect.OnBeginRelease();
                }
                this.m_currentDragObect = null;
            }
        }
        //===============================================================

        private Vector3 GetMouseWorldPosition(Vector3 targetPos)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = this.m_mainCamera.WorldToScreenPoint(targetPos).z;
            return this.m_mainCamera.ScreenToWorldPoint(mouseScreenPos);
        }

        private Vector3 ClampPositionToCamera(Vector3 targetPosition)
        {
            Vector3 viewportPosition = this.m_mainCamera.WorldToViewportPoint(targetPosition);

            viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.05f, 0.95f); // Add small margins to avoid object sticking on the edge
            viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.05f, 0.95f);

            return this.m_mainCamera.ViewportToWorldPoint(viewportPosition);
        }

        //===============================================================
    }
}