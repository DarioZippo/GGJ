#if NODECANVAS && LOCALIZATION

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Pearl.NodeCanvas.Tasks.Conditions
{
    [Category("Pearl")]
    public class LocalizationCheck : ConditionTask<PearlFSMOwner>
    {
        // aspetta la localizzazione
        public BBParameter<bool> isFinished = true;

        protected override bool OnCheck()
        {
            if (isFinished.value)
            {
                return LocalizationManager.IsDone();
            }
            else
            {
                return PlayerPrefs.HasKey("Language");
            }
        }
    }
}

#endif