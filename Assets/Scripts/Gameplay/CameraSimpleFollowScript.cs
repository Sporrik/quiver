using UnityEngine;

public class CameraSimpleFollowScript : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Vector3 _distance;

    void Update()
    {
        transform.position = _player.transform.position + _distance;
    }
}
