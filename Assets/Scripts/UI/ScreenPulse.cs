using UnityEngine;
using UnityEngine.UI;

public class ScreenPulse : MonoBehaviour
{
    public bool isTurnedOn;
    public float PulseSpeed;

    public Image ScreenOverlay;

    private Color transparentRed = new Color(1, 0, 0, .3f);
    private Color fullyTransparent = new Color(0, 0, 0, 0);
    void Start()
    {

    }


    void Update()
    {
        if (isTurnedOn)
        {
            ScreenOverlay.color = LerpRed();
        }
        else
        {
            ScreenOverlay.color = fullyTransparent;
        }
    }

    public Color LerpRed()
    {
        return Color.Lerp(transparentRed, fullyTransparent, Mathf.Sin(Time.time * PulseSpeed) * .5f + .5f);
    }
}
