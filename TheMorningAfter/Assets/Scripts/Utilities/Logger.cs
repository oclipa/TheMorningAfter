using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LogType
{
    TO_DEBUG_ONLY,
    TO_UI
}

/// <summary>
/// This was an attempt to implement a logging tool that would output
/// to the screen.  It is not fully implemented or tested.
/// 
/// Obtained from: https://answers.unity.com/questions/508268/i-want-to-create-an-in-game-log-which-will-print-a.html
/// </summary>
public class Logger : MonoBehaviour {

    static bool enableLogging;
    bool showLogger;

    /// <summary>
    /// Log the specified message to the Unity console only
    /// </summary>
    /// <param name="message">Message.</param>
    public static void Log(string message)
    {
        if (enableLogging)
        {
            Logger logger = Camera.main.GetComponent<Logger>();
            if (logger != null)
                logger.AddEvent(message, LogType.TO_DEBUG_ONLY);
        }
    }

    /// <summary>
    /// Log the specified message.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="logType">Log type.</param>
    public static void Log(string message, LogType logType)
    {
        if (enableLogging)
        {
            Logger logger = Camera.main.GetComponent<Logger>();
            if (logger != null)
                logger.AddEvent(message, logType);
        }
    }

    // Private VARS
    private List<string> Eventlog = new List<string>();
    private string displayedText = "";

    // Public VARS
    public int maxLines = 10;

    void OnGUI()
    {
        if (showLogger)
            GUI.Label(new Rect(0, Screen.height - (Screen.height / 3), Screen.width, Screen.height / 3), displayedText, GUI.skin.textArea);
    }

    public void AddEvent(string eventString, LogType logType)
    {
        if (enableLogging)
        {
            Debug.Log(eventString);

            if (logType == LogType.TO_UI)
            {
                Eventlog.Add(eventString);

                int numLines = displayedText.Split('\n').Length;
                while (numLines >= maxLines && Eventlog.Count > 0)
                {
                    Eventlog.RemoveAt(0);
                    numLines = displayedText.Split('\n').Length;
                }

                displayedText = "";

                foreach (string logEvent in Eventlog)
                {
                    displayedText += logEvent;
                    displayedText += "\n";
                }
            }
        }
    }
}
