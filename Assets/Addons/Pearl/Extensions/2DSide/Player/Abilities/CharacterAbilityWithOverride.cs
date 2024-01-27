#if PEARL_2DSIDE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl.Side2D
{
    public abstract class CharacterAbilityWithOverride<T> : CharacterAbility where T : class
    {
        /// the initial parameters
        [Tooltip("the initial parameters")]
        public T DefaultParameters;

        protected T _overrideParameters;

        protected override void Awake()
        {
            base.Awake();

            ResetInfo();
        }


        public void AddOverrideInfo(T newInfo)
        {
            _overrideParameters = newInfo;
        }

        public void ResetInfo()
        {
            _overrideParameters = DefaultParameters;
        }  
    }
}

#endif
