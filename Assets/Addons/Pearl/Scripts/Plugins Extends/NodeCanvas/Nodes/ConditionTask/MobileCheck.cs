#if NODECANVAS

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Pearl.NodeCanvas.Tasks.Conditions
{
    [Category("Pearl")]
    public class MobileCheck : ConditionTask<PearlFSMOwner>
    {
        protected override bool OnCheck()
        {
            return GameManager.IsMobile();
        }
    }
}

#endif

