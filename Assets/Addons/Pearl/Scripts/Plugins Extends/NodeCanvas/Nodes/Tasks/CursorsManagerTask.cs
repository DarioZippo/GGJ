#if NODECANVAS

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pearl.Input;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl.NodeCanvas.Tasks
{
    [Category("Pearl")]
    public class CursorsManagerTask : ActionTask
    {
        #region Inspector fields
        public BBParameter<bool> visible = false;
        #endregion

        #region Unity CallBacks
        protected override void OnExecute()
        {
            if (visible != null)
            {
                CursorManager.Visible = visible.value;
            }

            EndAction();
        }
        #endregion
    }
}

#endif
