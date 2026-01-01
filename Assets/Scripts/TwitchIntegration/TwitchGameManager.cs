using System;
using System.Collections.Generic;
using TMPro;
using TwitchIntegration;
using UI;
using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;


public class TwitchGameManager : TwitchMonoBehaviour
{

    [Header("Authorization")]
    [SerializeField] private TMP_InputField channelNameInput;
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private GameObject canvas;

    [Header("UI")]
    [SerializeField] private UIScriptableObject uiData;
    [SerializeField] private TextMeshProUGUI babyBrabbleTextUI;
    [SerializeField] private TextMeshProUGUI chatUserNameTextUI;

    [Header("Refresh time")]
    [SerializeField] private float refreshTime = 15f * 60f; //seconds, for minutes times it by 60

    [Header("Command Amounts")]
    [SerializeField] private float incPoop = 5f;
    [SerializeField] private float incPee = 5f;
    [SerializeField] private float incHungry = 5f;

    //DON'T REMOVE || dummy var for function call
    private readonly System.Action<bool> _authorized = null;

    //input field vars
    private string _username;
    private string _channelName;

    //Vars for chatters per [refreshTime]
    private static List<string> _userIDsInChat = new List<string>();
    private static List<float> _timeUserIDsInChat = new List<float>();

    //Vars for log file creation
    private static int _viewerCount;
    private static List<int> _viewerCounts = new List<int>();
    private static int _highestViewerCount;
    private static int _averageViewerCount;
    private float _accTime;

    private static List<string> _commands = new List<string>();

    //twitch user vars
    private TwitchUser _userPoop = new TwitchUser();
    private TwitchUser _userPee = new TwitchUser();
    private TwitchUser _userHunger = new TwitchUser();
    private TwitchUser _userEmpty; //just an empty for empty assignment

    private float _accTimeUser = 0;
    private const float MaxWaitTimeUser = 0.5f;

    //baby brabble
    private TwitchUser _userBrabble = new TwitchUser();
    private TwitchUser _userBrabblePrev = new TwitchUser();
    private string _stringBrabble = " ";
    private string _stringBrabblePrev = " ";

    private float _accTimeBrabble;
    private Random _randomBrabble = new Random();
    private float _maxWaitTimeBrabble = 0;
    private bool _msgOnScreen = false;

    private float _accTimeBrabbleRemove;
    private const float MaxWaitTimeBrabbleRemove = 10f;


    #region TwitchCommands

    [TwitchCommand("poop_command", "poo", "poO", "pOo", "pOO", "Poo", "PoO", "POo", "POO")]
    public void FillupPoopBar(TwitchUser user)
    {
        _commands.Add("poop_command"); //DON'T REMOVE || used for statistics in log file
        Debug.Log("Command poop proc"); //remove if clutter
        if (uiData == null) { Debug.LogWarning("TwitchManager: UIData not assigned."); return; }
        uiData.IncrementPoop(incPoop); //Increment call inside ui script
        _userPoop = user;
    }

    [TwitchCommand("wee_command", "wee", "weE", "wEe", "wEE", "Wee", "WeE", "WEe", "WEE")]
    public void FillupPeeBar(TwitchUser user)
    {
        _commands.Add("wee_command"); //DON'T REMOVE || used for statistics in log file
        Debug.Log("Command wee proc"); //remove if clutter
        if (uiData == null) { Debug.LogWarning("TwitchManager: UIData not assigned."); return; }
        uiData.IncrementPee(incPee); //Increment call inside ui script
        _userPee = user;
    }
    [TwitchCommand("hunger_command", "hunger", "hungeR", "hungEr", "hungER", "hunGer", "hunGeR", "hunGEr", "hunGER", "huNger", "huNgeR", "huNgEr",
        "huNgER", "huNGer", "huNGeR", "huNGEr", "huNGER", "hUnger", "hUngeR", "hUngEr", "hUngER", "hUnGer", "hUnGeR", "hUnGEr", "hUnGER", "Hunger", "HungeR",
        "HungEr", "HungER", "HunGer", "HunGeR", "HunGEr", "HunGER")]
    public void FillupHungerBar(TwitchUser user)
    {
        _commands.Add("hunger_command"); //DON'T REMOVE || used for statistics in log file
        Debug.Log("Command hunger proc"); //remove if clutter
        if (uiData == null) { Debug.LogWarning("TwitchManager: UIData not assigned."); return; }
        uiData.IncrementHungry(incHungry); //Increment call inside ui script
        _userHunger = user;
    }
    #endregion

    private void Start()
    {
        if (babyBrabbleTextUI != null)
        {
            babyBrabbleTextUI.text = " ";
            chatUserNameTextUI.text = " ";
        }

    }

