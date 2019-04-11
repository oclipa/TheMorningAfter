using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour {

    const string CONFIG_FILES = "*.cfg"; // name format for config files
    const string INDEX_CFG = "Index.cfg"; // the index file for all the room config files

    Text loadingText; // text displayed during loading

    // location of config files is different if WebGl
    bool isWebGl; 
    string assetPath;

    bool allRoomsAdded; // indicates when loading is finished
    bool loadConfigFailed; // if a problem occured
    int expectedRoomCount; // based on contents of index file
    int actualRoomCount; // based on existing config files

    // Use this for initialization
    void Start () 
    {
        Logger.Log("LOADING");
        AudioManager.Instance.Stop(); // stop all music

        loadingText = GameObject.FindGameObjectWithTag(GameConstants.LOADINGTEXT).GetComponent<Text>();

        assetPath = Application.streamingAssetsPath;
        isWebGl = assetPath.Contains("://") || assetPath.Contains(":///");

        GameConstants.TotalTreasures = 0;
        GameConstants.TotalCreatures = 0;

        try
        {
            displayMessage("Opening filing cabinet...");

            if (isWebGl)
            {
                // With WebGL, browser security prevents local files being read, so they 
                // must be requested via an http request.  This means that that the config
                // file access happens asynchronously, which means we need to use Coroutines to
                // avoid hanging up the game.
                StartCoroutine(SendRequest(Path.Combine(assetPath, INDEX_CFG), true));
            }
            else // desktop app
            {
                displayMessage("Reading plans...");

                // read the index file to get the list of available rooms
                List<FileInfo> files = readLocalIndexFile();
                foreach (FileInfo file in files)
                {
                    if (!file.Name.Equals(INDEX_CFG))
                    {
                        IRoom room = new Room(file.Name);
                        addRoom(room);
                    }
                }

                finish();
            }
        } 
        catch // if all else fails, add default rooms only
        {
            displayMessage("Failed to read stored plans");
        }
    }

    private List<FileInfo> readLocalIndexFile()
    {
        List<FileInfo> filteredEntries = new List<FileInfo>();

        StreamReader streamReader = null;
        try
        {
            streamReader = File.OpenText(Path.Combine(assetPath, INDEX_CFG));
            string currentLine = streamReader.ReadLine();
            while (currentLine != null)
            {
                // ignore blank lines and lines beginning with //
                if (!string.IsNullOrEmpty(currentLine) && !currentLine.StartsWith("//", StringComparison.CurrentCulture))
                {
                    FileInfo fileInfo = new FileInfo(Path.Combine(assetPath, currentLine));
                    // check if room config file exists
                    if (fileInfo.Exists)
                        filteredEntries.Add(fileInfo);
                }

                currentLine = streamReader.ReadLine();
            }
        }
        catch (Exception x)
        {
            Debug.Log("Whoops: " + x.Message);
            //SetDefaults(values);
        }
        finally
        {
            if (streamReader != null)
                streamReader.Close();
        }

        return filteredEntries;
    }

    private void addRoom(IRoom room)
    {
        displayMessage("Located " + room.GetName());
        if (Blueprints.AddRoom(room))
        {
            GameConstants.TotalTreasures += room.GetItemCount();
            GameConstants.TotalCreatures += room.GetCreatureCount();
        }
    }

    private void displayMessage(string message)
    {
        //AudioManager.Instance.PlayOneShot(AudioClipName.Click);
        loadingText.text = loadingText.text + Environment.NewLine + message;
    }

    private void finish()
    {
        displayMessage("Building first room...");
        allRoomsAdded = true;
    }

    // Update is called once per frame
    void Update () 
    {
        if (allRoomsAdded)
        {
            //Debug.Log("Open Gameplay Scene");
            SceneManager.LoadScene("Gameplay");
        } 
        else if (isWebGl)
        {
            if (loadConfigFailed || expectedRoomCount < actualRoomCount)
            {
                loadConfigFailed = false;
            }
            else if (expectedRoomCount > 0 && expectedRoomCount == actualRoomCount)
            {
                finish();
            }
        }
	}

    /// <summary>
    /// Get config from web server
    /// </summary>
    /// <returns>The request.</returns>
    /// <param name="url">URL.</param>
    /// <param name="readIndex">If set to <c>true</c> read index.</param>
    private IEnumerator SendRequest(string url, bool readIndex = false)
    {
        if (readIndex)
            displayMessage("Reading plans...");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                displayMessage("Request Error: " + request.error);
                loadConfigFailed = true;
            }
            else
            {
                try
                {
                    // get the file (could be index or room config)
                    string fileContents = request.downloadHandler.text;

                    // iterate over the contents of the file and extract each line
                    string[] entries = fileContents.Split('\n');
                    List<string> filteredEntries = new List<string>();
                    for (int i = 0; i < entries.Length; i++)
                    {
                        string entry = entries[i];
                        //displayMessage(entry);
                        if (!string.IsNullOrEmpty(entry) && !entry.StartsWith("//", StringComparison.CurrentCulture))
                        {
                            //displayMessage(" -- OK");
                            filteredEntries.Add(entry);
                        }
                    }

                    // The first time this is called, we will read the index file
                    if (readIndex)
                    {
                        expectedRoomCount = filteredEntries.Count;
                        displayMessage("House has " + expectedRoomCount + " rooms");

                        // for each room listed in the index file, query the web server for
                        // the correct config file
                        for (int i = 0; i < expectedRoomCount; i++)
                        {
                            //displayMessage("Reading room " + i);
                            StartCoroutine(SendRequest(Path.Combine(assetPath, filteredEntries[i])));
                        }
                    }
                    else // in subsequent calls we read the requested room config
                    {
                        IRoom room = new Room(filteredEntries.ToArray());
                        addRoom(room);
                        actualRoomCount++;
                    }
                }
                catch (Exception x)
                {
                    displayMessage("Load Error: " + x.Message);
                    loadConfigFailed = true;
                }
            }
        }
    }
}
