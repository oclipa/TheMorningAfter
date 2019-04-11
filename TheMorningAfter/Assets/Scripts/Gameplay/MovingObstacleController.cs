using UnityEngine;
using System.Collections;

/// <summary>
/// Handles moving creatures.
/// 
/// There are three basic creature movement modes:
/// 1) Patrol back-and-forth between constraints specified in room config file.
/// 2) Patrol back-and-forth along the length of specific platform or wall
/// 3) Rotate (in the Y plane) around a fixed point.
/// 
/// Strictly speaking this should be called MovingCreatureController, although
/// the original reqirements did not explicitly require all obstacles
/// to be creatures (hence the more generic "obstacle").
/// </summary>
public class MovingObstacleController : ObstacleController
{
    [SerializeField]
    private bool log; // for debugging

    private float walkSpeed; // default speed at which creatures walk

    // By default, the creature can move as far as ranges indicated in
    // in the config file for the room, which are represented by the
    // following property.  
    // (however, creature will also reverse direction if it encounters 
    // a platform, wall or scenery obstacle)
    //
    // In rotating objects, the movement constraints indicate the X,Y 
    // values of the center of rotation.
    private Vector2 movementConstraints =
                new Vector2(float.MinValue, float.MaxValue);

    // Animates the creature
    private Animator animator;

    //animation states - the values in the animator conditions
    private const int STATE_MOVE_BACKWARDS = 0;
    private const int STATE_WALK_FORWARDS = 1;
    private const int STATE_IDLE = 2;

    // creature is initially facing "forward"
    private string currentDirection = GameConstants.FORWARD;

    // creature is initially idle
    private int currentAnimationState = STATE_IDLE;

    // The speed multiplier for the creature
    private float speedMultiplier;

    // The difficulty multiplier for the creature
    private float difficultyMultiplier;

    // Is the creature in the process of toggling its direction?
    bool isToggling;

    // Is the creature allowed to toggle its direction at the minute?
    bool toggleAllowed = true;

    // circling behaviour
    private Rotater2D rotator;

    // the direction in which force is applied to get the
    // creature to move
    Vector3 forceDirection;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MovingObstacleController"/> class.
    /// </summary>
    public MovingObstacleController()
        : base()
    {
        this.direction = MovementDirection.STATIC;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MovingObstacleController"/> class.
    /// </summary>
    /// <param name="direction">Direction.</param>
    public MovingObstacleController(MovementDirection direction)
        : base()
    {
        setDirection(direction);
    }

    /// <summary>
    /// Set the direction in which the creature can move
    /// </summary>
    /// <param name="newDirection">Direction.</param>
    private void setDirection(MovementDirection newDirection)
    {
        this.direction = newDirection;

        switch (this.direction)
        {
            case MovementDirection.HORIZONTAL:
                forceDirection = Vector3.right;
                break;
            case MovementDirection.VERTICAL:
                forceDirection = Vector3.up;
                break;
            case MovementDirection.CIRCLING:
                forceDirection = Vector3.zero;
                break;
        }
    }

    /// <summary>
    /// Start the creature moving
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // get the speed multiplier for the current room
        IRoom currentRoom = Camera.main.GetComponent<RoomBuilder>().CurrentRoom;
        this.speedMultiplier = currentRoom.CreatureSpeedMultiplier;
        this.difficultyMultiplier = gameState.DifficultyMultiplier;

        // initialize initial movement properties
        switch (direction)
        {
            case MovementDirection.HORIZONTAL:
                rb.gravityScale = 0; // creatures are not affected by gravity
                // prevent all rotation, but particularly into the Z plane (i.e. around the Y axis)
                rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                walkSpeed = GameConstants.OBSTACLE_WALK_SPEED_X; // default walk speed in x direction
                break;
            case MovementDirection.VERTICAL:
                rb.gravityScale = 0; // creatures are not affected by gravity
                // prevent all rotation, but particularly into the Z plane (i.e. around the Y axis)
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                walkSpeed = GameConstants.OBSTACLE_WALK_SPEED_Y; // default walk speed in y direction
                break;
            case MovementDirection.CIRCLING:
                rotator = GetComponent<Rotater2D>();
                if (rotator != null)
                {
                    rotator.AngularSpeed = GameConstants.OBSTACLE_WALK_SPEED_X; // default rotation speed
                    rotator.CentreOfRotation = new Vector3(movementConstraints.x, movementConstraints.y, 0);
                    rotator.IsClockwise = false; // by default
                }
                break;
        }

        //define the animator attached to the player
        animator = this.GetComponent<Animator>();
        animator.speed = 0;
        changeState(STATE_IDLE);

        // creatures can only move after the maid has given the instructions
        // to the player (which forces the player to seek out the instructions)
        if (gameState.MaidGivenInstructions)
        {
            if (direction == MovementDirection.CIRCLING && rotator != null)
            {
                rotator.enabled = true;
            }
            else
            {
                changeDirection(GameConstants.FORWARD);
                moveObject(Time.deltaTime);
                changeState(STATE_WALK_FORWARDS);
            }
        }
    }

