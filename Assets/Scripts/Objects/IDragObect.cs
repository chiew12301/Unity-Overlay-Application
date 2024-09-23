using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COREGAME
{
    public interface IDragObect
    {
        public bool isDragging { get; }
        public Vector3 position { get; }
        public void OnBeginDrag(Vector3 worldPosition, Vector3 offset);
        public void OnDrag(Vector3 targetPosition);
        public void OnBeginRelease();
        public void OnRelease();
    }
}