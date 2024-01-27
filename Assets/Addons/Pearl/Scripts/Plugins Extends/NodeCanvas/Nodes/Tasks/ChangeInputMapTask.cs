#if NODECANVAS

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pearl.Input;
using System.Collections.Generic;

namespace Pearl.NodeCanvas.Tasks
{
    [Category("Pearl")]
    public class ChangeInputMapTask : ActionTask
    {
        #region Inspector fields
        [RequiredField]
        public BBParameter<string> newMap;

        public BBParameter<bool> UIEnable = true;

        public BBParameter<List<string>> mapForAbilitate = null;
        public BBParameter<List<string>> mapForDisabilitate = null;
        #endregion

        #region Unity CallBacks
        protected override void OnExecute()
        {
            var UIEnableValue = UIEnable == null || UIEnable.value;
            if (newMap != null && mapForAbilitate != null && mapForDisabilitate != null)
            {
                InputManager.SetSwitchMap(newMap.value, UIEnableValue, mapForAbilitate.value, mapForDisabilitate.value);
            }
            EndAction();
        }
        #endregion
    }
}

#endif
