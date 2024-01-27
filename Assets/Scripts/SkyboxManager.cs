using Pearl;
using Pearl.ClockManager;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public Camera cam;
    public Vector2 timeRandom;

    private readonly Timer timer = new Timer();

    // Start is called before the first frame update
    void Start()
    {
        Color color = ColorExtend.NewColorHSV(1, 1, 1);
        color = color.SetHUE(Random.value, ChangeTypeEnum.Setting);
        cam.backgroundColor = color;

        timer.ResetOn(timeRandom);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.IsFinish())
        {
            Color color = ColorExtend.NewColorHSV(1, 1, 1);
            color = color.SetHUE(Random.value, ChangeTypeEnum.Setting);
            cam.backgroundColor = color;

            Debug.Log(color);

            timer.ResetOn(timeRandom);
        }
    }
}
