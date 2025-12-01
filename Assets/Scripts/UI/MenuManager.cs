using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject _twitchScreen;
    [SerializeField] private GameObject _controlScreen;
    [SerializeField] private GameObject _settingsScreen;

    [SerializeField] private GameObject _twitchObject;

    [SerializeField] private UIScriptableObject _ScriptableObject;
    private bool _singlePlayer = true;

    private void Start()
    {

        // syncronise singleplayer button
        if(_ScriptableObject.GetGameModeSinglePlayer() == false)
        {
            _singlePlayer = false;
            _twitchObject.SetActive(!_singlePlayer); // shows when single player is false -> true image
        }

    }


    public void OnSinglePlayer()
    {
        // set it for singleplayer
        _ScriptableObject.SetSinglePlayer(true);

        SceneManager.LoadScene("LevelOne");

        Debug.Log("Single");
    }

    public void OnToggleSwitch()
    {

        _singlePlayer = !_singlePlayer;

        _ScriptableObject.SetSinglePlayer(_singlePlayer);

        _twitchObject.SetActive(!_singlePlayer);


    }

    public void OnPlayWithTwitch()
    {
        SceneManager.LoadScene("LevelOne");

        // check if it is autorized;
        // set it for multiplayer
        print("LOAD GAME");
    }

    public void OnControls()
    {
        _controlScreen.SetActive(true);
    }

    public void OnSettings()
    {
        _settingsScreen.SetActive(true);
    }


    public void OnPlay()
    {
        // _PlayScreen.SetActive(true);
        if (_singlePlayer)
        {
            SceneManager.LoadScene("LevelOne");
            print("LOAD GAME");

        }
        else
        {
            _twitchScreen.SetActive(true);
            print("Open LoginScreen");
        }
    }

    public void OnQuit()
    {
        Application.Quit();
    }


    public void OnBacktoMainMenu()
    {
        _controlScreen.SetActive(false);
        _twitchScreen.SetActive(false);
        _settingsScreen.SetActive(false);


    }
   









}
