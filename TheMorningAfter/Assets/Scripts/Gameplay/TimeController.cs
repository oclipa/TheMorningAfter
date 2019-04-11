using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the in-game time
/// 
/// The original intention was that there would be time-based easter eggs,
/// but this was never implemented.
/// </summary>
public class TimeController : MonoBehaviour {

    Text timeText;
    DateTime time = GameConstants.DEFAULT_TIME;

	// Use this for initialization
	void Start () {
        timeText = gameObject.GetComponent<Text>();
	}

    private void FixedUpdate()
    {
        time = time.AddSeconds(Time.fixedDeltaTime * 5); // 5 x speed up
        timeText.text = time.ToString("hh:mmtt", CultureInfo.InvariantCulture).ToLower();
    }

    /// <summary>
    /// Gets the current in-game time.  We might use this to trigger some time-dependent effects...
    /// </summary>
    /// <value>The current time.</value>
    public DateTime CurrentTime
    {
        get { return this.time; }
        set { this.time = value; }
    }
}