    private void Update()
    {
        //DON'T REMOVE || Has to check every time if a msg is sent || used for active viewer count
        TwitchManager.OnTwitchMessageReceived += (user, s) =>
        {
            AddUser(user);
            _userBrabble = user;
            _stringBrabble = s;
        };

        //DON'T REMOVE || empties out the twitchUser's for UI 
        _accTimeUser += Time.deltaTime;

        if (_accTimeUser >= MaxWaitTimeUser)
        {
            _accTimeUser = 0;
            _userHunger = _userEmpty;
            _userPee = _userEmpty;
            _userPoop = _userEmpty;
        }

        //DON'T REMOVE || Brabble logic
        if (babyBrabbleTextUI != null)
        {

            _accTimeBrabble += Time.deltaTime;
            if (_msgOnScreen) _accTimeBrabbleRemove += Time.deltaTime;

            if (_accTimeBrabble >= _maxWaitTimeBrabble)
            {
                if (_stringBrabble.Substring(0, 1) != "!")
                {
                    if (_stringBrabble.Contains("PRIVMSG"))
                    {
                        _stringBrabble = ParseRawIrcMessage(_stringBrabble);
                    }

                    if (_userBrabble.userid == _userBrabblePrev.userid)
                    {
                        if (_stringBrabble == _stringBrabblePrev)
                        {
                            _stringBrabble = " ";
                            _userBrabble = _userEmpty;
                        }
                    }

                    babyBrabbleTextUI.text = _stringBrabble;
                    chatUserNameTextUI.text = _userBrabble.displayname;

                    //color player name on screen
                    Random randR = new Random();
                    Random randG = new Random();
                    Random randB = new Random();

                    float r = randR.Next(256);
                    float g = randG.Next(256);
                    float b = randB.Next(256);
                    chatUserNameTextUI.color = new Color(r / 256f, g / 256f, b / 256f);

                    _msgOnScreen = true;

                    _stringBrabblePrev = _stringBrabble;
                    _userBrabblePrev = _userBrabble;
                }

                _accTimeBrabble = 0;
                _maxWaitTimeBrabble = _randomBrabble.Next(10, 15); //random interval between 60sec and 120sec
            }

            if (_accTimeBrabbleRemove >= MaxWaitTimeBrabbleRemove)
            {
                babyBrabbleTextUI.text = " ";
                chatUserNameTextUI.text = " ";
                _accTimeBrabbleRemove = 0;
                _msgOnScreen = false;
            }
        }

        //DON'T REMOVE || calculates current active chatters in chat || uses refresh time as waiting time 
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

        //DON'T REMOVE || sets highest viewer count
        if (_viewerCount > _highestViewerCount) _highestViewerCount = _viewerCount;


        //DON'T REMOVE || used for logging avr viewers in log file
        _accTime += Time.deltaTime;
        if (_accTime >= refreshTime)
        {
            _viewerCounts.Add(_viewerCount);
            _accTime = 0f;
        }

        //MAKESHIFT SOLUTION
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    //DON'T REMOVE || log creation after application.quit
    private void OnApplicationQuit()
    {
        Debug.Log("application close");
        //get average viewer count in whole stream with refreshRate
        float avrViewerCount = 0;
        foreach (var viewerCount in _viewerCounts)
        {
            avrViewerCount += viewerCount;
        }
        avrViewerCount /= _viewerCounts.Count;

        //get average comment usages
        int PoopUsed = 0;
        int PeeUsed = 0;
        int HungerUsed = 0;

        float avrCommandsPerSec = 0;
        float avrCommandsPerMin = 0;
        foreach (var command in _commands)
        {
            if (command == "poop_command") PoopUsed++;
            if (command == "wee_command") PeeUsed++;
            if (command == "hunger_command") HungerUsed++;
            avrCommandsPerSec++;
            avrCommandsPerMin++;
        }

        avrCommandsPerSec /= Time.realtimeSinceStartup;
        avrCommandsPerMin = avrCommandsPerSec * 60;

        //put in variables/data you want to be written to end Log
        LogHelper.Write("avrViewerCount : " + avrViewerCount);
        LogHelper.Write("HighestViewerCount : " + _highestViewerCount);
        LogHelper.Write("Commands-----------");
        LogHelper.Write("Commands per second : " + avrCommandsPerSec);
        LogHelper.Write("Commands per minute : " + avrCommandsPerMin);
        LogHelper.Write("poop_command : " + PoopUsed);
        LogHelper.Write("pee_command : " + PeeUsed);
        LogHelper.Write("hunger_command : " + HungerUsed);

        //places a divider between each application end (to separate multiple playings per day)
        LogHelper.EndOfApplication();
    }

    //DON'T REMOVE || used for active viewer count
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

    //Parses over string and removes unnecessary stuff from the string
    private string ParseRawIrcMessage(string ircLine)
    {
        // Example input: hynatos!hynatos@hynatos.tmi.twitch.tv PRIVMSG #amazonubereats :What a loser
        var exclamationIdx = ircLine.IndexOf('!');
        var spaceAfterUsernameIdx = ircLine.IndexOf(' ', exclamationIdx);
        var colonIdx = ircLine.IndexOf(':', spaceAfterUsernameIdx);

        string user = exclamationIdx > 0 ? ircLine.Substring(0, exclamationIdx) : "Unknown";
        string message = colonIdx > 0 ? ircLine.Substring(colonIdx + 1) : "";

        return $"{message}";
    }

    //=============== DON'T REMOVE || public function calls, read summaries! ===============
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

    /// <summary>
    /// Gets the most recent username of the person who set the most recent poop command
    /// </summary>
    public string GetUserNamePoopCommand()
    {
       // Debug.Log(_userPoop.displayname);
        return _userPoop.displayname;

    }

    /// <summary>
    /// Gets the most recent username of the person who set the most recent pee command
    /// </summary>
    public string GetUserNamePeeCommand()
    {
        return _userPee.displayname;
    }

    /// <summary>
    /// Gets the most recent username of the person who set the most recent hunger command
    /// </summary>
    public string GetUserNameHungerCommand()
    {
        return _userHunger.displayname;
    }

    //2 getter functions 1 voor _stringBrabble en 1 voor _userBrabble (to know if you need to show babble UI image;

    /// <summary>
    /// Gets the most recent baby brabble message
    /// </summary>
    /// <returns></returns>
    public string GetBabyBrabbleMessage()
    {

        if (_stringBrabble == null || _stringBrabble == " ")
        {
            return null;
        }
        //Debug.Log(_stringBrabble);
        return _stringBrabble;

    }
    public bool GetBabyBrabbleMsgOnScreen()
    {
        //Debug.Log(_msgOnScreen);
        return _msgOnScreen;
    }
    public TwitchUser GetBabyBrabbleUser()
    {

        return _userBrabble;
    }


}



