using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject _twitchScreen;
    [SerializeField] private GameObject _controlScreen;


    public void OnSinglePlayer()
    {

        // set it for singleplayer
        SceneManager.LoadScene("LevelOne");

        Debug.Log("Single");
    }

    public void OnTwitch()
    {
        _twitchScreen.SetActive(true);

    }

    public void OnPlayWithTwitch()
    {
        // check if it is autorized;
        // set it for multiplayer
        SceneManager.LoadScene("LevelOne");
        print("LOAD GAME");
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
        _twitchScreen.SetActive(false);

    }







}