    /// <summary>
    /// Keep the creature moving in the indicated direction
    /// </summary>
    protected override void Update()
    {
        base.Update();

        // creatures can only move after the maid has given the instructions
        // to the player (which forces the player to seek out the instructions)
        if (gameState.MaidGivenInstructions)
        {
            if (direction == MovementDirection.HORIZONTAL)
            {
                float currentX = transform.position.x;

                if (log)
                    Debug.Log("update - check toggle (HORIZONTAL): " + this.isToggling + "," + this.currentDirection + "," + currentX + "," + GameConstants.OBSTACLE_X_TOLERANCE + "," + movementConstraints);

                // If the creature encounters the end of the allowed range,
                // we want it to turn around and head in the other direction.
                if (canToggle(this.isToggling, this.currentDirection, currentX, GameConstants.OBSTACLE_X_TOLERANCE, movementConstraints))
                {
                    isToggling = true;
                    toggleDirection();
                }
                else
                {
                    isToggling = false;
                }
            }
            else if (direction == MovementDirection.VERTICAL)
            {
                float currentY = transform.position.y;

                if (log)
                    Debug.Log("update - check toggle (VERTICAL): " + this.isToggling + "," + this.currentDirection + "," + currentY + "," + GameConstants.OBSTACLE_Y_TOLERANCE + "," + movementConstraints);

                // If the creature encounters the end of the allowed range,
                // we want it to turn around and head in the other direction.
                if (canToggle(this.isToggling, this.currentDirection, currentY, GameConstants.OBSTACLE_Y_TOLERANCE, movementConstraints))
                {
                    isToggling = true;
                    toggleDirection();
                }
                else
                {
                    isToggling = false;
                }
            }
            else if (direction == MovementDirection.CIRCLING)
            {
                isToggling = false;
            }
        }
    }

