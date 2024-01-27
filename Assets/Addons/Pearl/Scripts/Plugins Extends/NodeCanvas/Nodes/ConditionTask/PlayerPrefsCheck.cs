#if NODECANVAS

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace Pearl.NodeCanvas.Tasks.Conditions
{
    //vede se c'è il playerprefs specifico
    [Category("Pearl")]
    public class PlayerPrefsCheck : ConditionTask
    {
        public BBParameter<string> nameVar;

        protected override bool OnCheck()
        {
            return PlayerPrefs.HasKey(nameVar.value);
        }
    }
}

#endif