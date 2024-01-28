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
        public float distanceForGameover = 10f;

        [ReadOnly]
        public float currentGas = 0;

        private float yPositionInit;

        // Start is called before the first frame update
        void Start()
        {
            yPositionInit = transform.position.y;
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

            if (transform.position.y - yPositionInit < -distanceForGameover)
            {
                LevelManager.GameOver();
            }
        }
    }
}