    private static bool canToggle(bool isToggling, string currentDirection, float current, float tolerance, Vector2 movementConstraints)
    {
        // We want to avoid the case where the object sits the edge of the movement range stuck in a toggling state.

        // can toggle if not already in process of toggling
        if (!isToggling)
        {
            // can toggle if not already moving in the "new" direction and current position is outside the set constraints (with a little tolerance).
            if (currentDirection == GameConstants.BACKWARD && current - tolerance < movementConstraints.x)
            {
                return true;
            }
            else if (currentDirection == GameConstants.FORWARD && current + tolerance > movementConstraints.y)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// The direction in which this creature moves
    /// </summary>
    /// <value>The direction.</value>
    public MovementDirection Direction
    {
        get { return this.direction; }
        set { setDirection(value); }
    }

    /// <summary>
    /// Constraints on movement in the indicated MovementDirection
    /// (e.g. Horizontal movement is constrained between two X values
    /// and Vertical movement is constrained between two Y values)
    /// </summary>
    /// <value>The constraints.</value>
    public Vector2 MovementConstraints
    {
        get { return this.movementConstraints; }
        set { this.movementConstraints = value; }
    }

    /// <summary>
    /// Fixed upates is used instead of Update to better
    /// handle the physics-based jump.
    /// </summary>
    void FixedUpdate()
    {
        if (gameState.MaidGivenInstructions)
        {
            if (currentDirection == GameConstants.FORWARD)
            {
                moveObject(Time.fixedDeltaTime);
            }
            else if (currentDirection == GameConstants.BACKWARD)
            {
                moveObject(Time.fixedDeltaTime);
            }
            else
            {
                changeState(STATE_IDLE);
            }
        }
    }

    /// <summary>
    /// Change the creature's animation state
    /// </summary>
    /// <param name="state">State.</param>
    void changeState(int state)
    {

        if (currentAnimationState == state)
            return;

        switch (state)
        {

            case STATE_WALK_FORWARDS:
                animator.speed = 1;
                animator.SetInteger("state", STATE_WALK_FORWARDS);
                break;

            case STATE_MOVE_BACKWARDS:
                animator.speed = 1;
                animator.SetInteger("state", STATE_MOVE_BACKWARDS);
                break;

            case STATE_IDLE:
                animator.speed = 0;
                break;
        }

        currentAnimationState = state;
    }

    /// <summary>
    /// Check if creature has collided with another object
    /// </summary>
    /// <param name="collision">collision</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collisionEnter(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        collisionStay(collision.gameObject);
    }

    /// <summary>
    /// Check if creature has collided with another object
    /// </summary>
    /// <param name="coll">Coll.</param>
    void OnCollisionEnter2D(Collision2D coll)
    {
        collisionEnter(coll.gameObject);
    }

    /// <summary>
    /// Check if creature is touching another object
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        collisionStay(collision.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collisionExit(collision.gameObject);
    }

    private void collisionEnter(GameObject collidedGameObject)
    {
        // collisions are only important if creature is allowed to toggle direction
        // this is to avoid the creature getting stuck on the edge of a collision
        // in a constant state of toggling
        if (toggleAllowed)
        {
            // if the creature hits a wall, platform or scenery obstacle, we want it to head 
            // in the other direction
            if (direction == MovementDirection.HORIZONTAL &&
                collidedGameObject.CompareTag(GameConstants.WALL))
            {
                if (log)
                    Debug.Log("hit wall - toggle direction");

                toggleAllowed = false;
                isToggling = true;
                toggleDirection();
            }
            else if (direction == MovementDirection.VERTICAL &&
                     collidedGameObject.CompareTag(GameConstants.PLATFORM))
            {
                if (log)
                    Debug.Log("hit platform - toggle direction");

                toggleAllowed = false;
                isToggling = true;
                toggleDirection();
            }
            else if (collidedGameObject.CompareTag(GameConstants.SCENERY_OBSTACLE))
            {
                if (log)
                    Debug.Log("hit scenery obstacle - toggle direction");

                toggleAllowed = false;
                isToggling = true;
                toggleDirection();
            }
            else
            {
                isToggling = false;
            }
        }
        else
        {
            isToggling = false;
        }
    }

    private void collisionStay(GameObject collidedGameObject)
    {
        // this handles the case where the movement range of the creature
        // is intended to be constrained by a platform or wall it is in
        // contact with (i.e. it patrols back-and-forth along a specific platform).

        if (direction == MovementDirection.HORIZONTAL &&
            collidedGameObject.CompareTag(GameConstants.PLATFORM))
        {
            // get the bounds of the platform
            Bounds collBounds =
                collidedGameObject.GetComponent<BoxCollider2D>().bounds;

            // gets the X extents of the platform
            float collMaxX = collBounds.max.x;
            float collMinX = collBounds.min.x;

            // get the current X position of the creature
            float currentX = transform.position.x;

            if (log)
                Debug.Log("collisionStay - check toggle (HORIZONTAL): " + this.isToggling + "," + this.currentDirection + "," + currentX + "," + GameConstants.OBSTACLE_X_TOLERANCE + "," + new Vector2(collMinX, collMaxX));

            // If the creature encounters the end of the platform (i.e, its 
            // current position is outside the extents of the platform),
            // we want it to turn around and head in the other direction.
            if (canToggle(this.isToggling, this.currentDirection, currentX, GameConstants.OBSTACLE_X_TOLERANCE, new Vector2(collMinX, collMaxX)))
            {
                isToggling = true;
                toggleDirection();
            }
            else
            {
                isToggling = false;
            }
        }
        else if (direction == MovementDirection.VERTICAL &&
                 collidedGameObject.CompareTag(GameConstants.WALL))
        {
            // get the bounds of the wall
            Bounds collBounds =
                collidedGameObject.GetComponent<BoxCollider2D>().bounds;

            // gets the Y extents of the platform
            float collMaxY = collBounds.max.y;
            float collMinY = collBounds.min.y;

            float currentY = transform.position.y;

            if (log)
                Debug.Log("collisionStay - check toggle (VERTICAL): " + this.isToggling + "," + this.currentDirection + "," + currentY + "," + GameConstants.OBSTACLE_Y_TOLERANCE + "," + new Vector2(collMinY, collMaxY));

            // If the creature encounters the end of the wall (i.e, its 
            // current position is outside the extents of the wall),
            // we want it to turn around and head in the other direction.
            if (canToggle(this.isToggling, this.currentDirection, currentY, GameConstants.OBSTACLE_Y_TOLERANCE, new Vector2(collMinY, collMaxY)))
            {
                isToggling = true;
                toggleDirection();
            }
            else
            {
                isToggling = false;
            }
        }
    }

    private void collisionExit(GameObject collidedGameObject)
    {
        if (!toggleAllowed)
        {
            if (direction == MovementDirection.HORIZONTAL &&
                collidedGameObject.CompareTag(GameConstants.WALL))
            {
                toggleAllowed = true;
            }
            else if (direction == MovementDirection.VERTICAL &&
                     collidedGameObject.CompareTag(GameConstants.PLATFORM))
            {
                toggleAllowed = true;
            }
            else if (collidedGameObject.CompareTag(GameConstants.SCENERY_OBSTACLE))
            {
                toggleAllowed = true;
            }
        }
    }


    /// <summary>
    /// Toggle the direction in which the object is moving
    /// </summary>
    void toggleDirection()
    {
        if (isToggling && direction != MovementDirection.CIRCLING)
        {
            if (log)
                Debug.Log("toggle");

            changeDirection(currentDirection == GameConstants.FORWARD ? GameConstants.BACKWARD : GameConstants.FORWARD);

            if (direction == MovementDirection.HORIZONTAL)
            {
                // this is a little confusing, however we always use Vector3.right here because the transform gets
                // flipped by 180 degrees around the Y axis in changeDirection() (so right becomes left!)
                forceDirection = Vector3.right;
            }
            else if (direction == MovementDirection.VERTICAL)
            {
                // in this case, flipping the transform around the Y axis has no affect on which way it up or down,
                // so we simply toggle between the two options.
                forceDirection = forceDirection == Vector3.down ? Vector3.up : Vector3.down;
            }
        }
    }

    /// <summary>
    /// Flip sprite for forward/backward movement
    /// </summary>
    /// <param name="newDirection">Direction.</param>
    void changeDirection(string newDirection)
    {
        // An alternative way to have done this would have created
        // two separate animations for the creature - one facing left
        // and one facing right - and flip between them when the
        // creature changes direction, however simply rotating the
        // transform is simpler (but can create some confusion since
        // this reverses what is left and right!).

        if (currentDirection != newDirection)
        {
            if (newDirection == GameConstants.FORWARD)
            {
                transform.Rotate(0, 180, 0);
                currentDirection = GameConstants.FORWARD;
            }
            else if (newDirection == GameConstants.BACKWARD)
            {
                transform.Rotate(0, -180, 0);
                currentDirection = GameConstants.BACKWARD;
            }
        }
    }

    private void moveObject(float deltaTime)
    {
        if (log)
            Debug.Log("apply translate: " + forceDirection +"*"+ walkSpeed + "*" + difficultyMultiplier + "*"+ speedMultiplier + "*" + deltaTime);


        if (direction != MovementDirection.CIRCLING)
            transform.Translate(forceDirection * walkSpeed * difficultyMultiplier * speedMultiplier * deltaTime);
    }
}