﻿#if NODECANVAS

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pearl.Debugging;

namespace Pearl.NodeCanvas.Tasks.Conditions
{
    [Category("Pearl")]
    public class DebugCheck : ConditionTask
    {
        public BBParameter<string> debugString;

        protected override bool OnCheck()
        {
            if (debugString != null && debugString.value.IsNotNull(out string debug))
            {
                return DebugManager.GetActiveDebug(debug);
            }
            return false;
        }
    }
}

#endif