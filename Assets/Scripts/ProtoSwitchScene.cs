using UnityEngine;
using UnityEngine.SceneManagement;

public class ProtoSwitchScene : MonoBehaviour
{
    [SerializeField]
    private string _minigameScene = "MinigameScene";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            SwitchScene();
        }
    }

    private void SwitchScene()
    {
        SceneManager.LoadScene(_minigameScene);
    }
}
