using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl
{
    public abstract class InpactAbstractInterface<T> : MonoBehaviour, IReset
    {
        [SerializeField]
        private ColliderResponseManager colliderResponseManager = null;

        [SerializeField]
        protected bool forFastObjects = false;

        [SerializeField]
        protected bool disable = false;

        [SerializeField]
        protected bool checkDisable = false;

        protected List<Tuple<T, GameObject>> _activeObjs = new();

        protected virtual void Reset()
        {
            colliderResponseManager = GetComponent<ColliderResponseManager>();
        }

        protected virtual void Awake()
        {
            if (colliderResponseManager)
            {
                colliderResponseManager.OnDisableResponse += ForceExit;
            }
        }

        protected virtual void FixedUpdate()
        {
            if (checkDisable && colliderResponseManager)
            {
                for (int i = _activeObjs.Count - 1; i >= 0; i--)
                {
                    var obj = _activeObjs[i];
                    if (!IsEnabled(obj.Item1) || !obj.Item2.activeSelf)
                    {
                        OnExit(obj.Item1, obj.Item2);
                    }
                }
            }
        }


        private void OnDisable()
        {
            ForceExit();
        }

        protected void OnStay(T element, GameObject obj)
        {
            if (!disable && element != null && _activeObjs != null)
            {
                CollisionEvent(obj, TriggerType.Stay);
            }
        }

        protected void OnEnter(T element, GameObject obj)
        {
            if (!disable && element != null && _activeObjs != null)
            {
                if (CollisionEvent(obj, TriggerType.Enter))
                {
                    _activeObjs.Add(new Tuple<T, GameObject>(element, obj));
                }
                else
                {
                    _activeObjs.Remove(new Tuple<T, GameObject>(element, obj));
                }
            }
        }

        protected void OnExit(T element, GameObject obj)
        {
            if (!disable && CollisionEvent(obj, TriggerType.Exit))
            {
                _activeObjs.Remove(new Tuple<T, GameObject>(element, obj));
            }
        }


        private bool CollisionEvent(GameObject obj, TriggerType type)
        {
            if (colliderResponseManager)
            {
                if (type == TriggerType.Enter)
                {
                    return colliderResponseManager.Inpact(obj);
                }
                else if (type == TriggerType.Exit)
                {
                    return colliderResponseManager.ExitInpact(obj);
                }
                else
                {
                    return colliderResponseManager.StayInpact(obj);
                }
            }
            return false;
        }

        private void ForceExit()
        {
            foreach (var obj in _activeObjs)
            {
                colliderResponseManager.ExitInpact(obj.Item2, true);
            }

            _activeObjs.Clear();
        }

        protected abstract bool IsEnabled(T item);

        public void ResetElement()
        {
            _activeObjs?.Clear();
        }

        public void DisableElement()
        {
        }
    }
}
