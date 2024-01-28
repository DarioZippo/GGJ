using Pearl;
using Pearl.Events;
using TMPro;
using UnityEngine;

namespace Game
{
    public class PointerUIManager : MonoBehaviour
    {
        public TMP_Text textUI;
        public Camera cam;

        // Start is called before the first frame update
        void Start()
        {
            textUI.text = "0";
            PearlEventsManager.AddAction<int>("OnPoint", OnUpdatePoint);
            cam = GameObject.FindAnyObjectByType<Camera>();
        }

        private void Update()
        {
            textUI.color = ColorExtend.Complementary(cam.backgroundColor);
        }

        private void OnDestroy()
        {
            PearlEventsManager.RemoveAction<int>("OnPoint", OnUpdatePoint);
        }

        public void OnUpdatePoint(int value)
        {
            textUI.text = value.ToString();
        }
    }
}
