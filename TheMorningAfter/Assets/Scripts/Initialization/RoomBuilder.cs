using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Builds the current room
/// </summary>
public class RoomBuilder : MonoBehaviour, IGameOverInvoker {

    private static string START_ROOM = GameConstants.DefaultRoom;
    private static Vector3 START_POSITION = new Vector3(1.71f, -0.21f, 0);

    private IRoom currentRoom;
    private string prevMusic;

    GameOverEvent gameOverEvent;
    bool isGameOver;

    GameState gameState;

    bool constructNewRoom;
    Vector3 nextStartPosition;

    // Use this for initialization
    void Start ()
    {
        //Debug.Log("RoomBuilder Start");
        gameState = Camera.main.GetComponent<GameState>();
        gameState.Load();

        Logger.Log("NEW GAME: " + gameState.Dump());

        if (Overrides.Enabled)
        {
            START_ROOM = Overrides.InitialRoom;
            START_POSITION = Overrides.StartPosition;
        }

        Blueprints.ResetRooms(gameState);

        if (!gameState.CurrentRoom.Equals(GameConstants.GAMEOVER))
        {
            Vector3 startPosition = START_POSITION;
            if (gameState.HasSavedState)
            {
                currentRoom = Blueprints.GetRoom(gameState.CurrentRoom);
                startPosition = gameState.CurrentPosition;
            }
            else
            {
                currentRoom = Blueprints.GetRoom(START_ROOM);
            }

            // build the first room
            BuildRoom(currentRoom, startPosition);

            // initialize the screen boundaries
            ScreenUtils.Initialize();

            // start room music
            //PlayMusic();

            // Listen for when we need to build a new room
            EventManager.AddNextRoomListener(buildNextRoom);
        }
        else
        {
            isGameOver = true;
        }

        this.gameOverEvent = new GameOverEvent();
        EventManager.AddGameOverInvoker(this);
    }

