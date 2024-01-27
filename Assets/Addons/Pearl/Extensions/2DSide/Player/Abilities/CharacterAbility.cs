#if PEARL_2DSIDE

using Pearl;
using Pearl.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pearl.Side2D
{
    public abstract class CharacterAbility : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        protected Side2DController controller = null;
        [SerializeField]
        protected CharacterManager manager = null;

        [Header("General Info")]

        [SerializeField]
        private bool useOneState = true;  
        [ConditionalField("@useOneState"), SerializeField]
        protected string statePlayer = string.Empty;
        [SerializeField]
        protected bool activated = true;

        public bool Activated { get { return activated; } }

        #region Controller State
        public void NewControllerState(string newStateString, bool useForDefault = false)
        {
            if (manager != null)
            {
                if (useForDefault)
                {
                    SetOverrideDefaultState(newStateString);
                }

                manager.NewState(newStateString);
            }
        }

        public void SetOverrideDefaultState(string newStateString)
        {
            if (manager != null)
            {
                manager.OverrideDefaultState = newStateString;
            }
        }

        public void ResetOverrideDefaultState()
        {
            if (manager != null)
            {
                manager.ResetOverrideDefaultState();
            }
        }

        public void ResetCondition()
        {
            if (manager != null)
            {
                manager.ResetCondition();
            }
        }

        public bool CheckStateCondition(string status)
        {
            if (manager != null)
            {
                return manager.CheckStateCondition(status);
            }
            return false;
        }

        public bool CheckStateCondition()
        {
            return CheckStateCondition(statePlayer);
        }

        public void NewConditions(string newConditionString)
        {
            if (manager != null)
            {
                manager.NewCondition(newConditionString);
            }
        }

        public void ResetState(bool ignoreConflit = false)
        {
            if (manager != null)
            {
                manager.ResetState(ignoreConflit);
            }
        }

        public void ResetConditions(bool ignoreConflit = false)
        {
            if (manager != null)
            {
                manager.ResetCondition(ignoreConflit);
            }
        }
        #endregion

        #region Unity callbacks
        private void Reset()
        {
            controller = ComponentExtend.GetComponentOnlyInParent<Side2DController>(this);
            manager = ComponentExtend.GetComponentOnlyInParent<CharacterManager>(this);
        }

        protected virtual void Awake()
        {
            if (manager != null)
            {
                manager.AddAbility(this, GetComponent<ControlAbilityNative>());
            }
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
            ResetAbility();
            if (manager != null)
            {
                manager.RemoveAbility(this);
            }
        }
        #endregion

        public abstract void UpdateAbility();

        protected virtual void ResetAbility()
        {

        }

        public virtual void CheckNewPosition(ref Vector2 newPosition)
        {

        }
    }

}

#endif