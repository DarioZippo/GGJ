using Pearl;
using UnityEngine;

public class LightingManager : MonoBehaviour
{

    public Light lighting;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lighting.color = ColorExtend.Complementary(cam.backgroundColor);
    }
}
