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
    [SerializeField]
    private bool enableTrail; // for debugging motion issues

    // some flags to check when character is in certain states
    private bool isOnLadder; // is the player touching a ladder?
    private bool isOnRope; // not currently used
    private bool isGrounded; // is the player touching the ground?
    private bool maybeFalling; // is the player falled?
    private bool isSolid; // is the player solid?  Can they pass through objects?

    // Player has a PlayerState
    PlayerState currentPlayerState; // walking, jumping, standing, etc.

    // Player has Animator
    private Animator animator;

    // Player has RigidBody2D
    private Rigidbody2D rb;

    // Player faces in a particular direction
    private string currentDirection;

    // Player has an animation state
    private int currentAnimationState; // walking, standing, climbing, etc.

    // The X component of the slope of the current surface 
    // the player is on
    private float currentSlopeNormalX;

    // Player has a start position in the current room
    private Vector3 startPosition;

    // If player is falling, from what Y coordinate did they start to fall?
    private float initialY;

    // distance between center of player to ground
    private float distToGround;

    // half-width of player
    private float halfWidth;

    // The speed multiplier for the player
    private float playerSpeedMultiplier;

    // The behaviour of the player controls
    private PlayerControlBehavior playerControlBehavior;

    // Player has a way to handle commands
    private CommandHandler commandHandler;

    // If player dies, need to notify death handlers
    PlayerDiedEvent playerDiedEvent;
    bool isDead;

    // The current movement state of the player
    private PlayerState lastState;

    // used for debugging
    private TrailRenderer trailRenderer;

    // the current game state
    private GameState gameState;

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
        IRoom currentRoom = Camera.main.GetComponent<RoomBuilder>().CurrentRoom;
        this.playerSpeedMultiplier = currentRoom.PlayerSpeedMultiplier;
        this.playerControlBehavior = currentRoom.PlayerControlBehavior;

        Vector2 playerSize = this.GetComponent<SpriteRenderer>().size;
        this.distToGround = playerSize.y / 2;
        this.halfWidth = playerSize.x / 2;

        // set sprite orientation based on gravity direction
        float gravityDirection = Mathf.Sign(Physics2D.gravity.y);
        if (gravityDirection > 0f)
            this.transform.Rotate(180, 0, 0);

        this.trailRenderer = GetComponent<TrailRenderer>();

        this.gameState = Camera.main.GetComponent<GameState>();
        if (this.gameState.PlayerHasTheWeapon)
            this.gameState.AddWeaponToPlayer(false);

        this.commandHandler = this.gameState.GetCommandHandler();
    }

    private void Update()
    {
        this.trailRenderer.enabled = this.enableTrail;

        checkFalling();

        if (!isOnRope)
        {
            // check the current command input
            this.currentPlayerState.UpdateInput(this);
        }
    }

    private void FixedUpdate()
    {
        if (!isOnRope)
        {
            // update the physics and animation based 
            // on the current command input
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
        if (gameObject != null)
        {
            // hit something, so can no longer be falling
            this.maybeFalling = false;

            // assume we don't want to pass through the object
            this.isSolid = true;

            // Did the player hit a platform?
            if (collidedGameObject.CompareTag(GameConstants.PLATFORM))
            {
                // have we landed squarely on a platform?
                checkGrounded(collidedGameObject);
            }
            // or a wall
            else if (collidedGameObject.CompareTag(GameConstants.WALL))
            {
                // have we squarely hit a wall?
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
                // have we landed squarely on a ladder?
                checkGrounded(collidedGameObject);
                this.isOnLadder = true;
            }
            // or some scenery?
            else if (collidedGameObject.CompareTag(GameConstants.SCENERY_OBSTACLE))
            {
                // have we landed squarely on a scenery obstacle?
                checkGrounded(collidedGameObject);
            }
            // or an obstacle?
            else if (collidedGameObject.CompareTag(GameConstants.OBSTACLE))
            {
                // obstacles always kill the player

                if (this.playerDiedEvent != null && !this.isDead)
                {
                    this.isDead = true;
                    this.playerDiedEvent.Invoke(this.startPosition);
                }
            }
        }
    }

    private void interactionStay(GameObject collidedGameObject)
    {
        // touching something, so probably not falling
        // (possible bug/feature: falling while touching an object means
        // that the falling does not register as falling)
        this.maybeFalling = false;

        // If the player is on a ramp, they need some extra help
        // to get up the slope (we get this by asking the ramp
        // for some help) 
        if (collidedGameObject.CompareTag(GameConstants.RAMP))
        {
            // have we landed squarely on a ramp?
            checkGrounded(collidedGameObject);

            // floating point comparison!
            if (currentSlopeNormalX == 0.0f)
                currentSlopeNormalX = collidedGameObject.GetComponent<Ramp>().SlopeNormalX;
        }
        else if (collidedGameObject.CompareTag(GameConstants.PLATFORM))
        {
            // are we squarely touching a platform?
            checkGrounded(collidedGameObject);
        }
        else if (collidedGameObject.CompareTag(GameConstants.WALL))
        {
            // are we squarely touching a wall?
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
            // are we squarely touching a scenery obstacle?
            checkGrounded(collidedGameObject);
        }
    }

    private void interactionExit(GameObject collidedGameObject)
    {
        // initially assume we are not touching the ground
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
        else
        {
            checkGrounded(collidedGameObject);
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// If user has left room, need to load the next room
    /// </summary>
    private void leftRoom(string nextRoom, string currentRoom, bool playDoorSound)
    {
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
            // can fall four body lengths without dying
            float currentY = transform.position.y;
            float deltaY = this.initialY - currentY;

            if (deltaY > GameConstants.MAX_FALL_DISTANCE && !this.isDead) // four body lengths
            {
                this.isDead = true;
                this.playerDiedEvent.Invoke(this.startPosition);
            }
        }
    }

    private void checkGrounded(GameObject collidedGameObject)
    {
        bool prevGroundedState = this.isGrounded;

        if (collidedGameObject.CompareTag(GameConstants.RAMP))
        {
            // assume that simply touching a ramp means we are grounded
            this.isGrounded = true;
        }
        else if (collidedGameObject.CompareTag(GameConstants.LADDER))
        {
            // assume that simply touching a ladder means we are grounded
            this.isGrounded = true;
        }
        else if (collidedGameObject.CompareTag(GameConstants.PLATFORM)
                    || collidedGameObject.CompareTag(GameConstants.WALL) 
                    || collidedGameObject.CompareTag(GameConstants.SCENERY_OBSTACLE))
        {
            // We are raycasting to check if the player is over an object.
            // Two rays are emitted downwards, one from the player's left and one from the player's right.
            // Slight offsets in the starting positions of the rays are included to accomodate
            // the fact that the box collider for the player is slightly smaller than the sprite.

            // need to take into account current direction of gravity (only consider y for time being)
            float gravityDirection = Mathf.Sign(Physics2D.gravity.y); // everyday direction is down - negative

            Vector3 playerPosition = transform.position;
            // get position of feet of player (could be upside down if gravity reversed)
            float dist = playerPosition.y + (gravityDirection * distToGround);
            // generate ray from left side of player, in the direction of gravity
            Vector3 rayCastPositionLeft = new Vector3(playerPosition.x - (halfWidth - 0.16f), dist, playerPosition.z);
            // generate ray from right side of player, in the direction of gravity
            Vector3 rayCastPositionRight = new Vector3(playerPosition.x + (halfWidth - 0.16f), dist, playerPosition.z);
            // which way is down?
            Vector3 down = gravityDirection * Vector3.up;
            // did either ray hit anything?  If so, player is grounded.
            this.isGrounded = Physics2D.Raycast(rayCastPositionLeft, down, 0.08f) || Physics2D.Raycast(rayCastPositionRight, down, 0.08f);
            Debug.DrawRay(rayCastPositionLeft, down);
            Debug.DrawRay(rayCastPositionRight, down);
        }
        else
        {
            this.isGrounded = false;
        }

        // if player was not grounded, but now is
        // assume they are initially stationary.
        if (this.isGrounded && !prevGroundedState)
            this.CurrentDirection = GameConstants.STATIONARY;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Walking, jumping, climbing, standing, etc.
    /// </summary>
    /// <value>The state of the player.</value>
    public PlayerState PlayerState
    {
        get { return this.currentPlayerState; }
        set { this.currentPlayerState = value; }
    }

    /// <summary>
    /// left, right, up, down, stationary
    /// </summary>
    /// <value>The current direction.</value>
    public string CurrentDirection
    {
        get { return this.currentDirection; }
        set { this.currentDirection = value; }
    }

    /// <summary>
    /// As configured in the animation controller
    /// </summary>
    /// <value>The state of the current animation.</value>
    public int CurrentAnimationState
    {
        get { return this.currentAnimationState; }
        set { this.currentAnimationState = value; }
    }

    /// <summary>
    /// Gets the Animator component for the player.
    /// </summary>
    /// <value>The animator.</value>
    public Animator Animator
    {
        get { return this.animator; }
    }

    /// <summary>
    /// Gets the Transform component for the player.
    /// </summary>
    /// <value>The transform.</value>
    public Transform Transform
    {
        get { return this.transform; }
    }

    /// <summary>
    /// Gets the Rigidbody2D component for the player.
    /// </summary>
    /// <value>The rigidbody2 d.</value>
    public Rigidbody2D Rigidbody2D
    {
        get { return this.rb; }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:PlayerController"/> is on a ladder.
    /// </summary>
    /// <value><c>true</c> if is on a ladder; otherwise, <c>false</c>.</value>
    public bool IsOnLadder
    {
        get { return this.isOnLadder; }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:PlayerController"/> is on a rope.
    /// </summary>
    /// <value><c>true</c> if is on a rope; otherwise, <c>false</c>.</value>
    public bool IsOnRope
    {
        get { return this.isOnRope; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:PlayerController"/> is grounded.
    /// </summary>
    /// <value><c>true</c> if is grounded; otherwise, <c>false</c>.</value>
    public bool IsGrounded
    {
        get { return this.isGrounded; }
        set { this.isGrounded = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:PlayerController"/> can pass through objects.
    /// </summary>
    /// <value><c>true</c> if is solid; otherwise, <c>false</c>.</value>
    public bool IsSolid
    {
        get { return this.isSolid; }
        set { this.isSolid = value; }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:PlayerController"/> has the weapon.
    /// </summary>
    /// <value><c>true</c> if has weapon; otherwise, <c>false</c>.</value>
    public bool HasWeapon
    {
        get { return this.gameState.PlayerHasTheWeapon; }
    }

    /// <summary>
    /// The X-component of the angle normal to the current slope
    /// </summary>
    /// <value>The current slope normal x.</value>
    public float CurrentSlopeNormalX
    {
        get { return this.currentSlopeNormalX; }
    }

    /// <summary>
    /// Handles all commands received in the current frame
    /// </summary>
    /// <value>The command handler.</value>
    public CommandHandler CommandHandler
    {
        get { return this.commandHandler; }
    }

    /// <summary>
    /// Gets the player speed multiplier.
    /// </summary>
    /// <value>The player speed multiplier.</value>
    public float PlayerSpeedMultiplier
    {
        get { return this.playerSpeedMultiplier; }
    }


    /// <summary>
    /// Gets the difficulty multiplier.
    /// </summary>
    /// <value>The difficulty multiplier.</value>
    public float DifficultyMultiplier
    {
        get { return this.gameState.DifficultyMultiplier; }
    }

    /// <summary>
    /// Gets the player control behavior.
    /// </summary>
    /// <value>The player control behavior.</value>
    public PlayerControlBehavior PlayerControlBehavior
    {
        get { return this.playerControlBehavior; }
    }

    #endregion
}