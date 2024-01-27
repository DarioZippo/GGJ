#if NODECANVAS

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pearl.Input;
using System.Collections.Generic;
using UnityEngine;
using static Pearl.Input.PearlVirtualMouse;

namespace Pearl.NodeCanvas.Tasks
{
    [Category("Pearl")]
    public class VirtualMouseManagerTask : ActionTask
    {
        #region Inspector fields
        public BBParameter<bool> changeState;
        [Conditional("changeState", 1)]
        public BBParameter<VirtualMouseEnum> state;
        #endregion

        #region Unity CallBacks
        protected override void OnExecute()
        {
            PearlVirtualMouse.Abilitate();

            if (changeState != null && state != null && changeState.value)
            {
                PearlVirtualMouse.State = state.value;
            }

            EndAction();
        }
        #endregion
    }
}

#endif
