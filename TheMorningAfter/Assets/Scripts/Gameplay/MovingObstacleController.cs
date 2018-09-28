using UnityEngine;
using System.Collections;

/// <summary>
/// Handles moving obstacles
/// </summary>
public class MovingObstacleController : ObstacleController
{
    private float walkSpeed;

    // By default, the obstacle can move as far as the end of each platform,
    // or as far as the distance betweeen two platforms.
    // This property enables finer control of the movement range.
    private Vector2 movementConstraints = 
                new Vector2(float.MinValue, float.MaxValue);

    // Animates the obstacle
    private Animator animator;

    //animation states - the values in the animator conditions
    private const int STATE_MOVE_BACKWARDS = 0;
    private const int STATE_WALK_FORWARDS = 1;
    private const int STATE_IDLE = 2;

    // obstacle is initially facing "forward"
    private string _currentDirection = GameConstants.FORWARD;

    // obstacle is initially idle
    private int _currentAnimationState = STATE_IDLE;

    // the direction in which force is applied to get the
    // obstacle to move
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
    /// Set the direction in which the obstacle can move
    /// </summary>
    /// <param name="direction">Direction.</param>
    private void setDirection(MovementDirection direction)
    {
        this.direction = direction;

        switch (direction)
        {
            case MovementDirection.HORIZONTAL:
                forceDirection = Vector3.right;
                break;
            case MovementDirection.VERTICAL:
                forceDirection = Vector3.down;
                break;
        }
    }

    /// <summary>
    /// Start the obstacle moving
    /// </summary>
    protected override void Start()
    {
        base.Start();

        switch (direction)
        {
            case MovementDirection.HORIZONTAL:
                rb.gravityScale = 0;
                walkSpeed = GameConstants.OBSTACLE_WALK_SPEED_X;
                break;
            case MovementDirection.VERTICAL:
                rb.gravityScale = 0;
                walkSpeed = GameConstants.OBSTACLE_WALK_SPEED_Y;
                break;
        }

        //define the animator attached to the player
        animator = this.GetComponent<Animator>();
        animator.speed = 0;
        changeState(STATE_IDLE);

        changeDirection(GameConstants.FORWARD);
        transform.Translate(forceDirection * walkSpeed * Time.deltaTime);
        changeState(STATE_WALK_FORWARDS);
    }

    /// <summary>
    /// Keep the obstacle moving in the indicated direction
    /// </summary>
    protected override void Update()
    {
        base.Update();

        if (direction == MovementDirection.HORIZONTAL)
        {
            float currentX = transform.position.x;

            // If the obstacle encounters the end of the platform,
            // we want it to turn around and head in the other direction.
            if (currentX - GameConstants.OBSTACLE_X_TOLERANCE <= movementConstraints.x || currentX + GameConstants.OBSTACLE_X_TOLERANCE >= movementConstraints.y)
            {
                toggleDirection();
            }
        } 
        else if (direction == MovementDirection.VERTICAL)
        {
            float currentY = transform.position.y;

            // If the obstacle encounters the end of the wall,
            // we want it to turn around and head in the other direction.
            if (currentY - GameConstants.OBSTACLE_Y_TOLERANCE <= movementConstraints.x || currentY + GameConstants.OBSTACLE_Y_TOLERANCE >= movementConstraints.y)
            {
                toggleDirection();
            }
        }
    }

    /// <summary>
    /// The direction in which this obstacle moves
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
        //Check for keyboard input
        if (_currentDirection == GameConstants.FORWARD)
        {
            transform.Translate(forceDirection * walkSpeed * Time.deltaTime);
        }
        else if (_currentDirection == GameConstants.BACKWARD)
        {
            transform.Translate(forceDirection * walkSpeed * Time.deltaTime);
        }
        else
        {
            changeState(STATE_IDLE);
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

        _currentAnimationState = state;
    }

