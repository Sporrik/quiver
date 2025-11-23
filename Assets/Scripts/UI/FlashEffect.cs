using UnityEngine;
using UnityEngine.UI;


public class FlashEffect : MonoBehaviour
{
    public bool isTurnedOn;
    public float FlashSpeed; //set this in the editor
    public Image Bar; 
    public RawImage BarOutline;
    public Color red => new Color(1,.6f,.2f,1);
    public Color white => Color.white;

    void Start()
    {

    }

    void Update()
    {
        if (isTurnedOn)
        {
            Bar.color = LerpRed();
            BarOutline.color = LerpRed();
        }
        else
        {
            Bar.color = Color.white;
            BarOutline.color = Color.white;
        }
    }

    public Color LerpRed()
    {
        return Color.Lerp(white, red, Mathf.Sin(Time.time * FlashSpeed) * .5f + .5f);
    }
}
