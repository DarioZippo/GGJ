using UnityEngine;

namespace Pearl
{
    public class TriggerInterface : TriggerAbstractInterface<Collider>
    {
        [SerializeField]
        private Collider _myCollider = null;

        protected override void Awake()
        {
            base.Awake();

            if (_myCollider == null)
            {
                _myCollider = GetComponent<Collider>();
            }
        }

        protected override void Reset()
        {
            base.Reset();

            _myCollider = GetComponent<Collider>();
        }

        protected override bool IsTouching(Collider element)
        {
            if (_myCollider != null)
            {
                return _myCollider.IsTouching(element);
            }
            return false;
        }

        protected void OnTriggerEnter(Collider collider)
        {
            if (collider)
            {
                OnEnter(collider, collider.gameObject);
            }
        }

        protected void OnTriggerStay(Collider collider)
        {
            if (collider)
            {
                OnStay(collider, collider.gameObject);
            }
        }

        protected void OnTriggerExit(Collider collider)
        {
            if (collider)
            {
                OnExit(collider, collider.gameObject);
            }
        }

        protected override bool IsEnabled(Collider item)
        {
            return item != null && item.enabled;
        }
    }
}
