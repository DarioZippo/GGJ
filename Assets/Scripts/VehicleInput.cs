using Pearl.Input;
using UnityEngine;

namespace Game
{
    public class VehicleInput : MonoBehaviour
    {
        public VehicleLogic vehicle;

        private bool acc;
        private bool deAcc;


        private void Start()
        {
            InputManager.PerformedHandle("Acceleration", AccelerationDown, AccelerationUp, Pearl.ActionEvent.Add);
            InputManager.PerformedHandle("DeAcceleration", DeaccelerationDown, DeaccelerationUp, Pearl.ActionEvent.Add);
        }

        private void OnDestroy()
        {
            InputManager.PerformedHandle("Acceleration", AccelerationDown, AccelerationUp, Pearl.ActionEvent.Remove);
            InputManager.PerformedHandle("DeAcceleration", DeaccelerationDown, DeaccelerationUp, Pearl.ActionEvent.Remove);
        }

        // Update is called once per frame
        void Update()
        {
            float move = InputManager.GetAxis("Movement");

            if (vehicle != null)
            {
                vehicle.UpdateControl(move);

                if (acc)
                {
                    vehicle.Acceleration(1);
                }
                else if (deAcc)
                {
                    vehicle.Acceleration(-1);
                }
                else
                {
                    vehicle.Acceleration(0);
                }

            }
        }

        private void AccelerationDown()
        {
            acc = true;
        }

        private void AccelerationUp()
        {
            acc = false;
        }

        private void DeaccelerationDown()
        {
            deAcc = true;
        }

        private void DeaccelerationUp()
        {
            deAcc = false;
        }
    }
}