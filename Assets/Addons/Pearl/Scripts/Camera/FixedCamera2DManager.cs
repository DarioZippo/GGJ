using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl
{
    [RequireComponent(typeof(Camera))]
    public class FixedCamera2DManager : MonoBehaviour
    {
        #region Inspector Field
        [SerializeField]
        private SpriteRenderer target = default;
        [SerializeField]
        private Vector2 positionAt00 = Vector3.zero;

        [SerializeField]
        private int indexRow = 0;
        [SerializeField]
        private int indexColumn = 0;
        #endregion

        #region Private Fields
        private Camera _camera;
        private Vector2 _extensCamera;
        private Vector2 _sizeCamera;
        private Transform targetTransform;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        private void Start()
        {
            InitializeFollowCamera();
            ChangePositionCamera();
        }

        // Update is called once per frame
        private void Update()
        {
            FollowTarget();
        }

        private void OnValidate()
        {
            InitializeFollowCamera();
            ChangePositionCamera();
        }
        #endregion

        #region Private Methods
        private void InitializeFollowCamera()
        {
            if (target != null)
            {
                targetTransform = target.transform;
            }
            if (_camera != null || TryGetComponent<Camera>(out _camera))
            {
                _extensCamera = _camera.Extents();
                _sizeCamera = _extensCamera * 2;
            }
        }

        private void FollowTarget()
        {
            if (target != null)
            {
                var boundsTarget = target.bounds;
                if (_camera != null && !_camera.IsSaw(boundsTarget))
                {
                    Vector2 positionTarget = (Vector2) targetTransform.position + _extensCamera - positionAt00;

                    float auxColumn = positionTarget.x / _sizeCamera.x;
                    float auxRow = positionTarget.y / _sizeCamera.y;

                    indexColumn = auxColumn >= 0 ? Mathf.FloorToInt(auxColumn) : -Mathf.CeilToInt(-auxColumn); 
                    indexRow = auxRow >= 0 ? Mathf.FloorToInt(auxRow) : -Mathf.CeilToInt(-auxRow);

                    ChangePositionCamera();
                }
            }
        }

        private void ChangePositionCamera()
        {
            Vector3 newPosition = new(positionAt00.x + (_sizeCamera.x * indexColumn), positionAt00.y + (_sizeCamera.y * indexRow), -10);
            transform.position = newPosition;
        }
        #endregion
    }
}
