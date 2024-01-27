using UnityEngine;

namespace Game
{
    public class VehicleLogic : MonoBehaviour
    {
        public float velocityInit = 5.0f;
        public float multiplyHorizontal = 2.0f;

        Rigidbody body;
        private float horizontal;
        private float velocity;

        public void Start()
        {
            body = GetComponent<Rigidbody>();
            velocity = velocityInit;
        }

        public void Update()
        {
            body.velocity = new Vector3(horizontal * multiplyHorizontal, body.velocity.y, velocity);
        }

        public void UpdateControl(float value)
        {
            horizontal = value;
        }

    }
}
