using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles scenery obstacles.
/// 
/// Scenery obstacles are decorative objects that the player cannot pass through.
/// Some scenery obstacles can be pushed by the player; some cannot.
/// </summary>
public class SceneryObstacleController : ObstacleController, IGameOverInvoker
{
    // strictly speaking we should probably be using a BedController
    // but ho hum.
    bool isBed;

    // in the special case of the bed, the game is over once the player reaches the bed.
    GameOverEvent gameOverEvent;

    // Scenery obstacles make a sound when moved (e.g. scraping/dragging sound).
    // We are using a separate AudioSource (rather than the AudioManager) because we
    // want the audio for the moving obstacles to play alongside the room music.
    AudioSource audioSource;

    // sound is only played if the object has moved a certain distance in a certain time
    Vector3 lastPosition;

    // sound is only played if the player is touching the object
    bool isTouchingPlayer;

    public SceneryObstacleController()
        : base()
    {
        // scenery objects do not move on their own
        this.direction = MovementDirection.STATIC;
    }

    protected override void Start()
    {
        base.Start();

        this.gameOverEvent = new GameOverEvent();
        EventManager.AddGameOverInvoker(this);
        this.gameState = Camera.main.GetComponent<GameState>();

        this.audioSource = GetComponent<AudioSource>();

        this.lastPosition = transform.position;
    }

    protected void FixedUpdate()
    {
        // only play sound if the player is touching the object and the object
        // moves a significant amount.
        if (this.isTouchingPlayer)
        {
            bool hasMoved = HasMoved();

            if (hasMoved && !this.audioSource.isPlaying && !this.gameState.DisableSoundEffects)
            {
                this.audioSource.Play();
            }
            else if (!hasMoved && this.audioSource.isPlaying)
            {
                this.audioSource.Stop();
            }
        }
    }

    private bool HasMoved()
    {
        // movement is defined as having moved a "significant" amount since the last update.
        // this avoid small movements creating too many sound effects.

        Vector3 currentPosition = transform.position;

        Vector3 velocity = (currentPosition - lastPosition) / Time.fixedDeltaTime;

        if (velocity.magnitude > 0.01f)
        {
            lastPosition = currentPosition;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Is this the bed?
    /// </summary>
    /// <value><c>true</c> if is bed; otherwise, <c>false</c>.</value>
    public bool IsBed
    {
        get { return this.isBed; }
        set { this.isBed = value;  }
    }

    /// <summary>
    /// Adds the games over listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    public void AddGameOverListener(UnityAction<int> listener)
    {
        gameOverEvent.AddListener(listener);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if player reaches the bed, the game is over.
        if (isBed && collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            this.gameState.BedReached = true;
            int itemsCollected = this.gameState.ItemsCollected;
            this.gameOverEvent.Invoke(itemsCollected);
        }
        // else if collision is with another scenery obstacle
        else if (collision.gameObject.CompareTag(GameConstants.SCENERY_OBSTACLE))
        {
            if (rb.bodyType == RigidbodyType2D.Static)
            {
                // If a scenery obstacle is pushed up against an immovable obstacle, we want to make 
                // sure there is enough room for the player to separate the obstacles.
                // To do this, we make the obstacles bounce apart. which forces them to
                // be fair enough apart for the player to get in between them.

                // get the bounds of the object that collided with this one
                BoxCollider2D boxColl = collision.gameObject.GetComponent<BoxCollider2D>();
                if (boxColl != null)
                {
                    // boing
                    AudioManager.Instance.PlayOneShot(AudioClipName.Boing);

                    Bounds collBounds = boxColl.bounds;

                    // get the extents of the collided object
                    //float collMaxX = collBounds.max.x;
                    float collMinX = collBounds.min.x;

                    // get the extents of this object
                    //float minCurrentX = transform.position.x;
                    float maxCurrentX = transform.position.x;

                    // initially assume impact was on left side of this object
                    Vector2 dir = Vector2.left;

                    // if impact was on right side (i.e. the right side of this object 
                    // is to the left of the left side of the colliding object), we
                    // want to bounce the collided object to the right
                    if (maxCurrentX < collMinX) // impact is on the right side
                        dir = Vector2.right;

                    // bounce apart
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(dir * GameConstants.OBSTACLE_BOUNCE_FORCE, ForceMode2D.Force);
                }
            }
        }
        // else if touching player
        else if (collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            this.isTouchingPlayer = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // if no longer touching player
        if (collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            this.isTouchingPlayer = false;
        }
    }
}
