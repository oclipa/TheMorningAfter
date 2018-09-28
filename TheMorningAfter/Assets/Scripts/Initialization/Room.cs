using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;

public class Room : IRoom {

    protected string name;

    protected string id;

    protected string music;

    protected int itemCount;

    protected Vector3 startPosition = new Vector3(-8.61f, -1.58f, 0);

    protected List<string> roomParameters = new List<string>();

    protected List<string> itemIds = new List<string>();

    protected List<string> collectedItemIds = new List<string>();

    protected Room()
    {
        EventManager.AddItemCollectedListener(ItemCollected);
    }

    public Room(string fileName)
        : this()
    {
        StreamReader streamReader = null;
        try
        {
            readConfig(streamReader, fileName);
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
    }

    public Room(string[] config)
        : this()
    {
        try
        {
            readConfig(config);
        }
        catch (Exception x)
        {
            Debug.Log("Whoops: " + x.Message);
            //SetDefaults(values);
        }
    }

    private void readConfig(StreamReader streamReader, string fileName)
    {
        streamReader = File.OpenText(Path.Combine(Application.streamingAssetsPath, fileName));
        string currentLine = streamReader.ReadLine();
        while (currentLine != null)
        {
            parseLine(currentLine);

            currentLine = streamReader.ReadLine();
        }
    }

    private void readConfig(string[] config)
    {
        for (int i = 0; i < config.Length; i++)
        {
            parseLine(config[i]);
        }
    }

    private void parseLine(string currentLine)
    {
        if (!string.IsNullOrEmpty(currentLine) && !currentLine.StartsWith("//", StringComparison.CurrentCulture))
        {
            string[] tokens = currentLine.Split(',');
            ConfigurationDataValueName valueName =
                (ConfigurationDataValueName)Enum.Parse(
                    typeof(ConfigurationDataValueName), tokens[0].ToUpper());

            switch (valueName)
            {
                case ConfigurationDataValueName.TITLE:
                    this.name = tokens[1];
                    break;
                case ConfigurationDataValueName.ID:
                    this.id = tokens[1];
                    break;
                case ConfigurationDataValueName.MUSIC:
                    this.music = tokens[1];
                    break;
                case ConfigurationDataValueName.ITEMCOUNT:
                    this.itemCount = int.Parse(tokens[1]);
                    break;
                default:
                    roomParameters.Add(currentLine);
                    break;
            }
        }
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <returns>The name.</returns>
    public virtual string GetName()
    {
        return this.name;
    }

    /// <summary>
    /// Gets the unique id for this room.
    /// </summary>
    /// <returns>The id.</returns>
    public virtual string GetID()
    {
        return this.id;
    }


    /// <summary>
    /// Gets the music for this room.
    /// </summary>
    /// <returns>The id.</returns>
    public virtual string GetMusic()
    {
        return this.music;
    }

    /// <summary>
    /// Gets the total number of items in this room.
    /// </summary>
    /// <returns>The item count.</returns>
    public virtual int GetItemCount()
    {
        return this.itemCount;
    }

    /// <summary>
    /// Gets the room details.
    /// </summary>
    /// <returns>The room details.</returns>
    public ReadOnlyCollection<string> GetRoomDetails()
    {
        return new ReadOnlyCollection<string>(this.roomParameters);
    }

    /// <summary>
    /// Does this room contain items?
    /// </summary>
    /// <value><c>true</c> if has items; otherwise, <c>false</c>.</value>
    public bool HasItems
    {
        get { return itemIds.Count > 0; }
    }

    /// <summary>
    /// Add an item to this room
    /// </summary>
    /// <returns><c>true</c>, if item was added, <c>false</c> otherwise.</returns>
    /// <param name="itemID">Item identifier.</param>
    public bool AddItem(string itemID)
    {
        if (!this.collectedItemIds.Contains(itemID))
        {
            this.itemIds.Add(itemID);
            return true;
        }

        return false;
    }

    public void Reset()
    {
        this.collectedItemIds.Clear();
    }

    private void ItemCollected(string itemID)
    {
        this.collectedItemIds.Add(itemID);
        this.itemIds.Remove(itemID);
    }

    /// <summary>
    /// Gets the start position for the player
    /// </summary>
    /// <returns>The start position.</returns>
    public Vector3 GetStartPosition(string enteringFrom)
    {
        foreach (string details in roomParameters)
        {
            string[] bits = details.Split(',');

            if (bits[0].Equals(GameConstants.DOOR))
            {
                string roomName = bits[5];

                if (roomName.Equals(enteringFrom))
                {
                    float x = float.Parse(bits[1]);
                    float y = float.Parse(bits[2]);

                    if (x > 8.73) { x = 8.41f; } // right wall
                    if (x < -8.73) { x = -8.41f; } // left wall
                    if (y > 4.84) { y = 4.36f; } // hole in ceiling
                    if (y < -2.24) // hole in floor
                    {
                        x = x - 0.48f; // don't want to fall down the hole!
                        y = -1.76f; 
                    }
                    return new Vector3(x, y, 0);
                }
            }
        }
        return this.startPosition;
    }
}
