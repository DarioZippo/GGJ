using Pearl;
using UnityEngine;

namespace Game
{
    public class VehicleLogic : MonoBehaviour
    {
        public float velocityInit = 5.0f;
        public float multiplyHorizontal = 2.0f;
        public float acceleration = 0.1f;
        [ReadOnly]
        public float velocity;

        Rigidbody body;
        private float horizontal;
        private PlayerStatus status;

        public void Start()
        {
            body = GetComponent<Rigidbody>();
            velocity = velocityInit;
            status = GetComponent<PlayerStatus>();
        }

        public void Update()
        {
            body.velocity = new Vector3(horizontal * multiplyHorizontal, body.velocity.y, velocity);
        }

        public void UpdateControl(float value)
        {
            horizontal = value;
        }

        public void Acceleration(bool up)
        {
            if (status.currentGas > 0)
            {
                velocity = up ? velocity + acceleration : Mathf.Max(velocity - acceleration, 0);
                status.OnUse();
            }
        }

    }
}