    private void Update()
    {
        if (isGameOver)
        {
            isGameOver = false;
            this.gameOverEvent.Invoke(GameConstants.TotalTreasures);
        }
        else if (constructNewRoom)
        {
            constructNewRoom = false;

            // now create the scoreboard
            ConstructScoreBoard();

            // now rebuild room from scratch
            ConstructRoomContents();

            // spawn the player at the correct state position
            PlayerSpawner.SpawnNewPlayer(this.nextStartPosition);

            // start the background music
            PlayMusic();
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

    public IRoom CurrentRoom
    {
        get { return this.currentRoom; }
    }

    #region Initialize Room

    /// <summary>
    /// If user has left previous room; need to load the next room
    /// </summary>
    private void buildNextRoom(string nextRoom, string prevRoom, bool playDoorSound)
    {
        IRoom room = Blueprints.GetRoom(nextRoom);

        if (playDoorSound)
        {
            //Debug.Log(prevRoom + " > " + nextRoom);
            if ((prevRoom.Equals("TheFilingCabinet") && nextRoom.Equals("TheResultOfTheQuirkafleeg")) || 
                    (prevRoom.Equals("TheResultOfTheQuirkafleeg") && nextRoom.Equals("TheFilingCabinet")))
            {
                //Debug.Log("play portal sound");
                AudioManager.Instance.PlayOneShot(AudioClipName.Portal);
            }
            else
            {
                //Debug.Log("play door sound");
                AudioManager.Instance.PlayOneShot(AudioClipName.Door);
            }
        }

        // if a room is not found, return to the prevRoom
        if (room == null)
        {
            // get previous room
            room = Blueprints.GetRoom(prevRoom);
            // so what was the next room becomes the previous room
            prevRoom = nextRoom;
            // get id of the room we are returning to
            nextRoom = room.GetID();
        }

        if (room != null)
        {
            Vector3 startPosition = room.GetStartPosition(prevRoom);

            Logger.Log("ENTERING " + nextRoom + " FROM " + prevRoom);

            gameState.CurrentRoom = room.GetID();
            gameState.CurrentPosition = startPosition;
            Logger.Log("SAVING STATE: " + gameState.Dump(), LogType.TO_DEBUG_ONLY);
            gameState.Save();
            Logger.Log("SAVED STATE: " + gameState.Dump(), LogType.TO_DEBUG_ONLY);

            BuildRoom(room, startPosition);
        }
        else
            SceneManager.LoadScene(GameConstants.MAINMENU);
    }

    /// <summary>
    /// Builds the indicated room.
    /// </summary>
    /// <param name="room">Room.</param>
    public void BuildRoom(IRoom room, Vector3 startPosition)
    {
        this.currentRoom = room;
        this.nextStartPosition = startPosition;

        // first destroy all existing objects in the room
        DestroyRoomContents();

        this.constructNewRoom = true;
    }

    private static void DestroyRoomContents()
    {
        foreach (GameObject o in UnityEngine.Object.FindObjectsOfType<GameObject>())
        {
            if (!o.CompareTag("MainCamera") &&
                    !o.CompareTag("MainCameraEventSystem") &&
                    !o.CompareTag("GameAudioSource") &&
                    !o.CompareTag("ScoreBoard") &&
                    !o.CompareTag("RoomTitleText") &&
                    !o.CompareTag("ScoreBoardPanel") &&
                    !o.CompareTag("BlankScreenCanvas") &&
                    !o.CompareTag("DebugText") &&
                    !o.CompareTag("BlankScreenBackground") &&
                    !o.CompareTag("LivesSprite") &&
                    !o.CompareTag("TimeText") &&
                    !o.CompareTag("MissingRoomMessage") &&
                    !o.CompareTag("MissingRoomMessageButton") &&
                    !o.CompareTag("LivesRemainingText") &&
                    !o.CompareTag("MessageText") &&
                    !o.CompareTag("ItemsCollectedText"))
                Destroy(o);
        }

        // reset gravity to normal
        Physics2D.gravity = new Vector2(0, -9.81f);
    }

    private void ConstructScoreBoard()
    {
        GameObject scoreBoardCanvas = GameObject.FindGameObjectWithTag(GameConstants.SCOREBOARD);
        if (scoreBoardCanvas == null)
            scoreBoardCanvas = UnityEngine.Object.Instantiate(Resources.Load("ScoreBoard/" + GameConstants.ROOMTITLECANVAS)) as GameObject;
        GameObject.FindGameObjectWithTag(GameConstants.ROOMTITLETEXT).GetComponent<Text>().text = this.currentRoom.GetName();

        // if there is a saved state for the room
        // update the room to match this
        //if (gameState != null && gameState.HasSavedState)
        //{
        //    // update the scoreboard to reflect the current state
        //    ScoreBoard scoreBoard = scoreBoardCanvas.GetComponent<ScoreBoard>();
        //    scoreBoard.Restore(gameState);
        //}
    }

    private void ConstructRoomContents()
    {
        string roomName = this.currentRoom.GetName();
        string roomID = this.currentRoom.GetID();
        ReadOnlyCollection<string> roomParameters = this.currentRoom.GetRoomDetails();

        Logger.Log("ConstructRoomContents: " + roomName + " / " + roomID, LogType.TO_DEBUG_ONLY);
        StringBuilder sb = new StringBuilder();

        foreach (string entry in roomParameters)
        {
            sb.AppendLine(entry);
            string[] bits = entry.Split(',');

            if (bits[0].Equals(GameConstants.PLATFORM))
            {
                createPlatform(bits);
            }
            else if (bits[0].Equals(GameConstants.WALL))
            {
                createWall(bits);
            }
            else if (bits[0].Equals(GameConstants.MOVING_OBSTACLE))
            {
                createMovingObstacle(bits);
            }
            else if (bits[0].Equals(GameConstants.STATIC_OBSTACLE))
            {
                createStaticObstacle(bits);
            }
            else if (bits[0].Equals(GameConstants.SCENERY_OBSTACLE))
            {
                createSceneryObstacle(bits);
            }
            else if (bits[0].Equals(GameConstants.LADDER))
            {
                createLadder(bits);
            }
            else if (bits[0].Equals(GameConstants.AERIAL))
            {
                createAerial(bits);
            }
            else if (bits[0].Equals(GameConstants.CRATE_SHAPE))
            {
                createCrateShape(bits);
            }
            else if (bits[0].Equals(GameConstants.RAMP))
            {
                createRamp(bits);
            }
            else if (bits[0].Equals(GameConstants.ROPE_ANCHOR))
            {
                createRope(bits);
            }
            else if (bits[0].Equals(GameConstants.ITEM))
            {
                createItem(bits);
            }
            else if (bits[0].Equals(GameConstants.POWERUP))
            {
                createPowerUp(bits);
            }
            else if (bits[0].Equals(GameConstants.KEY))
            {
                createKey(bits);
            }
            else if (bits[0].Equals(GameConstants.DOOR))
            {
                createDoor(bits, roomID);
            }
            else if (bits[0].Equals(GameConstants.PORTAL))
            {
                createPortal(bits, roomID);
            }
            else if (bits[0].Equals(GameConstants.MAID))
            {
                createMaid(bits);
            }
            else if (bits[0].Equals(GameConstants.PHYSICS))
            {
                setupRoomPhysics(bits);
            }
        }

        Logger.Log(sb.ToString());

        // if there is a saved state for the room
        // update the room to match this
        if (gameState != null && gameState.HasSavedState)
        {
            //Logger.Log("Current room = " + this.currentRoom.GetName());
            Logger.Log("Restore " + roomID + " using gameState: " + gameState.Dump());

            // reset the lists of collectable and collected items in the room
            this.currentRoom.Reset(gameState);

            // get a list of all items in the room 
            // (note that this can include destroyed items, since these don't
            // actually get destroyed until the next update)
            GameObject[] originalItems = GameObject.FindGameObjectsWithTag(GameConstants.ITEM);
            Logger.Log("Existing items: " + originalItems.Length);

            GameObject[] originalPowerUps = GameObject.FindGameObjectsWithTag(GameConstants.POWERUP);
            Logger.Log("Existing power-ups: " + originalPowerUps.Length);

            // get a list of all creatures in the room 
            // (note that this can include destroyed creatures, since these don't
            // actually get destroyed until the next update)
            GameObject[] originalCreatures = GameObject.FindGameObjectsWithTag(GameConstants.OBSTACLE);
            Logger.Log("Existing creatures: " + originalCreatures.Length);

            StringBuilder sb2 = new StringBuilder();
            foreach (GameObject g in originalItems)
                sb2.Append(g.name + ",");
            foreach (GameObject g in originalPowerUps)
                sb2.Append(g.name + ",");
            foreach (GameObject g in originalCreatures)
                sb2.Append(g.name + ",");
            Logger.Log(sb2.ToString());

            // get a list of items that have not yet been collected
            ReadOnlyCollection<string> remainingItems = this.currentRoom.GetRemainingItems();
            int uncollectedItemCount = remainingItems.Count;
            Logger.Log("Uncollected items: " + uncollectedItemCount);

            StringBuilder sb3 = new StringBuilder();
            foreach (string g in remainingItems)
                sb3.Append(g + ",");
            Logger.Log(sb3.ToString());

            // get a list of creatures that have not yet been killed
            ReadOnlyCollection<string> remainingCreatures = this.currentRoom.GetRemainingCreatures();
            int unkilledCreatureCount = remainingCreatures.Count;
            Logger.Log("Unkilled creatures: " + unkilledCreatureCount);

            StringBuilder sb4 = new StringBuilder();
            foreach (string g in remainingCreatures)
                sb4.Append(g + ",");
            Logger.Log(sb4.ToString());

            // iterate over original items
            for (int i = 0; i < originalItems.Length; i++)
            {
                GameObject item = originalItems[i];

                // if the original item is not listed in the list of uncollected items
                if (item != null && !remainingItems.Contains(item.GetComponent<ItemController>().ID))
                {
                    //Logger.Log("Destroy " + item.name);
                    // remove (destroy) the item from the UI
                    Destroy(item);
                }
            }

            // iterate over power-ups
            for (int i = 0; i < originalPowerUps.Length; i++)
            {
                GameObject item = originalPowerUps[i];

                // if the original item is not listed in the list of uncollected items
                if (item != null && !remainingItems.Contains(item.GetComponent<ItemController>().ID))
                {
                    //Logger.Log("Destroy " + item.name);
                    // remove (destroy) the item from the UI
                    Destroy(item);
                }
            }

            // iterate over original creatures
            for (int i = 0; i < originalCreatures.Length; i++)
            {
                GameObject creature = originalCreatures[i];

                // if the original item is not listed in the list of uncollected items
                if (creature != null && !remainingCreatures.Contains(creature.GetComponent<ObstacleController>().ID))
                {
                    //Logger.Log("Destroy " + item.name);
                    // remove (destroy) the item from the UI
                    Destroy(creature);
                }
            }

            // If we are in the bedroom and the player has already delivered
            // all the items in the saved game, we should not show the maid
            if (gameState.MaidReceivedItems)
            {
                GameObject maid = GameObject.FindGameObjectWithTag(GameConstants.MAID);
                if (maid != null)
                    Destroy(maid);
            }

            //Logger.Log("Current room = " + this.currentRoom.GetName());
            Logger.Log("After Restore GameState: " + gameState.Dump());
        }
    }

    private void PlayMusic()
    {
        string newMusic = currentRoom.GetMusic();

        if (!newMusic.Equals(prevMusic))
        {
            // stop any existing music
            AudioManager.Instance.Stop();

            // and start with the new music
            if (string.IsNullOrEmpty(newMusic))
            {
                AudioManager.Instance.Play(AudioClipName.GameMusic);
            }
            else
            {
                AudioManager.Instance.Play(newMusic);
            }
        }

        prevMusic = newMusic;
    }

    #endregion

    #region Create GameObjects

    private GameObject instantiateObject(string objectType)
    {
        return UnityEngine.Object.Instantiate(Resources.Load("Objects/" + objectType)) as GameObject;
    }

    private void createPlatform(string[] bits)
    {
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        float width = float.Parse(bits[3]);
        float depth = float.Parse(bits[4]);
        string brickType = bits[5];

        Vector3 position = new Vector3(positionX, positionY, 0);
        Vector2 size = new Vector2(width, depth);

        GameObject platform = instantiateObject(GameConstants.PLATFORM);
        platform.transform.position = position;

        string folder = "Sprites/FloorBricks/";
        if (brickType.StartsWith("Weird"))
            folder = "Sprites/WeirdBricks/";

        SpriteRenderer spriteRenderer = platform.GetComponent<SpriteRenderer>();
        spriteRenderer.size = size;
        spriteRenderer.sprite = Resources.Load<Sprite>(folder + brickType);

        BoxCollider2D boxCollider = platform.GetComponent<BoxCollider2D>();
        boxCollider.size = size;
    }

    private void createWall(string[] bits)
    {
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        float width = float.Parse(bits[3]);
        float depth = float.Parse(bits[4]);
        string brickType = bits[5];

        Vector3 position = new Vector3(positionX, positionY, 0);
        Vector2 size = new Vector2(width, depth);

        GameObject platform = instantiateObject(GameConstants.WALL);
        platform.transform.position = position;

        string folder = "Sprites/WallBricks/";
        if (brickType.StartsWith("Weird"))
            folder = "Sprites/WeirdBricks/";

        SpriteRenderer spriteRenderer = platform.GetComponent<SpriteRenderer>();
        spriteRenderer.size = size;
        spriteRenderer.sprite = Resources.Load<Sprite>(folder + brickType);

        BoxCollider2D boxCollider = platform.GetComponent<BoxCollider2D>();
        boxCollider.size = size;
    }

    private void createMovingObstacle(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        float minConstraint = float.Parse(bits[3]);
        float maxConstraint = float.Parse(bits[4]);
        string direction = bits[5];
        string sprite = bits[6];
        string id = bits[7];

        Vector3 position = new Vector3(positionX, positionY, 0);

        GameObject obstacle = instantiateObject(objectType);
        obstacle.transform.position = position;
        obstacle.GetComponent<ObstacleController>().ID = id;
        obstacle.name = id;

        Vector2 constraints = new Vector2(minConstraint, maxConstraint);
        MovingObstacleController controller = obstacle.GetComponent<MovingObstacleController>();
        switch(direction)
        {
            case GameConstants.HORIZONTAL:
                controller.Direction = MovementDirection.HORIZONTAL;
                break;
            case GameConstants.VERTICAL:
                controller.Direction = MovementDirection.VERTICAL;
                break;
            case GameConstants.CIRCLING:
                controller.Direction = MovementDirection.CIRCLING;
                break;
        }
        controller.MovementConstraints = constraints;

        SpriteRenderer spriteRenderer = obstacle.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/" + sprite + "SpriteSheet_0");

        Animator animator = obstacle.GetComponent<Animator>();
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/" + sprite + "AnimationController");

        this.currentRoom.AddCreature(id);
    }

    private void createStaticObstacle(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        string id = bits[3];

        Vector3 position = new Vector3(positionX, positionY, 0);

        GameObject obstacle = instantiateObject(objectType);
        obstacle.transform.position = position;
        obstacle.GetComponent<ObstacleController>().ID = id;
        obstacle.name = id;

        this.currentRoom.AddCreature(id);
    }

    private void createSceneryObstacle(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        float sizeX = float.Parse(bits[3]);
        float sizeY = float.Parse(bits[4]);
        bool isMovable = bool.Parse(bits[5]);
        string sprite = bits[6];

        Vector3 position = new Vector3(positionX, positionY, 0);
        Vector2 size = new Vector2(sizeX, sizeY);

        GameObject obstacle = instantiateObject(objectType);
        if (sprite.Equals(GameConstants.BED))
            obstacle.GetComponent<SceneryObstacleController>().IsBed = true;
        obstacle.transform.position = position;
        obstacle.name = sprite;

        SpriteRenderer spriteRenderer = obstacle.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load("Sprites/" + sprite, typeof(Sprite)) as Sprite;
        spriteRenderer.size = size;

        BoxCollider2D boxCollider = obstacle.GetComponent<BoxCollider2D>();
        boxCollider.size = size;

        Rigidbody2D rb = obstacle.GetComponent<Rigidbody2D>();
        rb.bodyType = isMovable ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;

        if (sprite.Equals("DirtBall"))
            rb.mass = 2.0f;
    }

    private void createLadder(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        float width = float.Parse(bits[3]);
        float length = float.Parse(bits[4]);

        Vector3 position = new Vector3(positionX, positionY, 0);
        Vector2 size = new Vector2(width, length);

        GameObject ladder = instantiateObject(objectType);
        ladder.transform.position = position;

        SpriteRenderer spriteRenderer = ladder.GetComponent<SpriteRenderer>();
        spriteRenderer.size = size;

        BoxCollider2D boxCollider = ladder.GetComponent<BoxCollider2D>();
        boxCollider.size = size;
    }


    private void createAerial(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);

        Vector3 position = new Vector3(positionX, positionY, 0);

        GameObject aerial = instantiateObject(objectType);
        aerial.transform.position = position;
    }

    private void createCrateShape(string[] bits)
    {
        //string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        bool isMovable = bool.Parse(bits[3]);
        string shape = bits[4];

        Vector3 position = new Vector3(positionX, positionY, 0);

        GameObject crateShape = instantiateObject(shape);
        crateShape.transform.position = position;

        Rigidbody2D rb = crateShape.GetComponent<Rigidbody2D>();
        rb.bodyType = isMovable ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
    }

    private void createRamp(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        float rotation = float.Parse(bits[3]);
        float width = float.Parse(bits[4]);
        float length = float.Parse(bits[5]);

        Vector3 position = new Vector3(positionX, positionY, 0);
        Vector2 size = new Vector2(width, length);

        GameObject ramp = instantiateObject(objectType);
        ramp.transform.position = position;
        ramp.transform.eulerAngles = new Vector3(0,0,rotation);

        SpriteRenderer spriteRenderer = ramp.GetComponent<SpriteRenderer>();
        spriteRenderer.size = size;

        BoxCollider2D boxCollider = ramp.GetComponent<BoxCollider2D>();
        boxCollider.size = size;
    }

    private void createRope(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        int units = int.Parse(bits[3]);

        Vector3 position = new Vector3(positionX, positionY, 0);

        GameObject ropeAnchor = instantiateObject(objectType);
        ropeAnchor.transform.position = position;

        GameObject prevSection = ropeAnchor;
        for (int i = 0; i < units; i++)
        {
            GameObject ropeSection = instantiateObject(GameConstants.ROPE_SECTION);
            ropeSection.transform.parent = ropeAnchor.transform;
            Vector3 ropeSectionPosition = ropeSection.transform.position;
            ropeSectionPosition.x = position.x;
            ropeSectionPosition.y = prevSection.transform.position.y - 0.1f;
            ropeSection.transform.position = ropeSectionPosition;
            prevSection = ropeSection;
        }
    }

    private void createItem(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        string id = bits[3];
        //int points = int.Parse(bits[4]);

        Vector3 position = new Vector3(positionX, positionY, 0);

        this.currentRoom.AddItem(id);

        GameObject item = instantiateObject(objectType);
        item.transform.position = position;
        item.GetComponent<ItemController>().ID = id;
        item.name = id;
        //Logger.Log("Add " + id);

        //item.GetComponent<ItemController>().Points = points;
    }

    private void createPowerUp(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        string id = bits[3];

        Vector3 position = new Vector3(positionX, positionY, 0);

        this.currentRoom.AddItem(id);

        GameObject powerup = instantiateObject(objectType);
        powerup.transform.position = position;
        powerup.GetComponent<ItemController>().ID = id;
        powerup.name = id;

        SpriteRenderer spriteRenderer = powerup.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/" + id + "SpriteSheet_0");

        Animator animator = powerup.GetComponent<Animator>();
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/" + id + "AnimationController");
    }

    private void createKey(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        string id = bits[3];

        Vector3 position = new Vector3(positionX, positionY, 0);

        this.currentRoom.AddItem(id);

        GameObject item = instantiateObject(objectType);
        item.transform.position = position;
        item.GetComponent<ItemController>().ID = id;
        item.name = id;
    }

    private void createDoor(string[] bits, string currentRoom)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        float sizeX = float.Parse(bits[3]);
        float sizeY = float.Parse(bits[4]);
        bool playSound = bool.Parse(bits[5]);
        string nextRoom = bits[6];

        Vector3 position = new Vector3(positionX, positionY, 0);
        Vector2 size = new Vector2(sizeX, sizeY);

        GameObject door = instantiateObject(objectType);
        door.transform.position = position;

        BoxCollider2D boxCollider = door.GetComponent<BoxCollider2D>();
        boxCollider.size = size;

        door.GetComponent<Door>().NextRoom = nextRoom;
        door.GetComponent<Door>().CurrentRoom = currentRoom;
        door.GetComponent<Door>().PlaySound = playSound;
    }

