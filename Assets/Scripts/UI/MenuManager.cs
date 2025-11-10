using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject _twitchScreen;
    [SerializeField] private GameObject _controlScreen;
    static public bool _isSinglePlayer = false;

    public void OnSinglePlayer()
    {
        _isSinglePlayer = true;
        // set it for singleplayer
        SceneManager.LoadScene("LevelOne");

        Debug.Log("Single");
    }

    public void OnTwitch()
    {
        _isSinglePlayer = false;
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
