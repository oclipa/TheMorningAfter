using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the Ladder object
/// </summary>
public class Ladder : MonoBehaviour
{
    // Ladder knows about player since ladder 
    // controls movement up and down itself.
    GameObject playerOBJ;

    // We need to ensure that the player can pass through any
    // platforms that intersect the ladder.
    List<GameObject> intersectingPlatforms = new List<GameObject>();

    private void Start()
    {
        // identify platforms that intersect ladder

        // get bounds of ladder
        Bounds ladderBounds = GetComponent<Collider2D>().bounds;

        // get all platforms and check if they intersect with ladder bounds
        GameObject[] platforms = GameObject.FindGameObjectsWithTag(GameConstants.PLATFORM);
        for (int i = 0; i < platforms.Length; i++)
        {
            GameObject platform = platforms[i];
            Bounds platformBounds = platform.GetComponent<Collider2D>().bounds;
            if (ladderBounds.Intersects(platformBounds))
                intersectingPlatforms.Add(platform);
        }
    }

    /// <summary>
    /// When the player enters a ladder, the ladder then controls
    /// the up/down movement.
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            // need to stop the player moving unnecessarily
            playerOBJ = collision.gameObject;
            playerOBJ.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            playerOBJ.GetComponent<Rigidbody2D>().gravityScale = 0;

            // ensure that the player ignores any intersecting platforms
            foreach (GameObject platform in intersectingPlatforms)
            {
                if (platform != null)
                {
                    Physics2D.IgnoreCollision(playerOBJ.GetComponent<Collider2D>(),
                                              platform.GetComponent<Collider2D>());

                }
            }
        }
    }

    /// <summary>
    /// When the player exits the ladder, the ladder
    /// must no longer control the up/down movement
    /// </summary>
    /// <param name="collision">Collision.</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            // Ensure that player can interact with all platforms
            foreach (GameObject platform in intersectingPlatforms)
                Physics2D.IgnoreCollision(playerOBJ.GetComponent<Collider2D>(), platform.GetComponent<Collider2D>(), false);

            // ensure player is affected by gravity again
            playerOBJ.GetComponent<Rigidbody2D>().gravityScale = 1;

            playerOBJ = null;
        }
    }

    //void FixedUpdate()
    //{
    //    // if ladder controls movement
    //    if (canClimb && playerOBJ != null)
    //    {
    //        if (Input.GetAxisRaw(GameConstants.VERTICAL) > 0)
    //        {
    //            playerOBJ.transform.Translate(Vector3.up * Time.deltaTime * speed);
    //        }
    //        if (Input.GetAxisRaw(GameConstants.VERTICAL) < 0)
    //        {
    //            playerOBJ.transform.Translate(Vector3.down * Time.deltaTime * speed);
    //        }
    //    }
    //}
}