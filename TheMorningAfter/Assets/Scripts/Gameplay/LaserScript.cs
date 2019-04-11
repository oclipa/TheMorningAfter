using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Draws a line between the start and end points of the laser
/// </summary>
public class LaserScript : MonoBehaviour {

    public Transform startPoint;
    public Transform endPoint;
    LineRenderer laserLine; 

	// Use this for initialization
	void Start () {
        laserLine = GetComponent<LineRenderer>();
        laserLine.startWidth = 0.08f;
        laserLine.endWidth = 0.08f;
	}
	
	// Update is called once per frame
	void Update () {
        laserLine.SetPosition(0, startPoint.position);
        laserLine.SetPosition(1, endPoint.position);
    }
}
