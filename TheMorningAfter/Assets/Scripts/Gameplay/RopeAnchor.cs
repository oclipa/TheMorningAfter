
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This has not been fully implemented or tested.
/// 
/// In theory this models the anchor point of a rope
/// to another object.
/// </summary>
public class RopeAnchor : MonoBehaviour {

    bool canClimb = false; // indicates it player can climb rope
    private int currentPlayerPosition = -1; // indicates the current RopeSection that has the player attached 
    private GameObject player; // the player
    List<RopeSection> ropeSections = new List<RopeSection>(); // all of the RopeSections that make up the rope

    // Use this for initialization
    void Start () {

        // a rope consists of multiple child RopeSections
        int childCount = this.transform.childCount;

        // each RopeSection is joined to the one above it by a HingeJoint2D,
        // and the top segment is joined to this anchor by another HingeJoint2D.
        for (int i = 0; i < childCount; i++)
        {
            Transform t = this.transform.GetChild(i);
            HingeJoint2D hinge = t.gameObject.GetComponent<HingeJoint2D>();
            hinge.connectedBody = i == 0 ? 
                this.gameObject.GetComponent<Rigidbody2D>() : 
                this.transform.GetChild(i - 1).GetComponent<Rigidbody2D>();

            ropeSections.Add(t.GetComponent<RopeSection>());
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (canClimb)
        {
            // check if any of the RopeSections has the player
            // attached to it
            bool hasPlayer = false;
            foreach (RopeSection ropeSection in ropeSections)
            {
                if (ropeSection.HasPlayer)
                {
                    hasPlayer = true;
                    break;
                }
            }

            // if player is not attached to any RopeSection, ensure that
            // gravity is re-enabled for player.
            if (!hasPlayer)
            {
                if (player != null)
                    this.player.GetComponent<Rigidbody2D>().gravityScale = 1;
                this.player = null;
                this.currentPlayerPosition = -1; // player not on rope
                canClimb = false; // player cannot climb rope
            }
        }
    }

    void FixedUpdate()
    {
        // if player can climb rope
        if (canClimb)
        {
            ClimbRope();

            SwingRope();
        }
    }

    private void ClimbRope()
    {
        // if moving upwards
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            // move player up one section
            int newPlayerPosition = this.currentPlayerPosition - 1;

            // if player is on still rope, set the current position
            if (newPlayerPosition > -1)
            {
                SetPlayerPosition(player, newPlayerPosition);
            }
        }

        // if moving downwards
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            // move player down one section
            int newPlayerPosition = this.currentPlayerPosition + 1;

            // if player is on still rope, set the current position
            if (newPlayerPosition < this.transform.childCount)
            {
                SetPlayerPosition(player, newPlayerPosition);
            }
        }
    }

    private void SwingRope()
    {
        // get the RopeSection that holds the player
        Transform child = this.transform.GetChild(currentPlayerPosition);

        // the following code enables the rope to swing from side-to-side
        // while carrying the player with it

        // if moving right
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            // add force to RopeSection moving it right
            child.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 50f, ForceMode2D.Force);

            // if player jumps
            if (Input.GetKey("space"))
            {
                // player is no longer on rope
                SetPlayerPosition(player, -1);

                // add slight impulse to rope - why? Not sure if this is needed.
                child.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 10f, ForceMode2D.Impulse);
            }
            else
            {
                // set the position of the player to match the position of the rope
                this.player.transform.position = child.position;
            }
        }
        // if moving left
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            // add force to RopeSection moving it left
            child.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 50f, ForceMode2D.Force);

            // if player jumps
            if (Input.GetKey("space"))
            {
                // player is no longer on rope
                SetPlayerPosition(player, -1);

                // add slight impulse to rope - why? Not sure if this is needed.
                child.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 10f, ForceMode2D.Impulse);
            }
            else
            {
                // set the position of the player to match the position of the rope
                this.player.transform.position = child.position;
            }
        }
    }

    /// <summary>
    /// Sets the position of the player on the rope (indicated by the
    /// RopeSection the player is attached to)
    /// </summary>
    /// <param name="player">Player.</param>
    /// <param name="childIndex">Child index.</param>
    public void SetPlayerPosition(GameObject player, int childIndex)
    {
        if (childIndex == -1) // not on rope
        {
            this.player.GetComponent<Rigidbody2D>().gravityScale = 1;
            this.player.transform.parent = null;
            this.player = null;
            this.currentPlayerPosition = -1;
            canClimb = false;
        }
        else if (player != null) // on rope
        {
            this.player = player;
            this.currentPlayerPosition = childIndex;

            // add player as child of RopeSection
            Transform child = this.transform.GetChild(this.currentPlayerPosition);
            this.player.transform.parent = child.transform;
            this.player.transform.position = child.position;
            Rigidbody2D playerRB = player.GetComponent<Rigidbody2D>();
            playerRB.velocity = Vector2.zero; // player has not independent velocity
            playerRB.gravityScale = 0; // player is not affected by gravity

            canClimb = true;
        }
    }

    public bool IsPlayerOnRope
    {
        get { return this.player != null; }
    }
}
