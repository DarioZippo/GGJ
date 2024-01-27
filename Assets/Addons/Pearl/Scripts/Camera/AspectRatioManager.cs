using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pearl;

namespace Pearl
{
    [RequireComponent(typeof(Camera))]
    public class AspectRatioManager : MonoBehaviour
    {
        #region Inspector 
        [SerializeField]
        private Camera cam = null;

        [SerializeField]
        private bool ignoreInvertedProportion = true;

        [SerializeField]
        private bool useText = false;

        [SerializeField, ConditionalField("!@useText")]
        private float aspectRatio = 1.77777777778f;

        [SerializeField, ConditionalField("@useText")]
        private string aspectRatioText = "16:9";

        [SerializeField, InspectorButton("ChangeRatio")]
        private bool changeAspectRatio;
        #endregion

        #region Property
        public float AspectRatio
        {
            set
            {
                aspectRatio = value;
                ChangeAspectRatio();
            }
        }

        public string AspectRatioText
        {
            set
            {
                aspectRatioText = value;
                ConvertText();
                ChangeAspectRatio();
            }
        }
        #endregion

        #region UnityCallbacks
        // Start is called before the first frame update
        protected void Start()
        {
            ChangeRatio();
        }

        protected void Reset()
        {
            cam = GetComponent<Camera>();
        }
        #endregion

        #region Private methods
        private void ChangeRatio()
        {
            ConvertText();
            ChangeAspectRatio();
        }

        private void ConvertText()
        {
            if (useText)
            {
                if (aspectRatioText == null)
                {
                    return;
                }

                var numbers = aspectRatioText.Split(":");

                if (numbers == null) 
                {
                    return;
                }

                try 
                {
                    var number1 = float.Parse(numbers[0]);
                    aspectRatio = numbers.Length >= 2 ? number1 / float.Parse(numbers[1]) : number1;
                }
                catch 
                {
                    UnityEngine.Debug.Log("The text of aspecctRatio is wrong");
                }
            }
        }

        private void ChangeAspectRatio()
        {
            cam.rect = new Rect(0, 0, 1, 1);

            if (cam != null)
            {
                if (ignoreInvertedProportion && !(  (aspectRatio < 1 && cam.aspect < 1) || (aspectRatio > 1 && cam.aspect > 1) )) 
                {
                    return;
                }

                var variance = aspectRatio / cam.aspect;
                if (variance < 1.0)
                {
                    cam.rect = new Rect((1.0f - variance) / 2.0f, 0, variance, 1.0f);
                }
                else
                {
                    variance = 1.0f / variance;
                    cam.rect = new Rect(0, (1.0f - variance) / 2.0f, 1.0f, variance);
                }
            }
        }
        #endregion
    }
}
