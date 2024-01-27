using Pearl.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GasBarManager : MonoBehaviour
    {
        public Image image;

        // Start is called before the first frame update
        void Start()
        {
            image.fillAmount = 0;
            PearlEventsManager.AddAction<float>("OnGas", OnUpdateGas);
        }

        private void OnDestroy()
        {
            PearlEventsManager.RemoveAction<float>("OnGas", OnUpdateGas);
        }

        public void OnUpdateGas(float value)
        {
            image.fillAmount = value;
        }
    }
}
