#if PEARL_2DSIDE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl.Side2D
{
    public abstract class ControlAbilityNative : PearlBehaviour
    {
        protected bool _blockInput = false;

        public bool BlockInput { set { _blockInput = value; } }

        public virtual void UpdateInput()
        {

        }
    }
}

#endif
