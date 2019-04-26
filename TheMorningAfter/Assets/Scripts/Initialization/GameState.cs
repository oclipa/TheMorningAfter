using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameState : MonoBehaviour, IGameOverInvoker
{
    // various strings used for persisting game state
    const string CURRENT_ROOM = "CurrentRoom";
    const string CURRENT_POSITION_X = "CurrentPositionX";
    const string CURRENT_POSITION_Y = "CurrentPositionY";
    const string RESTART_POSITION_X = "RestartPositionX";
    const string RESTART_POSITION_Y = "RestartPositionY";
    const string ROOM_STATE = "RoomState";
    const string ITEMS_COLLECTED = "ItemsCollected";
    const string CREATURES_KILLED = "CreaturesKilled";
    const string LIVES_REMAINING = "LivesRemaining";
    const string IN_GAME_TIME = "InGameTime";
    const string MAID_INSTRUCTIONS = "MaidInstructions";
    const string MAID_MENTIONED_WEAPON = "MaidMentionedWeapon";
    const string MAID_WARNING = "MaidWarning";
    const string MAID_RECEIVED_ITEMS = "MaidReceivedItems";
    const string BED_REACHED = "BedReached";
    const string HELP_SHOWN = "HelpShown";
    const string HAS_THE_WEAPON = "HasTheWeapon";
    const string DISABLE_MUSIC = "DisableMusic";
    const string DISABLE_EFFECTS = "DisableEffects";
    const string DIFFICULTY_MULTIPLIER = "DifficultMultiplier";

    // initialiation happens during Update() so this avoid multiple initializations
    bool isInitialized;

    float difficultyMultiplier = 1.0f;
    string currentRoom;
    Vector3? currentPosition; // of player
    Vector3 restartPosition; // where player should restart in the room if they die
    string roomState; // serialized string detailing items collected, creatures killed, etc.
    int itemsCollected; // total number of items collected
    int creaturesKilled; // total number of creatures killed
    int livesRemaining; // number of lives remaining
    DateTime? inGameTime;
    bool maidGivenInstructions;
    bool maidMentionedWeapon;
    bool maidGivenWarning;
    bool maidReceivedItems;
    bool bedReached;
    bool helpShown;
    bool playerHasTheWeapon;

    RoomBuilder roomBuilder;
    GameObject player;
    TimeController timeController;

    private bool died;
    private Image blankScreen;
    private bool isBlank;
    private Timer deathTimer;

    private bool gameOver;
    private GameOverEvent gameOverEvent;

    private bool disableMusic;
    private bool disableSoundEffects;

    private void OnDestroy()
    {
        //Debug.Log("Destroy gamestate");
        OnDestruction();
    }

    public static Action OnDestruction = delegate { };

    private void Awake()
    {
        //Debug.Log("Awake gamestate");
        this.deathTimer = gameObject.AddComponent<Timer>();
        this.deathTimer.Duration = 0.5f; // time before player respawn after death (seconds)

        Initialize();
    }

    private void Start()
    {
        if (this.livesRemaining == 0)
            this.livesRemaining = GameConstants.MaxLives - 1; // -1 because don't have extra life yet
        Logger.Log("LIVES AT START:" + this.livesRemaining);

        EventManager.AddItemCollectedListener(CollectItem);
        EventManager.AddCreatureKilledListener(KillCreature);
        EventManager.AddPowerUpListener(CollectItem);
        EventManager.AddPlayerDiedListener(LoseLife);
        EventManager.AddNextRoomListener(NextRoom);
        EventManager.AddRoomVisitedListener(RoomVisited);

        // display an initially transparent blank screen when first started 
        // (will be used if player loses a life)
        GameObject background = GameObject.FindGameObjectWithTag(GameConstants.BLANKSCREENBACKGROUND);
        if (background != null)
        {
            this.blankScreen = background.GetComponent<Image>();
            Color color = this.blankScreen.color;
            color.a = 0f; // transparent
            this.blankScreen.color = color;
        }

        this.gameOverEvent = new GameOverEvent();
        EventManager.AddGameOverInvoker(this);

        //Debug.Log("Start gamestate: " + Dump());
    }

    private void Initialize()
    {
        GameObject sb = GameObject.FindGameObjectWithTag(GameConstants.SCOREBOARD);
        if (sb != null)
        {
            roomBuilder = Camera.main.GetComponent<RoomBuilder>();
            player = GameObject.FindGameObjectWithTag(GameConstants.PLAYER);
            timeController = GameObject.FindGameObjectWithTag(GameConstants.TIMETEXT).GetComponent<TimeController>();

            isInitialized = true;
        }
    }

    private void Update()
    {
        // do this here to ensure that everything else is initialized
        // TODO: maybe move this to Start() and all other initialization to Awake()?
        if (!isInitialized)
            Initialize();
    }

    private void FixedUpdate()
    {
        // if player has lost a life, blank the screen for the timer period
        // and then respawn the player.
        if (died)
        {
            if (!isBlank)
            {
                deathTimer.Run();
                Color color = blankScreen.color;
                color.a = 255f;
                blankScreen.color = color;
                isBlank = true;
            }

            if (deathTimer.Finished)
            {
                died = false;
                Color color = blankScreen.color;
                color.a = 0f;
                blankScreen.color = color;
                isBlank = false;
                PlayerSpawner.SpawnNewPlayer(RestartPosition);
            }
        }
    }

    /// <summary>
    /// Adds the games over listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    public void AddGameOverListener(UnityAction<int> listener)
    {
        gameOverEvent.AddListener(listener);
    }

    /// <summary>
    /// Save game state to PlayerPrefs
    /// </summary>
    public void Save()
    {
#if UNITY_WEBGL
        SaveToPlayerPrefs();
#else
        SaveToFile();
#endif

        Logger.Log("SAVED GAME STATE");

        Load();
    }

    private void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetString(CURRENT_ROOM, CurrentRoom);
        Vector3 cPosition = CurrentPosition;
        PlayerPrefs.SetFloat(CURRENT_POSITION_X, cPosition.x);
        PlayerPrefs.SetFloat(CURRENT_POSITION_Y, cPosition.y);
        Vector3 rPosition = RestartPosition;
        PlayerPrefs.SetFloat(RESTART_POSITION_X, rPosition.x);
        PlayerPrefs.SetFloat(RESTART_POSITION_Y, rPosition.y);
        PlayerPrefs.SetString(ROOM_STATE, RoomStateSummary);
        PlayerPrefs.SetInt(ITEMS_COLLECTED, ItemsCollected);
        PlayerPrefs.SetInt(CREATURES_KILLED, CreaturesKilled);
        PlayerPrefs.SetInt(LIVES_REMAINING, LivesRemaining);
        PlayerPrefs.SetString(IN_GAME_TIME, InGameTime.Ticks.ToString());
        PlayerPrefs.SetString(MAID_INSTRUCTIONS, MaidGivenInstructions.ToString());
        PlayerPrefs.SetString(MAID_MENTIONED_WEAPON, MaidMentionedWeapon.ToString());
        PlayerPrefs.SetString(MAID_WARNING, MaidGivenWarning.ToString());
        PlayerPrefs.SetString(MAID_RECEIVED_ITEMS, MaidReceivedItems.ToString());
        PlayerPrefs.SetString(BED_REACHED, BedReached.ToString());
        PlayerPrefs.SetString(HELP_SHOWN, HelpShown.ToString());
        PlayerPrefs.SetString(HAS_THE_WEAPON, PlayerHasTheWeapon.ToString());
        PlayerPrefs.SetString(DISABLE_MUSIC, DisableMusic.ToString());
        PlayerPrefs.SetString(DISABLE_EFFECTS, DisableSoundEffects.ToString());
        PlayerPrefs.SetFloat(DIFFICULTY_MULTIPLIER, difficultyMultiplier);
        //Debug.Log("Save gamestate: " + Dump());
    }

    private void SaveToFile()
    {
        // TODO: implement this if we want to avoid using PlayerPrefs for some reason
        SaveToPlayerPrefs();
    }

    /// <summary>
    /// Load game state from PlayerPrefs
    /// </summary>
    public void Load()
    {
#if UNITY_WEBGL
        LoadFromPlayerPrefs();
#else
        LoadFromFile();
#endif

        Logger.Log("LIVES AFTER LOAD:" + this.livesRemaining);
        Logger.Log("LOADED SAVED GAME STATE: " + Dump());
    }

    private void LoadFromPlayerPrefs()
    {
        currentRoom = PlayerPrefs.GetString(CURRENT_ROOM);
        float currentX = PlayerPrefs.GetFloat(CURRENT_POSITION_X);
        float currentY = PlayerPrefs.GetFloat(CURRENT_POSITION_Y);
        currentPosition = new Vector3(currentX, currentY, 0);
        float restartX = PlayerPrefs.GetFloat(RESTART_POSITION_X);
        float restartY = PlayerPrefs.GetFloat(RESTART_POSITION_Y);
        restartPosition = new Vector3(restartX, restartY, 0);
        roomState = PlayerPrefs.GetString(ROOM_STATE);
        if (Overrides.Enabled)
            itemsCollected = Overrides.ItemsCollected;
        else
            itemsCollected = PlayerPrefs.GetInt(ITEMS_COLLECTED);
        creaturesKilled = PlayerPrefs.GetInt(CREATURES_KILLED);
        livesRemaining = PlayerPrefs.GetInt(LIVES_REMAINING);
        long ticks = 0;
        long.TryParse(PlayerPrefs.GetString(IN_GAME_TIME), out ticks);
        inGameTime = new DateTime(ticks);
        bool.TryParse(PlayerPrefs.GetString(MAID_INSTRUCTIONS), out maidGivenInstructions);
        bool.TryParse(PlayerPrefs.GetString(MAID_MENTIONED_WEAPON), out maidMentionedWeapon);
        bool.TryParse(PlayerPrefs.GetString(MAID_WARNING), out maidGivenWarning);
        bool.TryParse(PlayerPrefs.GetString(MAID_RECEIVED_ITEMS), out maidReceivedItems);
        bool.TryParse(PlayerPrefs.GetString(BED_REACHED), out bedReached);
        bool.TryParse(PlayerPrefs.GetString(HELP_SHOWN), out helpShown);
        bool.TryParse(PlayerPrefs.GetString(HAS_THE_WEAPON), out playerHasTheWeapon);
        bool.TryParse(PlayerPrefs.GetString(DISABLE_MUSIC), out disableMusic);
        bool.TryParse(PlayerPrefs.GetString(DISABLE_EFFECTS), out disableSoundEffects);
        difficultyMultiplier = PlayerPrefs.GetFloat(DIFFICULTY_MULTIPLIER);
        //Debug.Log("Load gamestate: " + Dump());
    }

    private void LoadFromFile()
    {
        // TODO: implement this if we want to avoid using PlayerPrefs for some reason
        LoadFromPlayerPrefs();
    }

    /// <summary>
    /// Dumping the game state forces all properties to be initialized to their current state.
    /// </summary>
    /// <returns>The dump.</returns>
    public string Dump()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(CurrentRoom + "#");
        sb.Append(CurrentPosition.x.ToString() + "#");
        sb.Append(CurrentPosition.y.ToString() + "#");
        sb.Append(RestartPosition.x.ToString() + "#");
        sb.Append(RestartPosition.y.ToString() + "#");
        sb.Append(RoomStateSummary + "#");
        sb.Append(ItemsCollected.ToString() + "#");
        sb.Append(CreaturesKilled.ToString() + "#");
        sb.Append(LivesRemaining.ToString() + "#");
        sb.Append(InGameTime.ToShortTimeString() + "#");
        sb.Append(MaidGivenInstructions.ToString() + "#");
        sb.Append(MaidGivenWarning.ToString() + "#");
        sb.Append(MaidReceivedItems.ToString() + "#");
        sb.Append(BedReached.ToString() + "#");
        sb.Append(PlayerHasTheWeapon.ToString() + "#");
        sb.Append(DisableMusic.ToString() + "#");
        sb.Append(DisableSoundEffects.ToString() + "#");
        sb.Append(DifficultyMultiplier.ToString());
        return sb.ToString();
    }

    /// <summary>
    /// Deletes the saved game
    /// </summary>
    public void DeleteAll()
    {
        // reset properties to default
        currentRoom = GameConstants.DefaultRoom;
        currentPosition = new Vector3(0,0,0);
        restartPosition = new Vector3(0, 0, 0);
        roomState = string.Empty;
        itemsCollected = 0;
        creaturesKilled = 0;
        livesRemaining = 0;
        Logger.Log("LIVES AFTER DELETE:" + this.livesRemaining);
        inGameTime = GameConstants.DEFAULT_TIME;
        maidGivenInstructions = false;
        maidMentionedWeapon = false;
        maidGivenWarning = false;
        maidReceivedItems = false;
        bedReached = false;
        helpShown = false;
        playerHasTheWeapon = false;
        difficultyMultiplier = 1.0f;

        // not sure we need to override these here
        //enableMusic = true;
        //enableSoundEffects = true;

        // overwrite saved properties with defaults
        PlayerPrefs.SetString(CURRENT_ROOM, currentRoom);
        PlayerPrefs.SetFloat(CURRENT_POSITION_X, currentPosition.Value.x);
        PlayerPrefs.SetFloat(CURRENT_POSITION_Y, currentPosition.Value.y);
        PlayerPrefs.SetFloat(RESTART_POSITION_X, restartPosition.x);
        PlayerPrefs.SetFloat(RESTART_POSITION_Y, restartPosition.y);
        PlayerPrefs.SetString(ROOM_STATE, roomState);
        PlayerPrefs.SetInt(ITEMS_COLLECTED, itemsCollected);
        PlayerPrefs.SetInt(CREATURES_KILLED, creaturesKilled);
        PlayerPrefs.SetInt(LIVES_REMAINING, livesRemaining);
        PlayerPrefs.SetString(IN_GAME_TIME, inGameTime.Value.Ticks.ToString());
        PlayerPrefs.SetString(MAID_INSTRUCTIONS, maidGivenInstructions.ToString());
        PlayerPrefs.SetString(MAID_MENTIONED_WEAPON, maidMentionedWeapon.ToString());
        PlayerPrefs.SetString(MAID_WARNING, maidGivenWarning.ToString());
        PlayerPrefs.SetString(MAID_RECEIVED_ITEMS, maidReceivedItems.ToString());
        PlayerPrefs.SetString(BED_REACHED, bedReached.ToString());
        PlayerPrefs.SetString(HELP_SHOWN, helpShown.ToString());
        PlayerPrefs.SetString(HAS_THE_WEAPON, playerHasTheWeapon.ToString());
        PlayerPrefs.SetString(DISABLE_MUSIC, disableMusic.ToString());
        PlayerPrefs.SetString(DISABLE_EFFECTS, disableSoundEffects.ToString());
        PlayerPrefs.SetFloat(DIFFICULTY_MULTIPLIER, difficultyMultiplier);
        Logger.Log("DELETED GAME STATE");

        //Debug.Log("Delete gamestate: " + Dump());

        // load new, default state
        Load();
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:GameState"/> has saved state.
    /// </summary>
    /// <value><c>true</c> if has saved state; otherwise, <c>false</c>.</value>
    public bool HasSavedState
    {
        get
        {
            // if the name of the currentRoom is set
            // this should mean that the current state is saved
            return !string.IsNullOrEmpty(currentRoom) && (!currentRoom.Equals(GameConstants.DefaultRoom) || itemsCollected > 0);
        }
    }


    /// <summary>
    /// The difficulty multiplier
    /// </summary>
    /// <value>difficult multiplier</value>
    public float DifficultyMultiplier
    {
        get
        {
            return this.difficultyMultiplier;
        }
        set
        {
            this.difficultyMultiplier = value;
        }
    }

    /// <summary>
    /// Gets the id of the room in which the player currently is
    /// </summary>
    /// <value>The current room.</value>
    public string CurrentRoom
    {
        get
        {
            if (string.IsNullOrEmpty(currentRoom) && roomBuilder != null && roomBuilder.CurrentRoom != null)
                currentRoom = roomBuilder.CurrentRoom.GetID();

            return currentRoom;
        }
        set
        {
            this.currentRoom = value;
        }
    }

    /// <summary>
    /// Gets the current position of the player in the current room
    /// </summary>
    /// <value>The current position of the player.</value>
    public Vector3 CurrentPosition
    {
        get
        {
            if (!currentPosition.HasValue)
                currentPosition = player.transform.position;

            return currentPosition.Value;
        }
        set
        {
            this.currentPosition = value;
        }
    }

    /// <summary>
    /// Gets the position at which the player will restart if they die
    /// </summary>
    /// <value>The restart position of the player.</value>
    public Vector3 RestartPosition
    {
        get
        {
            return restartPosition;
        }
        set
        {
            this.restartPosition = value;
        }
    }

    /// <summary>
    /// Gets a summary of the state of all the rooms (i.e. which items they still contain)
    /// </summary>
    /// <value>The state of the rooms.</value>
    public string RoomStateSummary
    {
        get
        {
            if (string.IsNullOrEmpty(roomState))
            {
                StringBuilder sb = new StringBuilder();

                // for each room
                foreach (string roomId in Blueprints.GetRoomIDs())
                {
                    IRoom room = Blueprints.GetRoom(roomId);

                    // serialize list of collected items
                    sb.Append(roomId);
                    sb.Append(":");
                    bool isFirst = true;
                    foreach (string item in room.GetCollectedItems())
                    {
                        if (!isFirst)
                            sb.Append(",");
                        isFirst = false;
                        sb.Append(item);
                    }

                    // serialize list of killed creatures
                    sb.Append(":");
                    isFirst = true;
                    foreach (string creature in room.GetKilledCreatures())
                    {
                        if (!isFirst)
                            sb.Append(",");
                        isFirst = false;
                        sb.Append(creature);
                    }

                    // serialize list of killed creatures
                    sb.Append(":");
                    sb.Append(room.Visited);

                    // room separater
                    sb.Append("/");
                }

                roomState = sb.ToString();
            }

            return roomState;
        }
    }

    /// <summary>
    /// Gets an array of all items that have been collected in the specified room.
    /// </summary>
    /// <returns>The items from room.</returns>
    /// <param name="roomName">Room name.</param>
    public string[] GetCollectedItemsInRoom(string roomName)
    {
        string stateOfRooms = RoomStateSummary;

        if (!string.IsNullOrEmpty(stateOfRooms))
        {
            // get state of all rooms
            string[] rooms = stateOfRooms.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // iterate over rooms looking for requested room
            for (int r = 0; r < rooms.Length; r++)
            {
                string[] bits = rooms[r].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                // if room name matches, return the list of items in the room
                if (bits[0].Equals(roomName) && bits.Length > 1)
                {
                    return bits[1].Split(',');
                }
            }
        }

        return new string[0];
    }

    /// <summary>
    /// Gets an array of all creatures that have been killed in the specified room.
    /// </summary>
    /// <returns>The items from room.</returns>
    /// <param name="roomName">Room name.</param>
    public string[] GetKilledCreaturesInRoom(string roomName)
    {
        string stateOfRooms = RoomStateSummary;

        if (!string.IsNullOrEmpty(stateOfRooms))
        {
            // get state of all rooms
            string[] rooms = stateOfRooms.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // iterate over rooms looking for requested room
            for (int r = 0; r < rooms.Length; r++)
            {
                string[] bits = rooms[r].Split(new char[] { ':' });

                // if room name matches, return the list of creatures in the room
                if (bits[0].Equals(roomName) && bits.Length > 2)
                {
                    return bits[2].Split(',');
                }
            }
        }

        return new string[0];
    }

    /// <summary>
    /// Gets a boolean indicating whether the player has already visited the specified room.
    /// </summary>
    /// <returns>true if visited, otherwise false</returns>
    /// <param name="roomName">Room name.</param>
    public bool GetVisitedStatus(string roomName)
    {
        string stateOfRooms = RoomStateSummary;

        if (!string.IsNullOrEmpty(stateOfRooms))
        {
            // get state of all rooms
            string[] rooms = stateOfRooms.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // iterate over rooms looking for requested room
            for (int r = 0; r < rooms.Length; r++)
            {
                string[] bits = rooms[r].Split(new char[] { ':' });

                // if room name matches, return the list of creatures in the room
                if (bits[0].Equals(roomName) && bits.Length > 2)
                {
                    bool visited = false;
                    bool.TryParse(bits[3], out visited);
                    return visited;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Gets the count of items collected.
    /// </summary>
    /// <value>The count of items collected.</value>
    public int ItemsCollected
    {
        get
        {
            return itemsCollected;
        }
    }

    /// <summary>
    /// Gets the count of creatures killed.
    /// </summary>
    /// <value>The count of creatures killed.</value>
    public int CreaturesKilled
    {
        get
        {
            return creaturesKilled;
        }
    }

    /// <summary>
    /// Gets the number of lives remaining
    /// </summary>
    /// <value>The number of lives remaining.</value>
    public int LivesRemaining
    {
        get
        {
            return livesRemaining;
        }
    }

    /// <summary>
    /// Gets the in-game time.
    /// </summary>
    /// <value>The in-game time.</value>
    public DateTime InGameTime
    {
        get
        {
            if (timeController != null && timeController.CurrentTime.CompareTo(inGameTime.Value) > 0)
            {
                Logger.Log("Setting in-game time:" + timeController.CurrentTime.ToShortTimeString());
                inGameTime = timeController.CurrentTime;
            }
            else if (timeController == null)
            {
                Logger.Log("Time controller is null");
            }
            else if (timeController.CurrentTime.CompareTo(inGameTime.Value) != 0)
            {
                Logger.Log("Time controller is " + timeController.CurrentTime.CompareTo(inGameTime.Value));
            }

            if (inGameTime.HasValue)
                return inGameTime.Value;
            else
                return GameConstants.DEFAULT_TIME;
        }
    }

    /// <summary>
    /// Has the maid given her instructions?
    /// </summary>
    /// <value><c>true</c> if maid given instructions; otherwise, <c>false</c>.</value>
    public bool MaidGivenInstructions
    {
        get
        {
            if (Overrides.Enabled)
                return Overrides.MaidGivenInstructions;
            return this.maidGivenInstructions;
        }
        set
        {
            this.maidGivenInstructions = value;
        }
    }

    /// <summary>
    /// Has the maid mentioned the weapon?
    /// </summary>
    /// <value><c>true</c> if weapon mentioned; otherwise, <c>false</c>.</value>
    public bool MaidMentionedWeapon
    {
        get
        {
            if (Overrides.Enabled)
                return Overrides.MaidMentionedWeapon;
            return this.maidMentionedWeapon;
        }
        set
        {
            this.maidMentionedWeapon = value;
        }
    }

    /// <summary>
    /// Has the maid given her warning?
    /// </summary>
    /// <value><c>true</c> if maid given warning; otherwise, <c>false</c>.</value>
    public bool MaidGivenWarning
    {
        get
        {
            return this.maidGivenWarning;
        }
        set
        {
            this.maidGivenWarning = value;
        }
    }

    /// <summary>
    /// Has the maid received all the items?
    /// </summary>
    /// <value><c>true</c> if maid has received items; otherwise, <c>false</c>.</value>
    public bool MaidReceivedItems
    {
        get
        {
            return this.maidReceivedItems;
        }
        set
        {
            this.maidReceivedItems = value;
        }
    }

    /// <summary>
    /// Has the bed been reached?
    /// </summary>
    /// <value><c>true</c> if bed has been reached; otherwise, <c>false</c>.</value>
    public bool BedReached
    {
        get
        {
            return this.bedReached;
        }
        set
        {
            this.bedReached = value;
        }
    }

    /// <summary>
    /// Does the player have the weapon?
    /// </summary>
    /// <value><c>true</c> if player has weapon; otherwise, <c>false</c>.</value>
    public bool PlayerHasTheWeapon
    {
        get
        {
            if (Overrides.Enabled)
                return Overrides.PlayerHasWeapon;
            return this.playerHasTheWeapon;
        }
        set
        {
            this.playerHasTheWeapon = value;
        }
    }

    /// <summary>
    /// Has the help been shown?
    /// </summary>
    /// <value><c>true</c> if help has been shown; otherwise, <c>false</c>.</value>
    public bool HelpShown
    {
        get
        {
            return this.helpShown;
        }
        set
        {
            this.helpShown = value;
        }
    }

    /// <summary>
    /// Is the game over?
    /// </summary>
    /// <value><c>true</c> if game over; otherwise, <c>false</c>.</value>
    public bool GameOver
    {
        get
        {
            return this.gameOver;
        }
    }

    /// <summary>
    /// Is the background music disabled?
    /// </summary>
    /// <value><c>true</c> if disable music; otherwise, <c>false</c>.</value>
    public bool DisableMusic
    {
        get
        {
            return this.disableMusic;
        }
        set
        {
            this.disableMusic = value;
        }
    }

    /// <summary>
    /// Are the sound effects disabled?
    /// </summary>
    /// <value><c>true</c> if disable sound effects; otherwise, <c>false</c>.</value>
    public bool DisableSoundEffects
    {
        get
        {
            return this.disableSoundEffects;
        }
        set
        {
            this.disableSoundEffects = value;
        }
    }

    /// <summary>
    /// Player has collected an item, which invalidates the previous room state
    /// </summary>
    /// <param name="itemID">Item identifier.</param>
    public void CollectItem(string itemID)
    {
        Logger.Log("ITEM COLLECTED: " + itemID);
        this.roomState = string.Empty;
        Logger.Log("UPDATED GAME STATE: " + Dump());
        this.itemsCollected++;

        if (itemID.Equals(GameConstants.EXTRALIFE))
        {
            this.livesRemaining++;
        }
        else if (itemID.Equals(GameConstants.WEAPON))
        {
            AddWeaponToPlayer(true);
        }
    }


    /// <summary>
    /// Player has entered the next room; get ready for a new roomState
    /// </summary>
    public void NextRoom(string nextRoom, string prevRoom, bool playDoorSound)
    {
        IRoom pRoom = Blueprints.GetRoom(prevRoom);
        pRoom.Visited = true;
        IRoom nRoom = Blueprints.GetRoom(nextRoom);
        nRoom.Visited = true;
        this.roomState = string.Empty;
        Logger.Log("UPDATED GAME STATE: " + Dump());
    }

    /// <summary>
    /// Player has visited the specified room; get ready for a new roomState
    /// </summary>
    public void RoomVisited(IRoom room)
    {
        this.roomState = string.Empty;
        Logger.Log("UPDATED GAME STATE: " + Dump());
    }

    /// <summary>
    /// Player has killed a creature, which invalidates the previous room state
    /// </summary>
    /// <param name="creatureID">Item identifier.</param>
    public void KillCreature(string creatureID)
    {
        Logger.Log("CREATURE KILLLED: " + creatureID);
        this.roomState = string.Empty;
        Logger.Log("UPDATED GAME STATE: " + Dump());
        this.creaturesKilled++;
    }

    /// <summary>
    /// Adds the weapon to the player
    /// </summary>
    public void AddWeaponToPlayer(bool showHelp)
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag(GameConstants.PLAYER);

        GameObject theWeapon = UnityEngine.Object.Instantiate(Resources.Load("Objects/TheWeapon")) as GameObject;
        theWeapon.transform.SetParent(player.transform);
        theWeapon.transform.localPosition = new Vector3(0, -0.2f, 0);
        if (showHelp) // should the help image be shown next to the weapon?
            theWeapon.transform.GetChild(1).GetComponent<HelpFader>().ShowHelp();
        this.playerHasTheWeapon = true;
    }

    /// <summary>
    /// Player has lost a life, which invalidates the number of remaining lives
    /// </summary>
    /// <param name="restartPosition">Position at which player should start again.</param>
    public void LoseLife(Vector3 restartPosition)
    {
        this.livesRemaining--;
        this.restartPosition = restartPosition;
        Logger.Log("PLAYER DIED");
        Logger.Log("LIVES LEFT:" + this.livesRemaining);

        if (!this.gameOver) // if game is not over
        {
            died = true; // player has died

            killPlayer(); // do the necessary to kill the player

            if (this.livesRemaining <= 0) // if no lives are left
            {
                // game is over
                this.gameOverEvent.Invoke(this.itemsCollected);
                this.gameOver = true;
            }
        }
    }

    private static void killPlayer()
    {
        AudioManager.Instance.PlayOneShot(AudioClipName.Died);

        GameObject player = GameObject.FindGameObjectWithTag(GameConstants.PLAYER);

        Animator animator = player.GetComponent<Animator>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        // stop player moving (to avoid any unexpected interactions)
        animator.speed = 0;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

        Destroy(player);
    }

    private CommandHandler commandHandler;

    /// <summary>
    /// Gets the command handler for user input
    /// </summary>
    /// <returns>The command handler.</returns>
    public CommandHandler GetCommandHandler()
    {
        if (commandHandler == null)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
                commandHandler = new MobileCommandHandler();
            else if (Application.platform == RuntimePlatform.WebGLPlayer)
                commandHandler = new KeyboardCommandHandler();
        }

        return commandHandler;
    }
}
