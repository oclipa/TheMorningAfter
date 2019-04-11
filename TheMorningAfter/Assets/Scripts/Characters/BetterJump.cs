using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This code is derived from the example should in the following
/// youtube video: https://www.youtube.com/watch?v=7KiK0Aqtmzc
/// Essentially, it provides a better feel to the jump mechanic but
/// making the downward motion more severe (it makes it feel less floaty).
/// </summary>
public class BetterJump : MonoBehaviour {

    public float fallMultiplier = 2.6f;

    Rigidbody2D rb;
    PlayerController pc;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        pc = GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (pc.CurrentSlopeNormalX != 0f)
            return;

        float gravityDirection = Mathf.Sign(Physics2D.gravity.y);

        if (gravityDirection > 0f && rb.velocity.y > 0f)
        {
            modifyJump(gravityDirection);
        }
        else if (gravityDirection < 0f && rb.velocity.y < 0f)
        {
            modifyJump(-1 * gravityDirection);
        }
    }

    private void modifyJump(float jumpDirection)
    {
        rb.velocity += jumpDirection * Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
    }
}
