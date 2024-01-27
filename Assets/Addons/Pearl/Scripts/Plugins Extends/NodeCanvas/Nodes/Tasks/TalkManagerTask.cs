#if NODECANVAS && INK

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pearl.Ink;
using UnityEngine;

namespace Pearl
{
    [Category("Pearl")]
    public class TalkManagerTask : ActionTask<Transform>
    {
        public enum TalkAction { Talk, SetVar, ChangeDialog }

        public BBParameter<TalkAction> talkAction;

        [Conditional("talkAction", 1)]
        public BBParameter<string> varName;
        [Conditional("talkAction", 1)]
        public BBParameter<string> varValue;
        [Conditional("talkAction", 2)]
        public BBParameter<string> varDialog;

        [Conditional("talkAction", 0)]
        public BBParameter<bool> useSpecificDialog;
        [Conditional("talkAction", 0), Conditional("useSpecificDialog", 1)]
        public BBParameter<string> textName;
        [Conditional("talkAction", 0), Conditional("useSpecificDialog", 1)]
        public BBParameter<string> path;

        [Conditional("talkAction", 0)]
        public BBParameter<bool> waitFinishDialog;

        public BBParameter<bool> useAgent = true;
        [Conditional("useAgent", 0)]
        public BBParameter<Pearl.NodeCanvas.ComponentReference<TalkManager>> container;

        private TalkManager _talkManager;

        protected override void OnExecute()
        {
            if (!useAgent.value && container.value != null)
            {
                _talkManager = container.value.Component;
            }
            else
            {
                _talkManager = agent.GetComponent<TalkManager>();
            }

            if (_talkManager == null)
            {
                return;
            }


            if (talkAction.value == TalkAction.Talk)
            {
                if (useSpecificDialog.value)
                {
                    _talkManager.Story = new(textName.value, path.value);
                }

                if (waitFinishDialog.value)
                {
                    _talkManager.OnFinishDialogEvent += OnFinishDialog;
                }


                _talkManager.Talk();
            }
            else if (talkAction.value == TalkAction.SetVar && varName != null && varValue != null)
            {
                _talkManager.SetVar(varName.value, varValue.value);
            }
            else if (talkAction.value == TalkAction.ChangeDialog && varDialog != null)
            {
                _talkManager.ChangePath(varDialog.value);
            }

            if (talkAction.value != TalkAction.Talk || !waitFinishDialog.value)
            {
                EndAction();
            }
        }

        private void OnFinishDialog()
        {
            if (_talkManager)
            {
                _talkManager.OnFinishDialogEvent -= OnFinishDialog;
            }

            EndAction();
        }
    }
}

#endif