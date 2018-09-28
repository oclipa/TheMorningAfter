
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAnchor : MonoBehaviour {

    bool canClimb = false;
    private int currentPlayerPosition = -1;
    private GameObject player;
    List<RopeSection> ropeSections = new List<RopeSection>();

    // Use this for initialization
    void Start () {
        int childCount = this.transform.childCount;

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
            bool hasPlayer = false;
            foreach (RopeSection ropeSection in ropeSections)
            {
                if (ropeSection.HasPlayer)
                    hasPlayer = true;
            }

            if (!hasPlayer)
            {
                if (player != null)
                    this.player.GetComponent<Rigidbody2D>().gravityScale = 1;
                this.player = null;
                this.currentPlayerPosition = -1;
                canClimb = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (canClimb)
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                int newPlayerPosition = this.currentPlayerPosition - 1;
                if (newPlayerPosition > -1)
                {
                    SetPlayerPosition(player, newPlayerPosition);
                }
            }
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                int newPlayerPosition = this.currentPlayerPosition + 1;
                if (newPlayerPosition < this.transform.childCount)
                {
                    SetPlayerPosition(player, newPlayerPosition);
                }
            }

            Transform child = this.transform.GetChild(currentPlayerPosition);

            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                child.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 50f, ForceMode2D.Force);

                if (Input.GetKey("space"))
                {
                    SetPlayerPosition(player, -1);
                    child.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 10f, ForceMode2D.Impulse);
                }
                else
                {
                    this.player.transform.position = child.position;
                }
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                child.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 50f, ForceMode2D.Force);

                if (Input.GetKey("space"))
                {
                    SetPlayerPosition(player, -1);
                    child.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 10f, ForceMode2D.Impulse);
                }
                else
                {
                    this.player.transform.position = child.position;
                }
            }


        }
    }

    public void SetPlayerPosition(GameObject player, int childIndex)
    {
        if (childIndex == -1)
        {
            this.player.GetComponent<Rigidbody2D>().gravityScale = 1;
            this.player.transform.parent = null;
            this.player = null;
            this.currentPlayerPosition = -1;
            canClimb = false;
        }
        else if (player != null)
        {
            this.player = player;
            this.currentPlayerPosition = childIndex;

            Transform child = this.transform.GetChild(this.currentPlayerPosition);
            this.player.transform.parent = child.transform;
            this.player.transform.position = child.position;
            Rigidbody2D playerRB = player.GetComponent<Rigidbody2D>();
            playerRB.velocity = Vector2.zero;
            playerRB.gravityScale = 0;

            canClimb = true;
        }
    }

    public bool IsPlayerOnRope
    {
        get { return this.player != null; }
    }
}
