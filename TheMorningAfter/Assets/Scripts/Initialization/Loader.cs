using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour {

    const string CONFIG_FILES = "*.cfg";
    const string INDEX_CFG = "Index.cfg";

    Text loadingText;

    bool isWebGl;
    string assetPath;

    bool readIndexFile;
    bool allRoomsAdded;
    bool loadConfigFailed;
    int expectedRoomCount;
    int actualRoomCount;

    // Use this for initialization
    void Start () 
    {
        AudioManager.Stop();
        //AudioManager.Play(AudioClipName.Loading, true, 0.5f);

        loadingText = GameObject.FindGameObjectWithTag(GameConstants.LOADINGTEXT).GetComponent<Text>();

        assetPath = Application.streamingAssetsPath;
        isWebGl = assetPath.Contains("://") || assetPath.Contains(":///");

        try
        {
            displayMessage("Opening filing cabinet...");

            if (isWebGl)
            {

                // use intermediate loading scene?
                StartCoroutine(SendRequest(Path.Combine(assetPath, INDEX_CFG), true));
            }
            else // desktop app
            {
                displayMessage("Reading plans...");

                DirectoryInfo d = new DirectoryInfo(assetPath);
                FileInfo[] files = d.GetFiles(CONFIG_FILES); // Getting room config files
                foreach (FileInfo file in files)
                {
                    if (!file.Name.Equals(INDEX_CFG))
                    {
                        Room room = new Room(file.Name);
                        addRoom(room);
                    }
                }

                finish();
            }
        } 
        catch // if all else fails, add default rooms only
        {
            displayMessage("Failed to read stored plans; using default plans instead...");

            loadDefaults();
        }
    }

    private void addRoom(Room room)
    {
        displayMessage("Located " + room.GetName());
        if (Blueprints.AddRoom(room))
            GameConstants.TotalTreasures += room.GetItemCount();
    }

    private void loadDefaults()
    {
        Blueprints.ClearAll();
        GameConstants.TotalTreasures = 0;

        TheBathroom room1 = new TheBathroom();
        TheBedroom room2 = new TheBedroom();
        TheLanding room3 = new TheLanding();
        TheLoft room4 = new TheLoft();

        addRoom(room1);
        addRoom(room2);
        addRoom(room3);
        addRoom(room4);

        finish();
    }

    private void displayMessage(string message)
    {
        //AudioManager.PlayOneShot(AudioClipName.Click);
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
            SceneManager.LoadScene("Gameplay");
        } 
        else if (isWebGl)
        {
            if (loadConfigFailed || expectedRoomCount < actualRoomCount)
            {
                loadDefaults();
                loadConfigFailed = false;
            }
            else if (expectedRoomCount > 0 && expectedRoomCount == actualRoomCount)
            {
                finish();
            }
        }
	}

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
                    string fileContents = request.downloadHandler.text;
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

                    if (readIndex)
                    {
                        expectedRoomCount = filteredEntries.Count;
                        displayMessage("House has " + expectedRoomCount + " rooms");
                        for (int i = 0; i < expectedRoomCount; i++)
                        {
                            //displayMessage("Reading room " + i);
                            StartCoroutine(SendRequest(Path.Combine(assetPath, filteredEntries[i])));
                        }
                    }
                    else
                    {
                        Room room = new Room(filteredEntries.ToArray());
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
