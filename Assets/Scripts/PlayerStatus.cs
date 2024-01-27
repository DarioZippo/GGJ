using Pearl.Events;
using System;
using UnityEngine;

namespace Game
{
    public class PlayerStatus : MonoBehaviour
    {
        float currentGas = 0;
        public float maxGas = 10;

        public float gasForBean = 1;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void OnBean(GameObject bean)
        {
            GameObject.Destroy(bean);
            currentGas = Math.Min(currentGas + gasForBean, maxGas);
        }

        // Update is called once per frame
        void Update()
        {
            PearlEventsManager.CallEvent("OnGas", currentGas);
        }
    }
}
