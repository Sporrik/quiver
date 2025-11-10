using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using TwitchIntegration;
using TMPro;
using Unity.VisualScripting;

public class TwitchGameManager : TwitchMonoBehaviour
{
    [Header("Authorization")]
    [SerializeField] private TMP_InputField channelNameInput;
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private GameObject canvas;
    [Header("UI")]
    [SerializeField] private GameObject UIManager;
    [Header("Refresh time")]
    [SerializeField] private float refreshTime = 15f * 60f; //seconds, for minutes times it by 60


    private Vector3 _targetPosition;
    private readonly System.Action<bool> _authorized = null;

    private UIManager _uiManagerScript;

    private string _username;
    private string _channelName;

    //Vars for chatters per [refreshTime]
    private List<string> _userIDsInChat = new List<string>();
    private List<float> _timeUserIDsInChat = new List<float>();

    private int _viewerCount;

    #region TwitchCommands
    [TwitchCommand("poop_command", "po")]
    public void FillupPoopBar()
    {
        if (_uiManagerScript != null)
            _uiManagerScript.IncrementPoop();
    }
    [TwitchCommand("pee_command", "pe")]
    public void FillupPeeBar()
    {
        if (_uiManagerScript != null)
            _uiManagerScript.IncrementPee();
    }
    [TwitchCommand("hunger_command", "hunger")]
    public void FillupHungerBar()
    {
        if (_uiManagerScript != null)
            _uiManagerScript.IncrementHungry();
    }
    [TwitchCommand("happiness_command", "happy")]
    public void FillUpHappinessBar()
    {
        if (_uiManagerScript != null)
            _uiManagerScript.IncrementHapiness();
    }
    [TwitchCommand("stamina_command", "stamina")]
    public void FillUpStaminaBar()
    {
        if (_uiManagerScript != null)
            _uiManagerScript.IncrementStamina();
    }
    #endregion

    private void Start()
    {
        if (UIManager != null) _uiManagerScript = UIManager.GetComponent<UIManager>();
    }

    //AUTH
    private void Update()
    {
        TwitchManager.OnTwitchClientJoinedChat += TwitchChatConnect;
        TwitchManager.OnTwitchMessageReceived += (user, s) => AddUser(user);

        if (_userIDsInChat.Count > 0)
        {
            for (var idx = 0; idx < _userIDsInChat.Count; idx++)
            {
                _timeUserIDsInChat[idx] -= Time.deltaTime;

                if (_timeUserIDsInChat[idx] < 0)
                {
                    _timeUserIDsInChat.RemoveAt(idx);
                    _userIDsInChat.RemoveAt(idx);
                    if (_viewerCount > 0) _viewerCount--;
                }
            }
        }
    }

    private void TwitchChatConnect()
    {
        canvas.SetActive(false);
    }

    private void AddUser(TwitchUser user)
    {
        for (var idx = 0; idx < _userIDsInChat.Count; idx++)
        {
            if (user.userid == _userIDsInChat[idx])
            {
                _timeUserIDsInChat[idx] = refreshTime;
                return;
            }
        }

        _userIDsInChat.Add(user.userid);
        _timeUserIDsInChat.Add(refreshTime);
        _viewerCount++;
    }

    /// <summary>
    /// Calls authenticate function with given parameters from textboxes in TwitchGameManager
    /// </summary>
    public void Authorize()
    {
        Debug.Log("starting . . .");

        _username = userNameInput.text;
        _channelName = channelNameInput.text;

        TwitchManager.Authenticate(_username, _channelName, _authorized);
        TwitchManager.SendChatMessage("Client Connected . . .");
    }

    /// <summary>
    /// Gets the estimate active viewers per [refreshTime]
    /// </summary>
    public int GetViewerCount()
    {
        return _viewerCount;
    }
}
