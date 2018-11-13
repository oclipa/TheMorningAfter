using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Builds the current room
/// </summary>
public class RoomBuilder : MonoBehaviour {

    //private const string START_ROOM = "TheBathroom";
    //private static Vector3 START_POSITION = new Vector3(0.82f, -0.185f, 0);

    private static string START_ROOM = "TheGarden";
    private static Vector3 START_POSITION = new Vector3(-8.24f, 1.96f, 0);

    //float minPlatformY = -2.24f;
    //float maxPlatformY = 4.84f;
    //float minPlatformDepth = 0.32f;
    //float minWallX = -8.73f;
    //float maxWallX = 8.73f;
    //float minWallWidth = 0.32f;
    //float screenWidth = 17.78f; // (8.73 * 2 + 32)
    //float screenHeight = 7.08f; // 4.84 + 2.24

    //float minDoorHeightEasy = 1.20f; // player height
    //float minDoorHeightMedium = 1.08f; // player height
    //float minDoorHeightHard = 0.96f; // player height
    //float minHoleWidth = 1.28f; // 2 x player width

    //float maxJumpHeightEasy = 1.28f;
    //float maxJumpHeightMedium = 1.40f;
    //float maxJumpHeightHard = 1.52f;

    private IRoom currentRoom;
    private string prevMusic;

    // Use this for initialization
    void Start ()
    {
        Blueprints.ResetRooms();

        // build the first room
        currentRoom = Blueprints.GetRoom(START_ROOM);
        BuildRoom(currentRoom, START_POSITION); // TheBathroom
        //BuildRoom(currentRoom, new Vector3(-8.5f, -2.0f, 0)); // TheLoft
        //BuildRoom(currentRoom, new Vector3(7.43f, 0f, 0)); // TheTreeTrunk

        // initialize the screen boundaries
        ScreenUtils.Initialize();

        // start room music
        playMusic();

        // Listen for when we need to build a new room
        EventManager.AddNextRoomListener(buildNextRoom);
    }

    private void playMusic()
    {
        string newMusic = currentRoom.GetMusic();

        if (!newMusic.Equals(prevMusic))
        {
            // stop any existing music
            AudioManager.Stop();

            // and start with the new music
            if (string.IsNullOrEmpty(newMusic))
            {
                AudioManager.Play(AudioClipName.GameMusic);
            }
            else
            {
                AudioManager.Play(newMusic);
            }
        }

        prevMusic = newMusic;
    }

    /// <summary>
    /// If user has left current room; need to load the next room
    /// </summary>
    private void buildNextRoom(string nextRoom, string currentRoom)
    {
        //Debug.Log("Building next room");
        //AudioManager.PlayOneShot(AudioClipName.ExitRoom);

        IRoom room = Blueprints.GetRoom(nextRoom);
        if (room != null)
        {
            Vector3 startPosition = room.GetStartPosition(currentRoom);
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
        currentRoom = room;

        // first destroy all existing objects in the room
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
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
                    !o.CompareTag("LivesRemainingText") && 
                    !o.CompareTag("ItemsCollectedText"))
                Destroy(o);
        }

        string roomName = room.GetName();

        ReadOnlyCollection<string> roomParameters = room.GetRoomDetails();

        foreach (string entry in roomParameters)
        {
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
            else if (bits[0].Equals(GameConstants.DOOR))
            {
                createDoor(bits, room.GetID());
            }
            else if (bits[0].Equals(GameConstants.MARIA))
            {
                createMaria(bits);
            }
        }

        GameObject roomTitleCanvas = GameObject.FindGameObjectWithTag(GameConstants.SCOREBOARD);
        if (roomTitleCanvas == null)
            roomTitleCanvas = Object.Instantiate(Resources.Load("ScoreBoard/" + GameConstants.ROOMTITLECANVAS)) as GameObject;
        GameObject.FindGameObjectWithTag(GameConstants.ROOMTITLETEXT).GetComponent<Text>().text = roomName;

        PlayerSpawner.SpawnNewPlayer(startPosition);

