using Pearl;
using UnityEngine;

namespace Pearl
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField]
        private Transform _cameraTransform;
        [SerializeField]
        private bool useCameraRotation = false;

        [SerializeField, ConditionalField("@rotationMode")]
        private bool x = true;
        [SerializeField, ConditionalField("@rotationMode")]
        private bool y = true;
        [SerializeField, ConditionalField("@rotationMode")]
        private bool z = true;

        private Transform _myTransform;

        private void Start()
        {
            Reset();
            _myTransform = transform;
        }

        private void Reset()
        {
            var camera = Camera.main;
            if (_cameraTransform == null && camera != null)
            {
                _cameraTransform = Camera.main.transform;
            }
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null || _myTransform == null) return;

            if (!useCameraRotation)
            {
                Vector3 v = _cameraTransform.position - _myTransform.position;
                v.x = v.z = 0.0f;
                _myTransform.LookAt(_cameraTransform.position - v);
            }
            else
            {
                Vector3 newRotation = _cameraTransform.eulerAngles;

                newRotation.x = x ? newRotation.x : 0;
                newRotation.y = y ? newRotation.y : 0;
                newRotation.z = z ? newRotation.z : 0;
                _myTransform.eulerAngles = newRotation;
            }
        }
    }
}