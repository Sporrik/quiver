using UnityEngine;

public class CameraSimpleFollowScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private GameObject _player;
    [SerializeField] private Vector3 _distance;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _player.transform.position + _distance;
    }
}
