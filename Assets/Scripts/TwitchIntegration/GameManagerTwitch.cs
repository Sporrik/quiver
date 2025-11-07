using UnityEngine;
using TwitchIntegration;
using Unity.VisualScripting;
using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine.InputSystem.Controls;


public class GameManagerTwitch : TwitchMonoBehaviour
{

    [SerializeField] private TMP_InputField channelNameInput;
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private GameObject canvas;

    [Header("On screen parameters")]
    [SerializeField] private GameObject messagesPerMinuteGameObject;
    [SerializeField] private GameObject messagesPerSecGameObject;
    [SerializeField] private GameObject totalMessagesGameObject;
    [SerializeField] private GameObject totalUptimeGameObject;

    [Header("On screen bars")]
    [SerializeField] private GameObject poopBar;
    [SerializeField] private GameObject peeBar;
    [SerializeField] private GameObject hungerBar;
    
    [SerializeField] private GameObject amountOfPoopGameObject;
    [SerializeField] private GameObject amountOfPeeGameObject;
    [SerializeField] private GameObject amountOfHungerGameObject;

    private Vector3 _targetPosition;
    private readonly System.Action<bool> _authorized = null;

    private GameObject _authorizationEmpty;
    private TextMeshProUGUI _messagesPerMinuteTextMeshProUGUI;
    private TextMeshProUGUI _messagesPerSecondTextMeshProUGUI;
    private TextMeshProUGUI _totalMessagesMeshProUGUI;
    private TextMeshProUGUI _totalUptimeMeshProUGUI;

    private TextMeshProUGUI _amountOfPoopMeshProUGUI;
    private TextMeshProUGUI _amountOfPeeMeshProUGUI;
    private TextMeshProUGUI _amountOfHungerMeshProUGUI;

    private string _username;
    private string _channelName;
    
    [TwitchCommand("poop_command", "p1")]
    public void FillupPoopBar()
    {
        poopBar.transform.localScale += new Vector3(0.01f, 0, 0);
    }

    [TwitchCommand("pee_command", "p2")]
    public void FillupPeeBar()
    {
        peeBar.transform.localScale += new Vector3(0.05f, 0, 0);
    }

    [TwitchCommand("hunger_command", "h")]
    public void FillupHungerBar()
    {
        hungerBar.transform.localScale += new Vector3(0.025f, 0, 0);
    }

    private void Start()
    {
        if (canvas != null) _authorizationEmpty = canvas.transform.Find("AuthorizationEmpty").gameObject;
        if (messagesPerMinuteGameObject != null) _messagesPerMinuteTextMeshProUGUI = messagesPerMinuteGameObject.GetComponent<TextMeshProUGUI>();
        if (messagesPerSecGameObject != null) _messagesPerSecondTextMeshProUGUI = messagesPerSecGameObject.GetComponent<TextMeshProUGUI>();
        if (totalMessagesGameObject != null) _totalMessagesMeshProUGUI = totalMessagesGameObject.GetComponent<TextMeshProUGUI>();
        if (totalUptimeGameObject != null) _totalUptimeMeshProUGUI = totalUptimeGameObject.GetComponent<TextMeshProUGUI>();
        if (amountOfPoopGameObject != null) _amountOfPoopMeshProUGUI = amountOfPoopGameObject.GetComponent<TextMeshProUGUI>();
        if (amountOfPeeGameObject != null) _amountOfPeeMeshProUGUI = amountOfPeeGameObject.GetComponent<TextMeshProUGUI>();
        if (amountOfHungerGameObject != null) _amountOfHungerMeshProUGUI = amountOfHungerGameObject.GetComponent<TextMeshProUGUI>();

        _amountOfPoopMeshProUGUI.text = "0";
        _amountOfPeeMeshProUGUI.text = "0";
        _amountOfHungerMeshProUGUI.text = "0";

        poopBar.transform.localScale =
            new Vector3(0, poopBar.transform.localScale.y, poopBar.transform.localScale.z);
        peeBar.transform.localScale =
            new Vector3(0, peeBar.transform.localScale.y, peeBar.transform.localScale.z);
        hungerBar.transform.localScale =
            new Vector3(0, hungerBar.transform.localScale.y, hungerBar.transform.localScale.z);
    }

    private int _amountPoop;
    private int _amountPee;
    private int _amountHunger;

    private bool _chatReceived;
    private float _elapsedTime;
    private float _elapsedUptime;
    private float _secondTimer = 0f;
    private float _minuteTimer = 0f;
    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        _elapsedUptime += Time.deltaTime;
        _secondTimer += Time.deltaTime;
        _minuteTimer += Time.deltaTime;

        TwitchManager.OnTwitchClientJoinedChat += TwitchChatConnect;

        if (poopBar.transform.localScale.x > 0.2f)
        {
            poopBar.transform.localScale =
                new Vector3(0, poopBar.transform.localScale.y, poopBar.transform.localScale.z);
            _amountPoop++;
            _amountOfPoopMeshProUGUI.text = _amountPoop.ToString();
        }
        if (peeBar.transform.localScale.x > 0.2f)
        {
            peeBar.transform.localScale =
                new Vector3(0, peeBar.transform.localScale.y, peeBar.transform.localScale.z);
            _amountPee++;
            _amountOfPeeMeshProUGUI.text = _amountPee.ToString();
        }
        if (hungerBar.transform.localScale.x > 0.2f)
        {
            hungerBar.transform.localScale =
                new Vector3(0, hungerBar.transform.localScale.y, hungerBar.transform.localScale.z);
            _amountHunger++;
            _amountOfHungerMeshProUGUI.text = _amountHunger.ToString();
        }

        TwitchManager.OnTwitchMessageReceived += (user, s) => TwitchChatMsgReceived(user, s);
        _totalMessagesMeshProUGUI.text = _msgCount.ToString();
        _totalUptimeMeshProUGUI.text = math.round(_elapsedUptime).ToString();

        if (_elapsedTime >= 0.5f) //reset after a delay
        {
            _prevMsg = null;
            _prevUser.userid = null;
            _elapsedTime = 0;
        }

        if (_secondTimer >= 1f)
        {
            _messagesPerSecondTextMeshProUGUI.text = _msgCountPerSec.ToString();
            _msgCountPerSec = 0;
            _secondTimer -= 1f;
        }

        if (_minuteTimer >= 60f)
        {
            _messagesPerMinuteTextMeshProUGUI.text = _msgCountPerMin.ToString();
            _msgCountPerMin = 0;
            _minuteTimer -= 60f;
        }

    }

    private object ScalarRound(float elapsedUptime, int v)
    {
        throw new NotImplementedException();
    }

    private int _msgCount = 0;
    private float _msgCountPerMin = 0;
    private float _msgCountPerSec = 0;
    private string _prevMsg;
    private TwitchUser _prevUser;
    private void TwitchChatMsgReceived(TwitchUser user, string s)
    {
        if (s != _prevMsg && user.userid != _prevUser.userid)
        {
            _msgCount++;
            _msgCountPerMin++;
            _msgCountPerSec++;

            Debug.Log(s);
            _prevMsg = s;
            _prevUser.userid = user.userid;
        }

    }

    private void TwitchChatConnect()
    {
        _authorizationEmpty.SetActive(false);
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
