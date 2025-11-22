using UnityEngine;
using UnityEngine.UI;


public class FlashEffect : MonoBehaviour
{

    public float FlashSpeed; //set this in the editor
    public Image Bar; 
    public RawImage BarOutline;
    public Color red => Color.red;
    public Color white => Color.white;

    void Start()
    {

    }

    void Update()
    {
        Bar.color = LerpRed();
        BarOutline.color = LerpRed();
    }

    public Color LerpRed()
    {
        return Color.Lerp(white, red, Mathf.Sin(Time.time * FlashSpeed) * .5f + .5f);
    }
}
