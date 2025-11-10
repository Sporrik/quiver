using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject _controlScreen;
    public void OnSinglePlayer()
    {
        Debug.Log("Single");
    }

    public void OnTwitch()
    {
        Debug.Log("twitch");

    }

    public void OnControls()
    {
        _controlScreen.SetActive(true);
    }

    public void OnHowToPlay()
    {

    }

    public void OnBacktoMainMenu()
    {
        _controlScreen.SetActive(false);

    }







}
