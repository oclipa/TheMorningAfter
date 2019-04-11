using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle ramp behaviour.
/// There are two basic ramp behaviours:
/// 1) The player needs to help to move up and down the ramp (to overcome gravity).
/// 2) THe player can pass thru the ramp under certain conditions.
/// </summary>
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
        // get the slope of the ramp (which is defined in the room config file)
        float slopeRadians = transform.rotation.z * Mathf.Deg2Rad;
        slopeNormalX = -1 * Mathf.Sin(slopeRadians);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            // If a player collides with the back of a ramp,
            // they can pass through the ramp.

            // get the contact points between the ramp and the player
            ContactPoint2D[] contacts = new ContactPoint2D[1];
            int numOfContacts = collision.GetContacts(contacts);

            // Is the player is solid and in contact with the ramp?
            // (if player is not solid, they can pass through the ramp anyway)
            if (collision.gameObject.GetComponent<PlayerController>().IsSolid && numOfContacts > 0)
            {
                ContactPoint2D contact = contacts[0];
                // Is the contact point above the player?  If so, this 
                // indicates that the player is hitting the back of the ramp.
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.5)
                {
                    // so player can pass thru the ramp (which is achieved by
                    // making the ramp collider a trigger)
                    GetComponent<Collider2D>().isTrigger = true;
                }
            }
        }
        else if (collision.gameObject.CompareTag(GameConstants.OBSTACLE))
        {
            // creatures are unaffected by ramps (they always pass thru)
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(),
                                      GetComponent<Collider2D>());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // while player remains in contact with the ramp and is not solid,
        // also ensure that the ramp is not solid (so the player can pass thru)
        PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
        if (pc != null && !pc.IsSolid)
        {
            GetComponent<Collider2D>().isTrigger = true;
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
        else if (collision.gameObject.CompareTag(GameConstants.OBSTACLE))
        {
            // When creature leaves the ramp, re-enable collisions between
            // the ramp and the creature (why? this can probably be removed)
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(),
                                      GetComponent<Collider2D>(), false);
        }
    }

    /// <summary>
    /// The X component of the normal to the slope
    /// </summary>
    /// <value>The slope normal x.</value>
    public float SlopeNormalX
    {
        get { return this.slopeNormalX; }
    }
}