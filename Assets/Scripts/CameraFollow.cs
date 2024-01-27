using UnityEngine;

namespace Game
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform destination;
        Vector3 delta;

        // Start is called before the first frame update
        void Start()
        {
            delta = destination.position - transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 aux = destination.position - delta;
            transform.position = new Vector3(destination.position.x, aux.y, aux.z);
        }
    }
}
