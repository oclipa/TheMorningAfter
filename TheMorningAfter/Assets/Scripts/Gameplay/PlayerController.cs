using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.Events;

/// <summary>
/// Handles the Player object
/// </summary>
public class PlayerController : MonoBehaviour, IPlayerDiedInvoker
{
    private bool canMove = true;

    private Animator animator;
    private Rigidbody2D rb;

    // Player needs to know how to walk up ramps
    private Ramp ramp;

    //some flags to check when certain animations are playing
    private bool isJumping = false; // is the player jumping?
    private bool onLadder = false;

    //animation states - the values in the animator conditions
    private const int STATE_WALK_LEFT = 0;
    private const int STATE_WALK_RIGHT = 1;
    private const int STATE_IDLE = 2;
    private const int STATE_JUMP = 3;

    // Player should initially face right
    private string _currentDirection = GameConstants.RIGHT;

    // Player should initially be idle
    private int _currentAnimationState = STATE_IDLE;

    // Force to move the player will initially be applied to the right
    Vector3 forceDirection = Vector3.right;

    // Has the user died?
    //private bool isDead;

    PlayerDiedEvent playerDiedEvent;

    Vector3 startPosition;

    bool maybeFalling;
    float initialY;


    // Use this for initialization
    void Start()
    {
        //define the animator attached to the player
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();

        this.playerDiedEvent = new PlayerDiedEvent();
        EventManager.AddPlayerDiedInvoker(this);

        EventManager.AddNextRoomListener(leftRoom);

        // player starts off stationary
        animator.speed = 0;
        changeState(STATE_IDLE);

        this.startPosition = transform.position;
    }

    private void Update()
    {
        if (maybeFalling && !onLadder)
        {
            // can fall three body lengths without dying
            float currentY = transform.position.y;
            float deltaY = this.initialY - currentY;

            if (deltaY > 2.88f) // three body lengths
            {
                this.playerDiedEvent.Invoke(this.startPosition);
            }
        }

        if (canMove)
        {
            //Check for keyboard input
            if (Input.GetAxisRaw(GameConstants.HORIZONTAL) > 0)
            {
                changeDirection(GameConstants.RIGHT);
                transform.Translate(forceDirection * GameConstants.PLAYER_WALK_SPEED * Time.fixedDeltaTime);
                changeState(STATE_WALK_RIGHT);
            }
            else if (Input.GetAxisRaw(GameConstants.HORIZONTAL) < 0)
            {
                changeDirection(GameConstants.LEFT);
                transform.Translate(forceDirection * GameConstants.PLAYER_WALK_SPEED * Time.fixedDeltaTime);
                changeState(STATE_WALK_LEFT);
            }
            else
            {
                changeState(STATE_IDLE);
            }

            // Don't want to allow jumping while we are already jumping
            if (Input.GetKey(GameConstants.SPACE) && !isJumping)
            {
                jump();
            }
        }
    }

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

    /// <summary>
    /// Is the player currently jumping?
    /// </summary>
    /// <value><c>true</c> if is jumping; otherwise, <c>false</c>.</value>
    public bool IsJumping
    {
        get { return this.isJumping;  }
    }

    /// <summary>
    /// Handles jump action
    /// </summary>
    private void jump()
    {
        isJumping = true;

        float jumpX = GameConstants.PLAYER_JUMP_SPEED_X;
        if (_currentAnimationState == STATE_IDLE)
            jumpX = 0;

        if (_currentDirection == GameConstants.RIGHT)
        {
            rb.AddForce(new Vector2(jumpX, GameConstants.PLAYER_JUMP_SPEED_Y));
        }
        else if (_currentDirection == GameConstants.LEFT)
        {
            rb.AddForce(new Vector2(-1 * jumpX, GameConstants.PLAYER_JUMP_SPEED_Y));
        }
    }

    /// <summary>
    /// Change the obstacle's animation state
    /// </summary>
    /// <param name="state">State.</param>
    void changeState(int state)
    {
        if (_currentAnimationState == state)
            return;

        switch (state)
        {

            case STATE_WALK_RIGHT:
                animator.speed = 1;
                animator.SetInteger("state", STATE_WALK_RIGHT);
                break;

            case STATE_WALK_LEFT:
                animator.speed = 1;
                animator.SetInteger("state", STATE_WALK_LEFT);
                break;

            case STATE_IDLE:
                animator.speed = 0;
                break;
        }

        _currentAnimationState = state;
    }

