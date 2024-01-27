using Pearl.Input;
using UnityEngine;

namespace Game
{
    public class VehicleMovement : MonoBehaviour
    {
        public VehicleLogic simpleBikeManager;

        // Start is called before the first frame update
        void Start()
        {
            InputManager.PerformedHandle("Acceleration", HandleAcceleration, Pearl.ActionEvent.Add);
            InputManager.PerformedHandle("DeAcceleration", HandleDeAcceleration, Pearl.ActionEvent.Add);
        }

        private void OnDestroy()
        {
            InputManager.PerformedHandle("Acceleration", HandleAcceleration, Pearl.ActionEvent.Remove);
            InputManager.PerformedHandle("DeAcceleration", HandleDeAcceleration, Pearl.ActionEvent.Remove);
        }

        // Update is called once per frame
        void Update()
        {
            float move = InputManager.GetAxis("Movement");
            if (simpleBikeManager != null)
            {
                simpleBikeManager.UpdateControl(move);
            }
        }

        void HandleAcceleration()
        {
            Debug.Log("acceleration");
        }

        void HandleDeAcceleration()
        {
            Debug.Log("DeAcceleration");
        }
    }
}