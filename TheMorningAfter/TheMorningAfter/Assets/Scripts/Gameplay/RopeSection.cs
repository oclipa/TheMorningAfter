
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSection : MonoBehaviour
{
    private RopeAnchor ropeAnchor;
    private bool hasPlayer;

    // Use this for initialization
    void Start()
    {
        this.ropeAnchor = this.gameObject.transform.parent.gameObject.GetComponent<RopeAnchor>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            hasPlayer = true;
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