    /// <summary>
    /// Check if player has collided with another object
    /// </summary>
    /// <param name="coll">Coll.</param>
    void OnCollisionEnter2D(Collision2D coll)
    {
        collisionEnter(coll.gameObject);
    }

    /// <summary>
    /// Check if player is in contact with another object
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        collisionStay(collision.gameObject);
    }

    /// <summary>
    /// Check if an player has exited contact with another obstacle
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnCollisionExit2D(Collision2D collision)
    {
        collisionExit(collision.gameObject);
    }

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
            canMove = false;
        }

        collisionEnter(collidedGameObject);
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
            canMove = true;
        }

        collisionExit(collidedGameObject);
    }

    private void collisionEnter(GameObject collidedGameObject)
    {
        // hit something, so can no longer be falling
        maybeFalling = false;

        // Did the player hit a platform?
        if (collidedGameObject.CompareTag(GameConstants.PLATFORM))
        {
            //stopJumping(collidedGameObject);
        }
        // or a rope?
        else if (collidedGameObject.CompareTag(GameConstants.ROPE_SECTION))
        {
            // If player is currently jumping and hits 
            // a rope (i.e. they are climbing), disable
            // further jumping until space is pressed again.
            // This avoids the player getting stuck on the
            // rope because they cannot jump off (because
            // the code thinks they are still jumping).
            if (isJumping)
                isJumping = false;
        }
        // or an obstacle?
        else if (collidedGameObject.CompareTag(GameConstants.OBSTACLE))
        {
            this.playerDiedEvent.Invoke(this.startPosition);
        }
        else if (collidedGameObject.CompareTag(GameConstants.LADDER))
        {
            onLadder = true;
        }
    }

    private void collisionStay(GameObject collidedGameObject)
    {
        // touching something, so can't be falling
        this.maybeFalling = false;

        // If the player is on a ramp, they need some extra help
        // to get up the slope (we get this by asking the ramp
        // for some help) 
        if (collidedGameObject.CompareTag(GameConstants.RAMP))
        {
            if (ramp == null)
                ramp = collidedGameObject.GetComponent<Ramp>();

            Vector2 playerVelocity = rb.velocity;
            Vector3 playerPosition = transform.position;
            ramp.HelpUpSlope(ref playerVelocity, ref playerPosition);

            rb.velocity = playerVelocity;
            transform.position = playerPosition;
        }
        else if (collidedGameObject.CompareTag(GameConstants.PLATFORM))
        {
            stopJumping(collidedGameObject);
        }
        else if (collidedGameObject.CompareTag(GameConstants.LADDER))
        {
            onLadder = true;
        }
    }

    private void collisionExit(GameObject collidedGameObject)
    {
        this.ramp = null;

        // no longer touching an object, so maybe falling
        this.maybeFalling = true;
        this.initialY = transform.position.y;

        if (collidedGameObject.CompareTag(GameConstants.LADDER))
        {
            onLadder = false;
        }
    }

    private void stopJumping(GameObject collidedGameObject)
    {
        Bounds collBounds =
           collidedGameObject.GetComponent<BoxCollider2D>().bounds;

        float collMaxX = collBounds.max.x;
        float collMinX = collBounds.min.x;
        float collMaxY = collBounds.max.y;
        //float collMinY = collBounds.min.y;

        float minCurrentX = transform.position.x - 0.16f;
        float maxCurrentX = transform.position.x + 0.16f;
        float currentY = transform.position.y;

        // only enabling jumping if the collision is with the upper surface
        // of the platform (not the side or the bottom).
        if (isJumping && maxCurrentX > collMinX && minCurrentX < collMaxX // tests for side
            && currentY > collMaxY) // tests for bottom
        {
            // If player is currently jumping and hits 
            // a platform (i.e. they have landed), disable
            // further jumping until space is pressed again.
            // This avoids multiple jumps magnifying each other
            // and the player bouncing out of control.
            isJumping = false;

            changeState(STATE_IDLE);
        }
    }

    /// <summary>
    /// Flip sprite for forward/backward movement
    /// </summary>
    /// <param name="direction">Direction.</param>
    void changeDirection(string direction)
    {
        if (_currentDirection != direction)
        {
            if (direction == GameConstants.RIGHT)
            {
                transform.Rotate(0, 180, 0);
                _currentDirection = GameConstants.RIGHT;
            }
            else if (direction == GameConstants.LEFT)
            {
                transform.Rotate(0, -180, 0);
                _currentDirection = GameConstants.LEFT;
            }
        }
    }
}