    /// <summary>
    /// Check if obstacle has collided with another object
    /// </summary>
    /// <param name="coll">Coll.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collisionEnter(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        collisionStay(collision.gameObject);
    }

    /// <summary>
    /// Check if obstacle has collided with another object
    /// </summary>
    /// <param name="coll">Coll.</param>
    void OnCollisionEnter2D(Collision2D coll)
    {
        collisionEnter(coll.gameObject);
    }

    /// <summary>
    /// Check if obstacle is touching another object
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        collisionStay(collision.gameObject);
    }

    private void collisionEnter(GameObject collidedGameObject)
    {
        // if the obstacle hits a wall, we want it to head 
        // in the other direction
        if (direction == MovementDirection.HORIZONTAL &&
            collidedGameObject.CompareTag(GameConstants.WALL))
        {
            toggleDirection();
        }
        else if (direction == MovementDirection.VERTICAL &&
                 collidedGameObject.CompareTag(GameConstants.PLATFORM))
        {
            toggleDirection();
        }
    }

    private void collisionStay(GameObject collidedGameObject)
    {
        if (direction == MovementDirection.HORIZONTAL &&
            collidedGameObject.CompareTag(GameConstants.PLATFORM))
        {
            Bounds collBounds =
                collidedGameObject.GetComponent<BoxCollider2D>().bounds;

            float collMaxX = collBounds.max.x;
            float collMinX = collBounds.min.x;

            float currentX = transform.position.x;

            // If the obstacle encounters the end of the platform,
            // we want it to turn around and head in the other direction.
            if (currentX - GameConstants.OBSTACLE_X_TOLERANCE <= collMinX || 
                currentX + GameConstants.OBSTACLE_X_TOLERANCE >= collMaxX)
            {
                toggleDirection();
            }
        }
        else if (direction == MovementDirection.VERTICAL &&
                 collidedGameObject.CompareTag(GameConstants.WALL))
        {
            Bounds collBounds =
                collidedGameObject.GetComponent<BoxCollider2D>().bounds;

            float collMaxY = collBounds.max.y;
            float collMinY = collBounds.min.y;

            float currentY = transform.position.y;

            // If the obstacle encounters the end of the wall,
            // we want it to turn around and head in the other direction.
            if (currentY - GameConstants.OBSTACLE_Y_TOLERANCE <= collMinY || 
                currentY + GameConstants.OBSTACLE_Y_TOLERANCE >= collMaxY)
            {
                toggleDirection();
            }
        }
    }

    /// <summary>
    /// Toggle the direction in which the object is moving
    /// </summary>
    void toggleDirection()
    {
        changeDirection(_currentDirection == GameConstants.FORWARD ? GameConstants.BACKWARD : GameConstants.FORWARD);

        if (direction == MovementDirection.HORIZONTAL)
        {
            forceDirection = Vector3.right;
            //forceDirection = forceDirection == Vector3.right ? Vector3.left : Vector3.right;
        }
        else if (direction == MovementDirection.VERTICAL)
        {
            forceDirection = forceDirection == Vector3.down ? Vector3.up : Vector3.down;
        }

        transform.Translate(forceDirection * walkSpeed * Time.deltaTime);
        changeState(_currentAnimationState == STATE_WALK_FORWARDS ? 
                            STATE_MOVE_BACKWARDS : STATE_WALK_FORWARDS);
    }

    /// <summary>
    /// Flip sprite for forward/backward movement
    /// </summary>
    /// <param name="direction">Direction.</param>
    void changeDirection(string direction)
    {
        if (_currentDirection != direction)
        {
            if (direction == GameConstants.FORWARD)
            {
                transform.Rotate(0, 180, 0);
                _currentDirection = GameConstants.FORWARD;
            }
            else if (direction == GameConstants.BACKWARD)
            {
                transform.Rotate(0, -180, 0);
                _currentDirection = GameConstants.BACKWARD;
            }
        }
    }
}