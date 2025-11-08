using UnityEngine;
using TwitchIntegration;
using Unity.VisualScripting;
using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine.InputSystem.Controls;


public class TwitchGameManager : TwitchMonoBehaviour
{
    [Header("Authorization")]
    [SerializeField] private TMP_InputField channelNameInput;
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private GameObject canvas;
    [Header("UI")]
    [SerializeField] private GameObject UIManager;

    private Vector3 _targetPosition;
    private readonly System.Action<bool> _authorized = null;

    private UIManager _uiManagerScript;

    private string _username;
    private string _channelName;

    [TwitchCommand("poop_command", "po")]
    public void FillupPoopBar()
    {
        _uiManagerScript.IncrementPoop();
    }

    [TwitchCommand("pee_command", "pe")]
    public void FillupPeeBar()
    {
        _uiManagerScript.IncrementPee();
    }

    [TwitchCommand("hunger_command", "hunger")]
    public void FillupHungerBar()
    {
        _uiManagerScript.IncrementHungry();
    }
    [TwitchCommand("happiness_command", "happy")]
    public void FillUpHappinessBar()
    {
        _uiManagerScript.IncrementHapiness();
    }
    [TwitchCommand("stamina_command", "stamina")]
    public void FillUpStaminaBar()
    {
        _uiManagerScript.IncrementStamina();
    }

    private void Start()
    {
        if (UIManager != null) _uiManagerScript = UIManager.GetComponent<UIManager>();
    }

    //AUTH
    private void Update()
    {
        TwitchManager.OnTwitchClientJoinedChat += TwitchChatConnect;
    }
    private string _prevMsg;
    private TwitchUser _prevUser;

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
