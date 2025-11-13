using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject _twitchScreen;
    [SerializeField] private GameObject _controlScreen;
    [SerializeField] private GameObject _PlayScreen;
    [SerializeField] private UIScriptableObject _ScriptableObject;

    public void OnSinglePlayer()
    {
        // set it for singleplayer
        _ScriptableObject.SetSinglePlayer(true);

        SceneManager.LoadScene("LevelOne");

        Debug.Log("Single");
    }

    public void OnTwitch()
    {

        _ScriptableObject.SetSinglePlayer(false);


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

    public void OnPlay()
    {
        _PlayScreen.SetActive(true);
        _twitchScreen.SetActive(false);
    }

    public void OnQuit()
    {
        Application.Quit();
    }


    public void OnBacktoMainMenu()
    {
        _controlScreen.SetActive(false);
        _twitchScreen.SetActive(false);
        _PlayScreen.SetActive(false);

    }
   









}
