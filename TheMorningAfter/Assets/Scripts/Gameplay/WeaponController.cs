using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the weapon
/// 
/// When the weapon fires, a line is drawn between the barrel of the gun
/// and the target.
/// </summary>
public class WeaponController : MonoBehaviour {

    private GameObject laser; // the source
    private GameObject laserTarget; // the target
    private LineRenderer lineRenderer; // the renderer for the line
    private bool isFiring; // is the weapon in the process of firing?

    private GameObject player;
    private PlayerController playerController;
    private string prevDirection;

    private Timer timer; // controls the length of time the laser beam is displayed for

    private void Awake()
    {
        timer = gameObject.AddComponent<Timer>();
        timer.Duration = 0.2f; // laser beam is displayed for this time (seconds)
    }

    // Use this for initialization
    void Start() {

        // weapon consists of a source and a target, connected by a line

        laser = this.transform.GetChild(0).gameObject; // source
        lineRenderer = laser.GetComponent<LineRenderer>(); // line to between source and target
        laserTarget = laser.transform.GetChild(1).gameObject; // target

        player = GameObject.FindGameObjectWithTag(GameConstants.PLAYER);
        if (player != null)
            playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // make sure laser is pointing in same direction as player
        SetDirection();
    }

    private void FixedUpdate()
    {
        // when time finishes, hide the laser beam
        if (timer.Finished && lineRenderer.enabled)
        {
            lineRenderer.enabled = false;
            laser.SetActive(false);

            this.isFiring = false;
        }
    }

    private void SetDirection()
    {
        // in a similar manner to the playercontroller, the direction of the weapon
        // is controlled by rotating the transform.

        if (playerController != null)
        {
            PlayerControlBehavior playerControlBehavior = playerController.PlayerControlBehavior;

            float lookRight = 0f;
            float lookLeft = 180f;

            switch (playerControlBehavior)
            {
                case PlayerControlBehavior.NORMAL:
                    lookRight = 0f;
                    lookLeft = 180f;
                    break;
                case PlayerControlBehavior.REVERSED:
                    lookRight = 180f;
                    lookLeft = 0f;
                    break;
            }

            // if player direction has changed
            if (playerController.CurrentDirection != prevDirection)
            {
                // rotate the weapon transform accordingly
                if (playerController.CurrentDirection == GameConstants.RIGHT)
                {
                    if (transform.rotation.y != lookRight)
                    {
                        transform.Rotate(0, 180, 0);
                        prevDirection = playerController.CurrentDirection;
                    }
                }
                else if (playerController.CurrentDirection == GameConstants.LEFT)
                {
                    if (transform.rotation.y != lookLeft)
                    {
                        transform.Rotate(0, 180, 0);
                        prevDirection = playerController.CurrentDirection;
                    }
                }

            }
        }
    }

    /// <summary>
    /// Fires a single shot from the weapon
    /// </summary>
    public void FireOneShot()
    {
        if (!this.isFiring)
        {
            this.isFiring = true;

            // pew pew
            AudioManager.Instance.PlayOneShot(AudioClipName.Laser, 1.0f);

            // raycast horizontally away from the weapon, in the direction the weapon is facing.
            var direction = transform.TransformDirection(Vector3.right);

            // ray will be cast until it hits the first structural object (e.g. platform, wall etc.)

            // count the first 10 hits of the ray (assume we won't have more than 10 objects in a row)
            RaycastHit2D[] hits = new RaycastHit2D[10];

            // only interested in hits with creatures (layer = "Obstacle") and 
            // structural objects, such as platforms or walls (layer = "Structural") 
            ContactFilter2D contactFilter = new ContactFilter2D();
            LayerMask mask = LayerMask.GetMask(new string[] { "Obstacle", "Structural" });
            contactFilter.SetLayerMask(mask);
            contactFilter.useLayerMask = true; // enable the layer mask
            contactFilter.useTriggers = true; // include colliders that are triggers

            int hitCount = Physics2D.Raycast(transform.position, direction, contactFilter, hits, 20f);

            // if we hit objects
            if (hitCount > 0)
            {
                // iterate over each object
                for (int i = 0; i < hitCount; i++)
                {
                    RaycastHit2D hit = hits[i];

                    // If we hit a structural object, this is our target (where the laser beam stops)
                    // For the purposes of hilarity, the maid is treated as a structural object.
                    if (hit.transform.CompareTag(GameConstants.WALL) ||
                        hit.transform.CompareTag(GameConstants.PLATFORM) ||
                        hit.transform.CompareTag(GameConstants.DOOR) ||
                        hit.collider.CompareTag(GameConstants.MAID)) // here we test the specific collider because the maid has several
                    {
                        FireLaser(hit.transform.position.x);

                        // maid swears when hit by the laser
                        if (hit.collider.CompareTag(GameConstants.MAID))
                        {
                            hit.collider.SendMessageUpwards("Swear");
                        }

                        break;
                    }
                    else if (hit.transform.CompareTag(GameConstants.OBSTACLE))
                    {
                        // creatures exploded when hit by the laser
                        hit.collider.SendMessageUpwards("Explode");
                    }
                }
            }
        }
    }

    private void FireLaser(float targetX)
    {
        // laser beam (line) is displayed between the source (laser) and the target.
        laserTarget.transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
        laser.SetActive(true);
        lineRenderer.enabled = true; // display the beam
        timer.Run(); // controls how long the beam will be displayed for
    }
}