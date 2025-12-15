using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenScript : MonoBehaviour
{
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject background;


    private void Awake()
    {
        pauseScreen.SetActive(false);
        background.SetActive(false);
    }


    // Update is called once per frame
    private void Update()
    {
        //pause game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameManager.instance.gamePaused)
            {
                GameManager.instance.GamePaused();
            }
            else
            {
                GameManager.instance.GameResume();
            }
        }

        //if game is paused
        if (GameManager.instance.gamePaused)
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
        }
    }

    //Public buttons functions
    /// <summary>
    /// Resumes game, function made for resume button
    /// </summary>
    public void ResumeBtnFunc()
    {
        GameManager.instance.GameResume();
    }
    /// <summary>
    /// Opens options tab, function made for options button
    /// </summary>
    public void OptionsBtnFunc()
    {
        AudioManager.instance.EnableCanvas();
    }

    /// <summary>
    /// Closes options tab, function made for options button
    /// </summary>
    public void CloseOptionsBtnFunc()
    {
    }


    /// <summary>
    /// Quits level and goes back to Main Menu, function made for Main Menu button
    /// </summary>
    public void MainMenuBtnFunc()
    {
        background.SetActive(true);
        GameManager.instance.GameResume();
        SceneManager.LoadScene("MenuScreen");

    }
    /// <summary>
    /// Quits game, function made for Quit Game button
    /// </summary>
    public void QuitGameBtnFunc()
    {
        Application.Quit();
    }
}
