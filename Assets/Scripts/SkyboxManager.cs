using Pearl;
using Pearl.ClockManager;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public Camera cam;
    public Vector2 timeRandom;

    private Timer timer = new Timer();

    // Start is called before the first frame update
    void Start()
    {
        cam.backgroundColor = ColorExtend.RandomColor();
        timer.ResetOn(timeRandom);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.IsFinish())
        {
            cam.backgroundColor = ColorExtend.RandomColor();
            timer.ResetOn(timeRandom);
        }
    }
}
