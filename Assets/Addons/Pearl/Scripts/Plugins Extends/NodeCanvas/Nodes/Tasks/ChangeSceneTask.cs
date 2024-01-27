#if NODECANVAS

using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Pearl.NodeCanvas.Tasks
{
    [Category("Pearl")]
    public class ChangeSceneTask : ActionTask
    {
        [RequiredField]
        public BBParameter<string> sceneStringContainer;

        public BBParameter<bool> isAsync;

        [Conditional("isAsync", 1)]
        public BBParameter<bool> useLoadingBar = false;

        public BBParameter<bool> saveScenePrev = false;
        [Conditional("saveScenePrev", 1)]
        public BBParameter<string> nameVarForScenePrev = string.Empty;

        protected override string info
        {
            get { return string.Format("Load Scene {0}", sceneStringContainer); }
        }

        protected override void OnExecute()
        {
            if (sceneStringContainer != null)
            {
                if (saveScenePrev.value)
                {
                    blackboard.UpdateVariable(nameVarForScenePrev.value, SceneSystemManager.CurrentScene);
                }


                if (isAsync.value)
                {
                    SceneSystemManager.EnterNewSceneAsync(sceneStringContainer.value, OnFinish, useLoadingBar.value);
                }
                else
                {
                    SceneSystemManager.EnterNewScene(sceneStringContainer.value);
                    EndAction();
                }
            }
        }

        private void OnFinish()
        {
            EndAction();
        }
    }
}

#endif
