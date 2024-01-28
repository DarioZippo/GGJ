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

        public AudioSource audioSource;
        public AudioClip burpClip;
        public AudioClip fartClip;

        bool isPlayingAudio = false;

        public void Start()
        {
            body = GetComponent<Rigidbody>();
            velocity = velocityInit;
            status = GetComponent<PlayerStatus>();
            audioSource = GetComponent<AudioSource>();
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

        public void Acceleration(float acceleration)
        {
            float aux = acceleration * Time.deltaTime;

            if (status.currentGas > 0)
            {
                velocity = acceleration > 0 ? velocity + aux : Mathf.Max(velocity - aux, velocityInit);

                if(acceleration > 0)
                {
                    audioSource.clip = fartClip;
                    audioSource.loop = true;

                    status.OnUse();
                }
                else if (acceleration < 0)
                {
                    audioSource.clip = burpClip;
                    audioSource.loop = true;

                    status.OnUse();
                }
                else
                {
                    audioSource.loop = false;
                    return;
                }
                audioSource.Play();
            }
        }

    }
}
