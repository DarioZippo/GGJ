using Pearl;
using Pearl.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GasBarManager : MonoBehaviour
    {
        public Image image;
        public Camera cam;

        // Start is called before the first frame update
        void Start()
        {
            image.fillAmount = 0;
            cam = GameObject.FindAnyObjectByType<Camera>();
            PearlEventsManager.AddAction<float>("OnGas", OnUpdateGas);
        }

        private void OnDestroy()
        {
            PearlEventsManager.RemoveAction<float>("OnGas", OnUpdateGas);
        }

        void Update()
        {
            image.color = ColorExtend.Complementary(cam.backgroundColor);
        }


        public void OnUpdateGas(float value)
        {
            image.fillAmount = value;
        }
    }
}
