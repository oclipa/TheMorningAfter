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
    List<GameObject> intersectingSurfaces = new List<GameObject>();

    private void Start()
    {
        // ladders allow the player to pass through a platform or wall.

        // identify platforms and walls that intersect ladder

        // get bounds of ladder
        Bounds ladderBounds = GetComponent<Collider2D>().bounds;

        // get all platforms and check if they intersect with ladder bounds
        GameObject[] platforms = GameObject.FindGameObjectsWithTag(GameConstants.PLATFORM);
        for (int i = 0; i < platforms.Length; i++)
        {
            GameObject platform = platforms[i];
            Bounds platformBounds = platform.GetComponent<Collider2D>().bounds;
            if (ladderBounds.Intersects(platformBounds))
                intersectingSurfaces.Add(platform);
        }

        // also check intersecting walls
        GameObject[] walls = GameObject.FindGameObjectsWithTag(GameConstants.WALL);
        for (int i = 0; i < walls.Length; i++)
        {
            GameObject wall = walls[i];
            Bounds wallBounds = wall.GetComponent<Collider2D>().bounds;
            if (ladderBounds.Intersects(wallBounds))
            {
                intersectingSurfaces.Add(wall);

                // If wall intersects a ladder, turn the wall collider
                // into a trigger (to allow pass-thru).  This is handled
                // differently to platforms, because walls don't tend to 
                // have other objects resting on them, so they can always
                // be triggers (although there is a risk that the wall allows
                // the player to pass through into an area they shouldn't be in!).
                // To be honest, I am not entirely sure of the point of this...
                wall.GetComponent<Wall>().IntersectsLadder = true;
            }
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
            // need to stop the player moving unnecessarily.
            // This is primarily to stop the player jumping
            // through the ladder; they will always stop on the ladder.
            // This also stops the player falling down the ladder
            // due to gravity.
            playerOBJ = collision.gameObject;
            playerOBJ.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            playerOBJ.GetComponent<Rigidbody2D>().gravityScale = 0;

            // ensure that the player ignores any intersecting surfaces
            foreach (GameObject platform in intersectingSurfaces)
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
            // Ensure that player can interact with all surfaces
            foreach (GameObject platform in intersectingSurfaces)
            {
                if (platform != null)
                {
                    Physics2D.IgnoreCollision(playerOBJ.GetComponent<Collider2D>(), platform.GetComponent<Collider2D>(), false);
                }
            }

            // ensure player is affected by gravity again
            playerOBJ.GetComponent<Rigidbody2D>().gravityScale = 1;

            playerOBJ = null;
        }
    }
}