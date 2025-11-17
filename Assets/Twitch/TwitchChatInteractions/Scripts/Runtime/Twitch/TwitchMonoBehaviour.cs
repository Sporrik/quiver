using System;
using System.Collections;
using System.IO;
using Unity.VisualScripting.IonicZip;
using UnityEngine;


namespace TwitchIntegration
{
    public abstract class TwitchMonoBehaviour : MonoBehaviour
    {
        protected virtual void Awake() => StartCoroutine(InitializeCoroutine());

        protected virtual void OnDestroy() => TwitchCommandManager.RemoveBehaviour(this);

        private IEnumerator InitializeCoroutine()
        {
            yield return new WaitUntil(() => TwitchManager.IsInitialized);
            yield return null;
            TwitchCommandManager.AddBehaviour(this);
            LogHelper.Init(); //initialise LogHelper (file creation)
        }
    }

    public class LogHelper
    {
        private static string _logPath;
        /// <summary>
        /// Initialises LogHelper, creates directory if not made already and sets the variable _logPath to "log_yyyy-MM-dd.log"
        /// </summary>
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
                    Debug.Log("Directory made");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Log path setup failed: " + e);
            }
        }

        /// <summary>
        /// Writes message to log file with time (HH:mm:ss.fff)
        /// </summary>
        public static void Write(string message)
        {
            try
            {
                using var writer = new StreamWriter(_logPath, append: true);
                writer.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - {message}");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to write log: " + e);
            }
        }

        /// <summary>
        /// Creates line with text "End of application"
        /// </summary>
        public static void EndOfApplication()
        {
            try
            {
                using var writer = new StreamWriter(_logPath, append: true);
                writer.WriteLine(" - - - - - - End of application - - - - - - ");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to write log: " + e);
            }
        }
    }
}