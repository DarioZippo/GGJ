using Pearl.Input;
using UnityEngine;

namespace Game
{
    public class VehicleInput : MonoBehaviour
    {
        public VehicleLogic vehicle;


        // Update is called once per frame
        void Update()
        {
            float move = InputManager.GetAxis("Movement");
            float acceleration = InputManager.GetAxis("Acceleration");

            if (vehicle != null)
            {
                vehicle.UpdateControl(move);

                if (acceleration != 0)
                {
                    vehicle.Acceleration(acceleration > 0);
                }
            }
        }
    }
}