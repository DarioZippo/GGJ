using UnityEngine;

namespace Pearl
{
    public class Dynamic3DSortingLayerManager : DynamicSortingLayerManager
    {
        [SerializeField]
        protected float minStep = 0.01f;

        private float _currentStep;
        private Transform _camTransform;

        public void Awake()
        {
            _currentStep = 1f / minStep;

            SetCameraTransform();
        }

        public override void SetCamera(Camera newCam = null)
        {
            base.SetCamera(newCam);

            SetCameraTransform();
        }

        public override int CalculateDistance(SortingOrderData sorter)
        {
            if (_camTransform == null)
            {
                return 0;
            }

            var newVector = _camTransform.InverseTransformPoint(sorter.transform.position);
            return Mathf.FloorToInt((-newVector.z * _currentStep));
        }

        private void SetCameraTransform()
        {
            if (cam != null)
            {
                _camTransform = cam.transform;
            }
        }
    }
}
