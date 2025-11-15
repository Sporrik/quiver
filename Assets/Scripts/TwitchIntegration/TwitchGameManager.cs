using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using TwitchIntegration;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class TwitchGameManager : TwitchMonoBehaviour
{
    [Header("Authorization")]
    [SerializeField] private TMP_InputField channelNameInput;
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private GameObject canvas;

    [Header("UI")]
    [SerializeField] private UIScriptableObject _uiData;

    [Header("Refresh time")]
    [SerializeField] private float refreshTime = 15f * 60f; //seconds, for minutes times it by 60

    [Header("Command Amounts")]
    [SerializeField] private float _incPoop = 5f;
    [SerializeField] private float _incPee = 5f;
    [SerializeField] private float _incHungry = 5f;

    private Vector3 _targetPosition;
    private readonly System.Action<bool> _authorized = null;

    private string _username;
    private string _channelName;

    //Vars for chatters per [refreshTime]
    private List<string> _userIDsInChat = new List<string>();
    private List<float> _timeUserIDsInChat = new List<float>();

    private int _viewerCount;
    private List<int> _viewerCounts = new List<int>();
    private int _highestViewerCount;
    private int _averageViewerCount;
    private float _accTime;

    private List<string> _commands = new List<string>();


    #region TwitchCommands
    [TwitchCommand("poop_command", "po")]
    public void FillupPoopBar()
    {
        _commands.Add("poop_command");
        if (_uiData == null) { Debug.LogWarning("TwitchManager: UIData not assigned."); return; }
        _uiData.IncrementPoop(_incPoop);
    }
    [TwitchCommand("pee_command", "pe")]
    public void FillupPeeBar()
    {
        _commands.Add("pee_command");
        if (_uiData == null) { Debug.LogWarning("TwitchManager: UIData not assigned."); return; }
        _uiData.IncrementPoop(_incPee);
    }
    [TwitchCommand("hunger_command", "hunger")]
    public void FillupHungerBar()
    {
        _commands.Add("hunger_command");
        if (_uiData == null) { Debug.LogWarning("TwitchManager: UIData not assigned."); return; }
        _uiData.IncrementPoop(_incHungry);
    }
    #endregion

    private void OnEnable()
    {
        TwitchManager.OnTwitchClientJoinedChat += TwitchChatConnect;
        TwitchManager.OnTwitchMessageReceived += (user, s) => AddUser(user);
    }

    private void OnDisable()
    {
        TwitchManager.OnTwitchClientJoinedChat -= TwitchChatConnect;
        TwitchManager.OnTwitchMessageReceived -= (user, s) => AddUser(user);
    }
    private void Start()
    {
        LogHelper.Init();
    }

    //AUTH
    private void Update()
    {
        for (int idx = _userIDsInChat.Count - 1; idx >= 0; idx--)
        {
            if (_timeUserIDsInChat[idx] < 0f)
            {
                _timeUserIDsInChat.RemoveAt(idx);
                _userIDsInChat.RemoveAt(idx);
                if (_viewerCount > 0) _viewerCount--;
            }
        }

        if (_viewerCount > _highestViewerCount) _highestViewerCount = _viewerCount;

        _accTime += Time.deltaTime;
        if (_accTime >= refreshTime)
        {
            _viewerCounts.Add(_viewerCount);
            _accTime = 0f;
        }
    }

    private void OnApplicationQuit()
    {
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
        int HappinessUsed = 0;
        int StaminaUsed = 0;

        float avrCommandsPerSec = 0;
        float avrCommandsPerMin = 0;
        foreach (var command in _commands)
        {
            if (command == "poop_command") PoopUsed++;
            if (command == "pee_command") PeeUsed++;
            if (command == "hunger_command") HungerUsed++;
            if (command == "happiness_command") HappinessUsed++;
            if (command == "stamina_command") StaminaUsed++;
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
        LogHelper.Write("happiness_command : " + HappinessUsed);
        LogHelper.Write("stamina_command : " + StaminaUsed);



        //places a divider between each application end (to separate multiple playings per day)
        LogHelper.EndOfApplication();
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

    static class LogHelper
    {
        private static string _logPath;
        public static void Init()
        {
            _logPath = Path.Combine(Application.persistentDataPath, "chatLogs/log_" + $"{DateTime.Now:yyyy-MM-dd}" + ".log");
            try
            {
                // Ensure directory exists
                var dir = Path.GetDirectoryName(_logPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    Debug.Log("test 123");

                }
            }
            catch (Exception e)
            {
                Debug.LogError("Log path setup failed: " + e);
            }
        }
        public static void Write(string message)
        {
            try
            {
                using (var writer = new StreamWriter(_logPath, append: true))
                {
                    writer.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - {message}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to write log: " + e);
            }
        }

        public static void EndOfApplication()
        {
            try
            {
                using (var writer = new StreamWriter(_logPath, append: true))
                {
                    writer.WriteLine(" - - - - - - End of application - - - - - - ");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to write log: " + e);
            }
        }
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
