using UnityEngine;

public class ProtoPlayerCollision : MonoBehaviour
{
    [SerializeField]
    private GameObject _gameOverScreen;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.collider.CompareTag("Guard"))
        {
            Debug.Log("TRIGGERED");
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        if(_gameOverScreen != null)
        {
            _gameOverScreen.SetActive(true);
        }

        Time.timeScale = 0f;
        Debug.Log("Guard caught the player, its so over");
    }
}
