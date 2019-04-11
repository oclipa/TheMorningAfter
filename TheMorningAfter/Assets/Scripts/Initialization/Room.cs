using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using UnityEngine;

public class Room : IRoom {

    protected string name; // user friendly name of room

    protected string id; // unique id of room

    protected string music; // the music for the room

    protected int itemCount; // the number of items in the room
    protected int creatureCount; // the number of creatures in the room

    protected Vector3 startPosition = new Vector3(-8.61f, -1.58f, 0); // the default start position in the room

    protected List<string> roomParameters = new List<string>(); // as read from the room config file

    protected List<string> originalItemIds = new List<string>(); // the list of all items that were originally in the room
    protected List<string> remainingItemIds = new List<string>(); // the list of items that remain in the room
    protected List<string> collectedItemIds = new List<string>(); // the list of items that have been collected from the room

    protected List<string> originalCreatures = new List<string>(); // the list of all creatures that were originally in the room
    protected List<string> remainingCreatures = new List<string>(); // the list of creatures that remain in the room
    protected List<string> killedCreatures = new List<string>(); // the list of creatures that have been killed in the room

    // physics overrides
    protected float creatureSpeedMultiplier = 1.0f;
    protected float playerSpeedMultiplier = 1.0f;
    protected float gravityXMultiplier = 1.0f;
    protected float gravityYMultiplier = 1.0f;
    protected PlayerControlBehavior playerControlBehavior = PlayerControlBehavior.NORMAL;

    #region Construct Room

    protected Room()
    {
        EventManager.AddItemCollectedListener(CollectItem);
        EventManager.AddPowerUpListener(CollectItem);
        EventManager.AddCreatureKilledListener(KillCreature);
    }

    /// <summary>
    /// Construct room from a config file
    /// </summary>
    /// <param name="fileName">File name.</param>
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

