using Pearl;
using UnityEngine;

namespace Game
{
    public class VehicleLogic : MonoBehaviour
    {
        public float velocityInit = 15.0f;
        public float velocityUpdate = 0.01f;


        public float acceleration = 1f;
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
            velocityInit += velocityUpdate * Time.deltaTime;
            body.velocity = new Vector3(horizontal * velocity / 2, body.velocity.y, Mathf.Max(velocity, velocityInit));
        }

        public void UpdateControl(float value)
        {
            horizontal = value;
        }

        public void Acceleration(bool up)
        {
            float aux = acceleration * Time.deltaTime;

            if (status.currentGas > 0)
            {
                velocity = up ? velocity + aux : Mathf.Max(velocity - aux, velocityInit);
                status.OnUse();
            }
        }

    }
}
