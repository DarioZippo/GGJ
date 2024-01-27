using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl.NodeCanvas.Tasks
{
    [Category("Pearl")]
    public class SwitchCameraTask : ActionTask
    {
        [RequiredField]
        public BBParameter<Camera> currentCamera;
        [RequiredField]
        public BBParameter<Camera> newCamera;

        protected override void OnExecute()
        {
            if (currentCamera != null && currentCamera.value != null && newCamera != null && newCamera.value != null)
            {
                if (currentCamera.value.gameObject.tag == "MainCamera")
                {
                    currentCamera.value.enabled = false;
                }
                else
                {
                    currentCamera.value.gameObject.SetActive(false);
                }

                newCamera.value.enabled = true;
                newCamera.value.gameObject.SetActive(true);

            }

            EndAction();
        }
    }
}
