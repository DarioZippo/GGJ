using UnityEngine;
using UnityWebGLOOrientationDetection;

namespace Pearl.Mobile
{
    public class MobileOrientation : MonoBehaviour
    {
        [SerializeField]
        private ScreenOrientation screenOrientation = ScreenOrientation.AutoRotation;
        [SerializeField]
        [ConditionalField("@screenOrientation == AutoRotation")]
        private bool autorotateToLandscapeLeft = false;
        [SerializeField]
        [ConditionalField("@screenOrientation == AutoRotation")]
        private bool autorotateToLandscapeRight = false;
        [SerializeField]
        [ConditionalField("@screenOrientation == AutoRotation")]
        private bool autorotateToPortrait = false;
        [SerializeField]
        [ConditionalField("@screenOrientation == AutoRotation")]
        private bool autorotateToPortraitUpsideDown = false;

        // Start is called before the first frame update
        private void Start()
        {
            if (GameManager.IsWebGL() && GameManager.IsMobile())
            {
                MobileOrientationDetector.Init();
                MobileOrientationDetector.OnOrientationChange += OnOrientationChange;
                MobileOrientationDetector.ScreenLock();
            }
            else if (GameManager.IsMobile())
            {
                Screen.orientation = screenOrientation;
                Screen.autorotateToLandscapeLeft = autorotateToLandscapeLeft;
                Screen.autorotateToLandscapeRight = autorotateToLandscapeRight;
                Screen.autorotateToPortrait = autorotateToPortrait;
                Screen.autorotateToPortraitUpsideDown = autorotateToPortraitUpsideDown;
            }
        }

        private void OnDestroy()
        {
            MobileOrientationDetector.OnOrientationChange -= OnOrientationChange;
        }

        public void OnOrientationChange(int angle)
        {
            MobileOrientationDetector.ScreenLock();
        }
    }
}