    /// <summary>
    /// Construct room from a list of config parameters
    /// </summary>
    /// <param name="config">Config.</param>
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
        // read config from file
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
        // ignore empty lines and ones beginning with //
        if (!string.IsNullOrEmpty(currentLine) && !currentLine.StartsWith("//", StringComparison.CurrentCulture))
        {
            string[] tokens = currentLine.Split(','); // property delimiter
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
                case ConfigurationDataValueName.CREATURECOUNT:
                    this.creatureCount = int.Parse(tokens[1]);
                    break;
                default:
                    roomParameters.Add(currentLine);
                    break;
            }
        }
    }

    #endregion

    #region Initialize Room

    /// <summary>
    /// Resets the state of the room based on a previously saved state
    /// (or the original state if no saved state is provided)
    /// </summary>
    /// <param name="gameState">Game state.</param>
    public void Reset(GameState gameState = null)
    {
        if (gameState != null)
            Logger.Log("RESETTING " + this.name + " BASED ON: " + gameState.Dump());
        else
            Logger.Log("RESETTING " + this.name + " TO DEFAULTS");

        // reset the list of collected items
        this.collectedItemIds.Clear();

        // if we have a saved state
        if (gameState != null && gameState.HasSavedState)
        {
            // get a list of items that have been collected from the room
            string[] items = gameState.GetCollectedItemsInRoom(this.GetID());

            // compare the collected items with the list of original items
            for (int r = 0; r < items.Length; r++)
            {
                string collectedItem = items[r];

                // if listed as being available to be collected
                if (this.remainingItemIds.Contains(collectedItem))
                {
                    //Debug.Log("remove " + collectedItem);
                    // remove item from list of collectable items
                    this.remainingItemIds.Remove(collectedItem);
                }

                // if not listed as having been collected
                if (!this.collectedItemIds.Contains(collectedItem))
                {
                    // add to list of collected items
                    this.collectedItemIds.Add(collectedItem);
                }
            }

            // get a list of creatures that have been killed in this room
            string[] creatures = gameState.GetKilledCreaturesInRoom(this.GetID());

            // compare the killed creatures with the list of original items
            for (int r = 0; r < creatures.Length; r++)
            {
                string killedCreature = creatures[r];

                // if listed as being available to be killed
                if (this.remainingCreatures.Contains(killedCreature))
                {
                    // remove item from list of killable creatures
                    this.remainingCreatures.Remove(killedCreature);
                }

                // if not listed as having been killed
                if (!this.killedCreatures.Contains(killedCreature))
                {
                    // add to list of killed creatures
                    this.killedCreatures.Add(killedCreature);
                }
            }
        }
        else // no saved state
        {
            // revert list of remaining objects to be the same as the list
            // of original objects
            //Debug.Log("create new list of remaining items");
            this.remainingItemIds = new List<string>(this.originalItemIds);

            // revert list of remaining creatures to be the same as the list
            // of original creatures
            this.remainingCreatures = new List<string>(this.originalCreatures);

            // reset list of collected items
            this.collectedItemIds.Clear();

            // reset list of killed creatures
            this.killedCreatures.Clear();
        }
    }

    /// <summary>
    /// Add an item to this room (this means it is available to be collected)
    /// </summary>
    /// <returns><c>true</c>, if item was added, <c>false</c> otherwise.</returns>
    /// <param name="itemID">Item identifier.</param>
    public void AddItem(string itemID)
    {
        // if not already listed as present in the room
        if (!this.remainingItemIds.Contains(itemID))
        {
            // add it to the list of remaining items
            //Debug.Log("Add remaining item: " + itemID);
            this.remainingItemIds.Add(itemID);
        }

        // if it has previously been flagged as collected
        if (this.collectedItemIds.Contains(itemID))
        {
            // remove it from the list of collected items
            this.collectedItemIds.Remove(itemID);
        }

        // also add it to the list of original items in the room
        if (!this.originalItemIds.Contains(itemID))
        {
            this.originalItemIds.Add(itemID);
        }
    }

    /// <summary>
    /// Collects the item (this removes it from the list of remaining items)
    /// </summary>
    /// <param name="itemID">Item identifier.</param>
    public void CollectItem(string itemID)
    {
        // if the item is available for collection
        if (this.remainingItemIds.Contains(itemID))
        {
            // remove it from the list of items available for collection
            //Debug.Log("collect " + itemID);
            this.remainingItemIds.Remove(itemID);

            // if it has not previously been flagged as collected
            if (!this.collectedItemIds.Contains(itemID))
            {
                // add it from the list of collected items
                this.collectedItemIds.Add(itemID);
            }

            // this action has not effect on the list of original items
        }
    }


    /// <summary>
    /// Add a creature to this room (this means it is available to be killed)
    /// </summary>
    /// <returns><c>true</c>, if item was added, <c>false</c> otherwise.</returns>
    /// <param name="creature">Creature identifier.</param>
    public void AddCreature(string creature)
    {
        // if not already listed as present in the room
        if (!this.remainingCreatures.Contains(creature))
        {
            // add it to the list of remaining creatures
            this.remainingCreatures.Add(creature);
        }

        // if it has previously been flagged as killed
        if (this.killedCreatures.Contains(creature))
        {
            // remove it from the list of killed creatures
            this.killedCreatures.Remove(creature);
        }

        // also add it to the list of original creatures in the room
        if (!this.originalCreatures.Contains(creature))
        {
            this.originalCreatures.Add(creature);
        }
    }

    /// <summary>
    /// Kills the creature (this removes it from the list of remaining creatures)
    /// </summary>
    /// <param name="creature">Creature identifier.</param>
    public void KillCreature(string creature)
    {
        // if the creature is available to be killed
        if (this.remainingCreatures.Contains(creature))
        {
            // remove it from the list of creatures available for killing
            this.remainingCreatures.Remove(creature);

            // if it has not previously been flagged as killed
            if (!this.killedCreatures.Contains(creature))
            {
                // add it from the list of killed creatures
                this.killedCreatures.Add(creature);
            }

            // this action has not effect on the list of original creatures
        }
    }

    #endregion

    #region Room Properties

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
    /// Gets the total number of creaturs in this room.
    /// </summary>
    /// <returns>The creature count.</returns>
    public virtual int GetCreatureCount()
    {
        return this.creatureCount;
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
    /// Does this room have any uncollected items?
    /// </summary>
    /// <value><c>true</c> if has items; otherwise, <c>false</c>.</value>
    public bool HasRemainingItems
    {
        get { return remainingItemIds.Count > 0; }
    }

    /// <summary>
    /// Gets the items currently in the room.
    /// </summary>
    /// <returns>The remaining items.</returns>
    public ReadOnlyCollection<string> GetRemainingItems()
    {
        return new ReadOnlyCollection<string>(this.remainingItemIds);
    }


    /// <summary>
    /// Gets the items that have already been collected in the room.
    /// </summary>
    /// <returns>The collected items.</returns>
    public ReadOnlyCollection<string> GetCollectedItems()
    {
        return new ReadOnlyCollection<string>(this.collectedItemIds);
    }

    /// <summary>
    /// Does this room have any unkilled creatures?
    /// </summary>
    /// <value><c>true</c> if has creatures; otherwise, <c>false</c>.</value>
    public bool HasRemainingCreatures
    {
        get { return remainingCreatures.Count > 0; }
    }

    /// <summary>
    /// Gets the creatures currently in the room.
    /// </summary>
    /// <returns>The remaining creatures.</returns>
    public ReadOnlyCollection<string> GetRemainingCreatures()
    {
        return new ReadOnlyCollection<string>(this.remainingCreatures);
    }


    /// <summary>
    /// Gets the creatures that have already been killed in the room.
    /// </summary>
    /// <returns>The collected items.</returns>
    public ReadOnlyCollection<string> GetKilledCreatures()
    {
        return new ReadOnlyCollection<string>(this.killedCreatures);
    }

    /// <summary>
    /// Gets the start position for the player
    /// </summary>
    /// <returns>The start position.</returns>
    public Vector3 GetStartPosition(string enteringFrom)
    {
        // This assumes that each door in a room leads to a different room.
        // If multiple doors lead to the same room, only the first door
        // encountered will be used as a reference.

        // iterate over room parameters looking for door player entered from
        foreach (string details in roomParameters)
        {
            string[] bits = details.Split(',');

            // if room parameters match door/portal player entered from
            if (bits[0].Equals(GameConstants.DOOR) || bits[0].Equals(GameConstants.PORTAL))
            {
                string roomName = bits[6]; // room door leads to

                // is room the same as the one the player entered from?
                if (roomName.Equals(enteringFrom))
                {
                    // get location of door
                    float x = float.Parse(bits[1]);
                    float y = float.Parse(bits[2]);

                    // slightly offset the player from the door 
                    if (x > 8.73) { x = 8.41f; } // right wall
                    if (x < -8.73) { x = -8.41f; } // left wall
                    if (y > 4.84) { y = 4.36f; } // hole in ceiling
                    if (y < -2.24) // hole in floor
                    {
                        //x = x - 0.48f; // don't want to fall down the hole!
                        y = -1.76f;
                    }
                    return new Vector3(x, y, 0);
                }
            }
        }
        return this.startPosition; // if no match was found, use the default position.
    }


    /// <summary>
    /// Gets the creature speed multiplier.
    /// </summary>
    /// <value>The creature speed multiplier.</value>
    public float CreatureSpeedMultiplier 
    { 
        get { return this.creatureSpeedMultiplier; }
        set { this.creatureSpeedMultiplier = value; }
    }

    /// <summary>
    /// Gets the player speed multiplier.
    /// </summary>
    /// <value>The player speed multiplier.</value>
    public virtual float PlayerSpeedMultiplier 
    { 
        get { return this.playerSpeedMultiplier; }
        set { this.playerSpeedMultiplier = value; }
    }

    /// <summary>
    /// Gets the multiplier for the gravity X axis.
    /// </summary>
    /// <value>The gravity X multiplier.</value>
    public float GravityXMultiplier 
    { 
        get { return this.gravityXMultiplier; }
        set { this.gravityXMultiplier = value; }
    }

    /// <summary>
    /// Gets the multiplier for the gravity Y axis.
    /// </summary>
    /// <value>The gravity Y multiplier.</value>
    public float GravityYMultiplier
    {
        get { return this.gravityYMultiplier; }
        set { this.gravityYMultiplier = value; }
    }

    /// <summary>
    /// Gets the player control behaviour.
    /// </summary>
    /// <value>The player control behaviour.</value>
    public PlayerControlBehavior PlayerControlBehavior 
    { 
        get { return this.playerControlBehavior; }
        set { this.playerControlBehavior = value; }
    }

    #endregion
}
