using Pearl.Events;
using System;
using UnityEngine;

namespace Game
{
    public class PointerManager : MonoBehaviour
    {
        public Transform player;
        public VehicleLogic veichle;

        private int point = 0;
        private float auxVelocity;
        private Vector3 positionInit;

        // Start is called before the first frame update
        void Start()
        {
            /*
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            veichle = GameObject.FindGameObjectWithTag("Player").GetComponent<VehicleLogic>();
            */
            positionInit = player.position;
        }

        // Update is called once per frame
        void Update()
        {
            auxVelocity += veichle.velocity * 2;
            point = (int)Math.Abs((transform.position - positionInit).z + auxVelocity);
            PearlEventsManager.CallEvent("OnPoint", point);
        }
    }
}
