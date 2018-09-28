using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour
{
    // The slope of the ramp indicates how much force 
    // is required for the player to move along it
    private float slopeNormalX;

    /// <summary>
    /// Start this instance.
    /// </summary>
    private void Start()
    {
        slopeNormalX = -1 * Mathf.Sin(transform.rotation.z * Mathf.Deg2Rad);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            // If a player collides with a ramp while jumping, they
            // can pass through the ramp.
            // Also, if a player collides with the back of a ramp,
            // they can pass through the ramp.
            ContactPoint2D[] contacts = new ContactPoint2D[1];
            int numOfContacts = collision.GetContacts(contacts);

            // Is the player jumping?
            if (collision.gameObject.
                    GetComponent<PlayerController>().IsJumping)
            {
                GetComponent<Collider2D>().isTrigger = true;
            }
            else if (numOfContacts > 0)
            {
                ContactPoint2D contact = contacts[0];
                // Is the contact point above the player?  If so, this 
                // indicates that the player is hitting the back of the ramp.
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.5)
                {
                    GetComponent<Collider2D>().isTrigger = true;
                }
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            // When the player leaves the ramp (in any direction)
            // we want to make sure that the default assumption is
            // that the player can interact with the ramp if they return.
            GetComponent<Collider2D>().isTrigger = false;
        }
    }

    /// <summary>
    /// Provide properties that allow the player to update their position
    ///  on the ramp as they move up it (allowing for the slope of the ramp)
    /// </summary>
    public void HelpUpSlope(ref Vector2 playerVelocity, 
                                ref Vector3 playerPosition)
    {
        float playerVelocityX = playerVelocity.x;
        float playerVelocityY = playerVelocity.y;

        // Apply the opposite force against the slope force 
        playerVelocity = new Vector2(playerVelocityX
                                     - (slopeNormalX * GameConstants.SLOPE_FRICTION), 
                                            playerVelocityY);

        //Move Player up or down to compensate for the slope below them
        playerPosition.y += -slopeNormalX 
                                * Mathf.Abs(playerVelocityX) 
                                * Time.fixedDeltaTime 
                                * (playerVelocityX - slopeNormalX > 0 ? 1 : -1);
    }
}