#if NODECANVAS && LOCALIZATION

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Pearl.NodeCanvas.Tasks.Conditions
{
    [Category("Pearl")]
    public class ListLanguagesCheck : ConditionTask<PearlFSMOwner>
    {
        protected override bool OnCheck()
        {
            return LocalizationManager.AreThereMoreLanguages();
        }
    }
}

#endif