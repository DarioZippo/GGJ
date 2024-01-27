#if NODECANVAS

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Pearl.NodeCanvas.Tasks.Conditions
{
    [Category("Pearl")]
    public class LandscapeCheck : ConditionTask<PearlFSMOwner>
    {
        protected override bool OnCheck()
        {
            return GameManager.IsLandscape();
        }
    }
}

#endif