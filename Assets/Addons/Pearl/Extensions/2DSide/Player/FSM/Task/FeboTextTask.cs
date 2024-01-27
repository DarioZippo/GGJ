using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pearl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOM
{
    [Category("Game")]
    public class FeboTextTask : ActionTask
    {
        [RequiredField]
        public BBParameter<string> dialogString;
        [RequiredField]
        public BBParameter<bool> wait;

        protected override void OnExecute()
        {
            if (dialogString != null)
            {
                //FeboTextManager.NewEvent(dialogString.value);
            }

            if (wait != null && wait.value)
            {
                //WaitManager.Wait(typeof(FeboTextManager), OnComplete);
            }
            else
            {
                EndAction();
            }
        }

        private void OnComplete()
        {
            EndAction();
        }
    }
}
