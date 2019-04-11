using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Wall behaviour(!)
/// 
/// Not entirely sure there is any need for this...
/// </summary>
public class Wall : MonoBehaviour {

    bool intersectsLadder;

    public bool IntersectsLadder
    {
        get
        {
            return this.intersectsLadder;
        }
        set
        {
            this.intersectsLadder = value;
            GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }
}