        playMusic();
    }

    private GameObject instantiateObject(string objectType)
    {
        return Object.Instantiate(Resources.Load("Objects/" + objectType)) as GameObject;
    }

    public IRoom CurrentRoom
    {
        get { return this.currentRoom; }
    }


    private void createPlatform(string[] bits)
    {
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        float width = float.Parse(bits[3]);
        float depth = float.Parse(bits[4]);

        Vector3 position = new Vector3(positionX, positionY, 0);
        Vector2 size = new Vector2(width, depth);

        //StringBuilder sb = new StringBuilder();
        //sb.AppendLine(objectType);
        //sb.AppendLine("Position: " + position.x + ", " + position.y + ", " + position.z);
        //sb.AppendLine("Size: " + size.x + ", " + size.y);
        //Debug.Log(sb.ToString());

        GameObject platform = instantiateObject(GameConstants.PLATFORM);
        platform.transform.position = position;

        SpriteRenderer spriteRenderer = platform.GetComponent<SpriteRenderer>();
        spriteRenderer.size = size;

        BoxCollider2D boxCollider = platform.GetComponent<BoxCollider2D>();
        boxCollider.size = size;
    }

    private void createWall(string[] bits)
    {
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        float width = float.Parse(bits[3]);
        float depth = float.Parse(bits[4]);

        Vector3 position = new Vector3(positionX, positionY, 0);
        Vector2 size = new Vector2(width, depth);

        //StringBuilder sb = new StringBuilder();
        //sb.AppendLine(objectType);
        //sb.AppendLine("Position: " + position.x + ", " + position.y + ", " + position.z);
        //sb.AppendLine("Size: " + size.x + ", " + size.y);
        //Debug.Log(sb.ToString());

        GameObject platform = instantiateObject(GameConstants.WALL);
        platform.transform.position = position;

        SpriteRenderer spriteRenderer = platform.GetComponent<SpriteRenderer>();
        spriteRenderer.size = size;

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

        Vector3 position = new Vector3(positionX, positionY, 0);

        //StringBuilder sb = new StringBuilder();
        //sb.AppendLine(objectType);
        //sb.AppendLine("Position: " + platformPosition.x + ", " + platformPosition.y + ", " + platformPosition.z);
        //sb.AppendLine("Size: " + platformSize.x + ", " + platformSize.y);
        //sb.AppendLine("Direction: " + direction);
        //Debug.Log(sb.ToString());

        GameObject obstacle = instantiateObject(objectType);
        obstacle.transform.position = position;

        Vector2 constraints = new Vector2(minConstraint, maxConstraint);
        MovingObstacleController controller = obstacle.GetComponent<MovingObstacleController>();
        controller.Direction = direction.Equals(GameConstants.HORIZONTAL) ? MovementDirection.HORIZONTAL : MovementDirection.VERTICAL;
        controller.MovementConstraints = constraints;
    }

    private void createStaticObstacle(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);

        Vector3 position = new Vector3(positionX, positionY, 0);

        //StringBuilder sb = new StringBuilder();
        //sb.AppendLine(objectType);
        //sb.AppendLine("Position: " + platformPosition.x + ", " + platformPosition.y + ", " + platformPosition.z);
        //sb.AppendLine("Size: " + platformSize.x + ", " + platformSize.y);
        //sb.AppendLine("Direction: " + direction);
        //Debug.Log(sb.ToString());

        GameObject obstacle = instantiateObject(objectType);
        obstacle.transform.position = position;
    }

    private void createSceneryObstacle(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        float sizeX = float.Parse(bits[3]);
        float sizeY = float.Parse(bits[4]);
        string sprite = bits[5];

        Vector3 position = new Vector3(positionX, positionY, 0);
        Vector2 size = new Vector2(sizeX, sizeY);

        //StringBuilder sb = new StringBuilder();
        //sb.AppendLine(objectType);
        //sb.AppendLine("Position: " + platformPosition.x + ", " + platformPosition.y + ", " + platformPosition.z);
        //sb.AppendLine("Size: " + platformSize.x + ", " + platformSize.y);
        //sb.AppendLine("Sprite: " + sprite);
        //Debug.Log(sb.ToString());

        GameObject obstacle = instantiateObject(objectType);
        if (sprite.Equals(GameConstants.BED))
            obstacle.GetComponent<SceneryObstacleController>().IsBed = true;
        obstacle.transform.position = position;

        SpriteRenderer spriteRenderer = obstacle.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load("Sprites/" + sprite, typeof(Sprite)) as Sprite;
        spriteRenderer.size = size;

        BoxCollider2D boxCollider = obstacle.GetComponent<BoxCollider2D>();
        boxCollider.size = size;
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

        //StringBuilder sb = new StringBuilder();
        //sb.AppendLine(ladderType);
        //sb.AppendLine("Position: " + position.x + ", " + position.y + ", " + position.z);
        //sb.AppendLine("Size: " + size.x + ", " + size.y);
        //Debug.Log(sb.ToString());

        GameObject ladder = instantiateObject(objectType);
        ladder.transform.position = position;

        SpriteRenderer spriteRenderer = ladder.GetComponent<SpriteRenderer>();
        spriteRenderer.size = size;

        BoxCollider2D boxCollider = ladder.GetComponent<BoxCollider2D>();
        boxCollider.size = size;
    }

    private void createRamp(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        float width = float.Parse(bits[3]);
        float length = float.Parse(bits[4]);

        Vector3 position = new Vector3(positionX, positionY, 0);
        Vector2 size = new Vector2(width, length);

        //StringBuilder sb = new StringBuilder();
        //sb.AppendLine(objectType);
        //sb.AppendLine("Position: " + position.x + ", " + position.y + ", " + position.z);
        //sb.AppendLine("Size: " + size.x + ", " + size.y);
        //Debug.Log(sb.ToString());

        GameObject ramp = instantiateObject(objectType);
        ramp.transform.position = position;

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

        //StringBuilder sb = new StringBuilder();
        //sb.AppendLine(objectType);
        //sb.AppendLine("Position: " + position.x + ", " + position.y + ", " + position.z);
        //sb.AppendLine("Length: " + units);
        //Debug.Log(sb.ToString());

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

        //StringBuilder sb = new StringBuilder();
        //sb.AppendLine(objectType);
        //sb.AppendLine("Position: " + position.x + ", " + position.y + ", " + position.z);
        //sb.AppendLine("Points: " + points);
        //Debug.Log(sb.ToString());

        bool added = this.currentRoom.AddItem(id);

        if (added)
        {
            GameObject item = instantiateObject(objectType);
            item.transform.position = position;
            item.GetComponent<ItemController>().ID = id;
        }

        //item.GetComponent<ItemController>().Points = points;
    }

    private void createDoor(string[] bits, string currentRoom)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);
        float sizeX = float.Parse(bits[3]);
        float sizeY = float.Parse(bits[4]);
        string nextRoom = bits[5];

        Vector3 position = new Vector3(positionX, positionY, 0);
        Vector2 size = new Vector2(sizeX, sizeY);

        //StringBuilder sb = new StringBuilder();
        //sb.AppendLine(objectType);
        //sb.AppendLine("Position: " + platformPosition.x + ", " + platformPosition.y + ", " + platformPosition.z);
        //sb.AppendLine("Size: " + platformSize.x + ", " + platformSize.y);
        //Debug.Log(sb.ToString());

        GameObject door = instantiateObject(objectType);
        door.transform.position = position;

        BoxCollider2D boxCollider = door.GetComponent<BoxCollider2D>();
        boxCollider.size = size;

        door.GetComponent<Door>().NextRoom = nextRoom;
        door.GetComponent<Door>().CurrentRoom = currentRoom;
    }

    private void createMaria(string[] bits)
    {
        string objectType = bits[0].ToUpper();
        float positionX = float.Parse(bits[1]);
        float positionY = float.Parse(bits[2]);

        Vector3 position = new Vector3(positionX, positionY, 0);

        //StringBuilder sb = new StringBuilder();
        //sb.AppendLine(objectType);
        //sb.AppendLine("Position: " + position.x + ", " + position.y + ", " + position.z);
        //Debug.Log(sb.ToString());

        GameObject maria = instantiateObject(objectType);
        maria.transform.position = position;
    }
}