    private void createPortal(string[] bits, string currentRoom)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        float sizeX = float.Parse(bits[3]);
        float sizeY = float.Parse(bits[4]);
        bool playSound = bool.Parse(bits[5]);
        string nextRoom = bits[6];
        bool enterFromRight = bool.Parse(bits[7]);

        Vector3 position = new Vector3(positionX, positionY, 0);
        Vector2 size = new Vector2(sizeX, sizeY);

        GameObject portal = instantiateObject(objectType);
        portal.transform.position = position;
        portal.transform.localScale = enterFromRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);

        BoxCollider2D boxCollider = portal.GetComponent<BoxCollider2D>();
        boxCollider.size = size;
        Vector2 colliderPosition = new Vector2(-0.18f, 0f); // enterFromRight ? new Vector2(-0.18f, 0f) : new Vector2(0.18f, 0f);
        boxCollider.offset = colliderPosition;

        portal.GetComponent<Door>().NextRoom = nextRoom;
        portal.GetComponent<Door>().CurrentRoom = currentRoom;
        portal.GetComponent<Door>().PlaySound = playSound;
    }

    private void createMaid(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);

        Vector3 position = new Vector3(positionX, positionY, 0);

        GameObject maid = instantiateObject(objectType);
        maid.transform.position = position;
    }

    private void setupRoomPhysics(string[] bits)
    {
        float creatureSpeedMultipler = float.Parse(bits[1]);
        float playerSpeedMultiplier = float.Parse(bits[2]);
        float gravityXMultiplier = float.Parse(bits[3]);
        float gravityYMultiplier = float.Parse(bits[4]);
        PlayerControlBehavior playerControlBehaviour = ParseEnum<PlayerControlBehavior>(bits[5]);

        Vector2 gravity = Physics2D.gravity;
        Physics2D.gravity = new Vector2(gravity.x * gravityXMultiplier, gravity.y * gravityYMultiplier);

        this.currentRoom.CreatureSpeedMultiplier = creatureSpeedMultipler;
        this.currentRoom.PlayerSpeedMultiplier = playerSpeedMultiplier;
        this.currentRoom.GravityXMultiplier = gravityXMultiplier;
        this.currentRoom.GravityYMultiplier = gravityYMultiplier;
        this.currentRoom.PlayerControlBehavior = playerControlBehaviour;
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    #endregion
}
