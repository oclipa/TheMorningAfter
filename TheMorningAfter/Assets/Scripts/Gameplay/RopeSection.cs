
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This has not been fully implemented or tested.
/// 
/// Models a section of a rope (a rope is defined as multiple solid sections, 
/// linked by joints, which are ultimately attached to an anchor)
/// </summary>
public class RopeSection : MonoBehaviour
{
    // the anchor point of the rope
    private RopeAnchor ropeAnchor;

    // is the player attached to this section of the rope?
    private bool hasPlayer;

    // Use this for initialization
    void Start()
    {
        // null if the parent of this RopeSection is another RopeSection.
        this.ropeAnchor = this.gameObject.transform.parent.gameObject.GetComponent<RopeAnchor>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if player collides with this section, attach the player to this section.
        if (collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            hasPlayer = true;

            // if player is not already attached to another section, attach it to this section.
            if (!ropeAnchor.IsPlayerOnRope)
                ropeAnchor.SetPlayerPosition(collision.gameObject, this.transform.GetSiblingIndex());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            this.hasPlayer = false;
        }
    }

    public bool HasPlayer
    {
        get { return this.hasPlayer; }
    }
}
