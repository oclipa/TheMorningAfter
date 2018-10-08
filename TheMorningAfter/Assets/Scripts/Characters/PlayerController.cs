using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Handles the Player object
/// </summary>
public class PlayerController : MonoBehaviour, IPlayerDiedInvoker
{
    // some flags to check when character is in certain states
    private bool isOnLadder;
    private bool isOnRope;
    private bool isGrounded;
    private bool maybeFalling;
    private bool isSolid;

    // Player has a PlayerState
    PlayerState currentPlayerState;

    // Player has Animator
    private Animator animator;

    // Player has RigidBody2D
    private Rigidbody2D rb;

    // Player faces in a particular direction
    private string currentDirection;

    // Player has an animation state
    private int currentAnimationState;

    // The X component of the slope of the current surface 
    // the player is on
    private float currentSlopeNormalX;

    // Player has a start position in the current room
    private Vector3 startPosition;

    // If player is falling, from what Y coordinate did they start to fall?
    private float initialY;

    // Player has a way to handle commands
    private CommandHandler commandHandler;

    // If player dies, need to notify death handlers
    PlayerDiedEvent playerDiedEvent;

    #region MonoBehaviour

    // Use this for initialization
    void Start()
    {
        this.animator = this.GetComponent<Animator>();
        this.rb = this.GetComponent<Rigidbody2D>();

        this.playerDiedEvent = new PlayerDiedEvent();
        EventManager.AddPlayerDiedInvoker(this);

        EventManager.AddNextRoomListener(leftRoom);

        // Player starts the room standing at the start position
        this.startPosition = transform.position;
        this.currentPlayerState = PlayerState.STANDING;

        this.commandHandler = new KeyboardCommandHandler();
    }

    private PlayerState lastState;

    private void Update()
    {
        checkFalling();

        if (!isOnRope)
        {
            //if (this.currentPlayerState != lastState)
            //{
            //    Debug.Log(this.currentPlayerState.GetType());
            //    lastState = this.currentPlayerState;
            //}
            this.currentPlayerState.UpdateInput(this);
        }
    }

    private void FixedUpdate()
    {
        if (!isOnRope)
        {
            this.currentPlayerState.UpdatePhysics(this);
            this.currentPlayerState.UpdateAnimation(this);
        }
    }

    #endregion

    #region Collisions

    /// <summary>
    /// Check if player has collided with another object
    /// </summary>
    /// <param name="coll">Coll.</param>
    void OnCollisionEnter2D(Collision2D coll)
    {
        interactionEnter(coll.gameObject);
    }

