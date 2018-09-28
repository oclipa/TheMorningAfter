using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour {

    Text timeText;
    DateTime time;

	// Use this for initialization
	void Start () {
        timeText = gameObject.GetComponent<Text>();
        time = new DateTime(2018, 1, 1, 7, 30, 0);
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
    }
}
