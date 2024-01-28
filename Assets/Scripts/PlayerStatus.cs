using Pearl;
using Pearl.Events;
using System;
using UnityEngine;

namespace Game
{
    public class PlayerStatus : MonoBehaviour
    {
        public float maxGas = 10;
        public float gasForBean = 1;
        public float gasForUse = 0.01f;

        [ReadOnly]
        public float currentGas = 0;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void OnBean(GameObject bean)
        {
            GameObject.Destroy(bean);
            currentGas = Math.Min(currentGas + gasForBean, maxGas);
        }

        public void OnUse()
        {
            currentGas = Math.Max(currentGas - gasForUse, 0);
        }

        // Update is called once per frame
        void Update()
        {
            PearlEventsManager.CallEvent("OnGas", currentGas / maxGas);
        }
    }
}