    /// <summary>
    /// Check if player is in contact with another object
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        interactionStay(collision.gameObject);
    }

    /// <summary>
    /// Check if an player has exited contact with another obstacle
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnCollisionExit2D(Collision2D collision)
    {
        interactionExit(collision.gameObject);
    }

    #endregion

    #region Triggers

    /// <summary>
    /// Check if player has entered a trigger zone 
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidedGameObject = collision.gameObject;

        // TODO: Not clear about the best way to handle movement
        // on ropes
        if (collidedGameObject.CompareTag(GameConstants.ROPE_SECTION))
        {
            isOnRope = true; // cannot move independently of rope
        }

        interactionEnter(collidedGameObject);
    }

    /// <summary>
    /// Check if player has exited a trigger zone
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject collidedGameObject = collision.gameObject;

        // TODO: Not clear about the best way to handle movement
        // on ropes
        if (collidedGameObject.CompareTag(GameConstants.ROPE_SECTION))
        {
            isOnRope = false; // no longer holding the rope, so can move independently
        }

        interactionExit(collidedGameObject);
    }

    /// <summary>
    /// Check if player is in contact with a trigger zone
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        interactionStay(collision.gameObject);
    }

    #endregion

    #region Interactions

    private void interactionEnter(GameObject collidedGameObject)
    {
        //Debug.Log("Entered " + collidedGameObject.tag);

        // hit something, so can no longer be falling
        this.maybeFalling = false;

        this.isSolid = true;

        // Did the player hit a platform?
        if (collidedGameObject.CompareTag(GameConstants.PLATFORM))
        {
            // have we landed squarely on a platform?
            checkGrounded(collidedGameObject);
        }
        // or a ramp?
        else if (collidedGameObject.CompareTag(GameConstants.RAMP))
        {
            // have we landed squarely on a ramp?
            checkGrounded(collidedGameObject);
        }
        // or a rope?
        else if (collidedGameObject.CompareTag(GameConstants.ROPE_SECTION))
        {
            isOnRope = true;
        }
        // or a ladder?
        else if (collidedGameObject.CompareTag(GameConstants.LADDER))
        {
            this.isOnLadder = true;
        }
        // or some scenery?
        else if (collidedGameObject.CompareTag(GameConstants.SCENERY_OBSTACLE))
        {
            // have we landed squarely on the scenery obstacle?
            checkGrounded(collidedGameObject);
        }
        // or an obstacle?
        else if (collidedGameObject.CompareTag(GameConstants.OBSTACLE))
        {
            this.playerDiedEvent.Invoke(this.startPosition);
        }
    }

    private void interactionStay(GameObject collidedGameObject)
    {
        //Debug.Log("Touching " + collidedGameObject.tag);

        // touching something, so can't be falling
        this.maybeFalling = false;

        // If the player is on a ramp, they need some extra help
        // to get up the slope (we get this by asking the ramp
        // for some help) 
        if (collidedGameObject.CompareTag(GameConstants.RAMP))
        {
            if (currentSlopeNormalX == 0.0f)
                currentSlopeNormalX = collidedGameObject.GetComponent<Ramp>().SlopeNormalX;
        }
        else if (collidedGameObject.CompareTag(GameConstants.PLATFORM))
        {
            // have we landed squarely on a platform?
            checkGrounded(collidedGameObject);
        }
        else if (collidedGameObject.CompareTag(GameConstants.RAMP))
        {
            // have we landed squarely on a ramp?
            checkGrounded(collidedGameObject);
        }
        else if (collidedGameObject.CompareTag(GameConstants.LADDER))
        {
            this.isOnLadder = true;
        }
        else if (collidedGameObject.CompareTag(GameConstants.ROPE_SECTION))
        {
            this.isOnRope = true;
        }
        else if (collidedGameObject.CompareTag(GameConstants.SCENERY_OBSTACLE))
        {
            // have we landed squarely on the scenery obstacle?
            checkGrounded(collidedGameObject);
        }

    }

    private void interactionExit(GameObject collidedGameObject)
    {
        //Debug.Log("Exited " + collidedGameObject.tag);

        this.isGrounded = false;

        // no longer touching an object, so maybe falling
        this.maybeFalling = true;
        this.initialY = transform.position.y;

        if (collidedGameObject.CompareTag(GameConstants.LADDER))
        {
            this.isOnLadder = false;
        }
        else if (collidedGameObject.CompareTag(GameConstants.ROPE_SECTION))
        {
            this.isOnRope = false;
        }
        else if (collidedGameObject.CompareTag(GameConstants.RAMP))
        {
            this.currentSlopeNormalX = 0.0f;
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// If user has left room, need to load the next room
    /// </summary>
    private void leftRoom(string nextRoom, string currentRoom)
    {
        //Debug.Log("Player left room");
        Destroy(gameObject);
    }

    /// <summary>
    /// Adds the player died listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    public void AddPlayerDiedListener(UnityAction<Vector3> listener)
    {
        playerDiedEvent.AddListener(listener);
    }

    #endregion

    #region Private Methods

    private void checkFalling()
    {
        // if we are moving downwards but not climbing, we must be falling
        if (maybeFalling && this.currentPlayerState != PlayerState.CLIMBING)
        {
            // can fall three body lengths without dying
            float currentY = transform.position.y;
            float deltaY = this.initialY - currentY;

            if (deltaY > 2.88f) // three body lengths
            {
                this.playerDiedEvent.Invoke(this.startPosition);
            }
        }
    }

    private void checkGrounded(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag(GameConstants.RAMP))
        {
            this.isGrounded = true;
        }
        else if (collidedGameObject.CompareTag(GameConstants.PLATFORM))
        {
            Bounds collBounds =
                collidedGameObject.GetComponent<BoxCollider2D>().bounds;

            float collMaxX = collBounds.max.x;
            float collMinX = collBounds.min.x;
            float collMaxY = collBounds.max.y;

            float minCurrentX = transform.position.x - 0.16f;
            float maxCurrentX = transform.position.x + 0.16f;
            float currentY = transform.position.y;

            // only enabling jumping if the collision is with the upper surface
            // of the platform (not the side or the bottom).
            if (maxCurrentX > collMinX && minCurrentX < collMaxX // tests for side
                && currentY > collMaxY) // tests for bottom
            {
                // If player is currently jumping and hits 
                // a platform (i.e. they have landed), disable
                // further jumping until space is pressed again.
                // This avoids multiple jumps magnifying each other
                // and the player bouncing out of control.

                this.isGrounded = true;
            }
        }
    }

    #endregion

    #region Properties

    public PlayerState PlayerState
    {
        get { return this.currentPlayerState; }
        set { this.currentPlayerState = value; }
    }

    public string CurrentDirection
    {
        get { return this.currentDirection; }
        set { this.currentDirection = value; }
    }

    public int CurrentAnimationState
    {
        get { return this.currentAnimationState; }
        set { this.currentAnimationState = value; }
    }

    public Animator Animator
    {
        get { return this.animator; }
    }

    public Transform Transform
    {
        get { return this.transform; }
    }

    public Rigidbody2D Rigidbody2D
    {
        get { return this.rb; }
    }

    public bool IsOnLadder
    {
        get { return this.isOnLadder; }
    }

    public bool IsOnRope
    {
        get { return this.isOnRope; }
    }

    public bool IsGrounded
    {
        get { return this.isGrounded; }
    }

    public bool IsSolid
    {
        get { return this.isSolid; }
        set { this.isSolid = value; }
    }

    public float CurrentSlopeNormalX
    {
        get { return this.currentSlopeNormalX; }
    }

    public CommandHandler CommandHandler
    {
        get { return this.commandHandler; }
    }

    #endregion
}