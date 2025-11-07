using UnityEngine;
using TwitchIntegration;
using Unity.VisualScripting;
using System;
using TMPro;


public class GameManagerTwitch : TwitchMonoBehaviour
{

    [SerializeField] private GameObject testObject;
    [SerializeField] private TMP_InputField channelNameInput;
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private GameObject canvas;

    private Vector3 _targetPosition;
    private readonly System.Action<bool> _authorized = null;

    private string _username;
    private string _channelName;

    [TwitchCommand("move_object", "move", "m")]
    public void MoveObject(float x, float y)
    {
        _targetPosition = new Vector3(x, y, 0);
    }

    [TwitchCommand("test_command", "test", "t")]
    public void TestCommand()
    {
        Debug.Log("Test came through!");
    }

    private void Update()
    {
        testObject.transform.position = Vector3.Lerp(testObject.transform.position, _targetPosition, Time.deltaTime);

        TwitchManager.OnTwitchClientJoinedChat += TwitchChatConnect;

    }

    private void TwitchChatConnect()
    {
        canvas.SetActive(false);
    }

    //if button pressed do authorization
    public void Authorize()
    {
        Debug.Log("starting . . .");

        _username = userNameInput.text;
        _channelName = channelNameInput.text;

        TwitchManager.Authenticate(_username, _channelName, _authorized);
        TwitchManager.SendChatMessage("Client Connected . . .");
    }
}
