using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates an object around a point
/// </summary>
public class Rotater2D : MonoBehaviour {

    [SerializeField]
    private float angularSpeed = 1f; // degrees per second
    [SerializeField]
    private Vector3 centreOfRotation = new Vector3(0,0,0);
    [SerializeField]
    private bool isClockwise = true;

    // radius is defined as distance between start position of object and the centre of rotation
    private float radius = 1f;

    // current angle of rotation of the object
    private float angle = 0f;

    // Use this for initialization
    void Start () {
        StartCircling();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = GetCirclingPosition();
    }

    private void StartCircling()
    {
        // radius is defined as distance between start position of object and the centre of rotation
        radius = Vector3.Distance(transform.position, centreOfRotation);

        // initial angle is derived from initial position of the object relative to the centre of rotation
        angle = Mathf.Atan2(centreOfRotation.y - transform.position.y, centreOfRotation.x - transform.position.x) * Mathf.Rad2Deg;
    }

    private Vector3 GetCirclingPosition()
    {
        if (isClockwise)
        {
            angle -= angularSpeed * Time.deltaTime;
        }
        else
        {
            angle += angularSpeed * Time.deltaTime;
        }

        // angle is converted to an X,Y offset from the centre of rotation
        var offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;

        return centreOfRotation + offset;
    }

    /// <summary>
    /// Gets or sets the angular speed.
    /// </summary>
    /// <value>The angular speed.</value>
    public float AngularSpeed
    {
        get { return this.angularSpeed; }
        set { this.angularSpeed = value; }
    }

    /// <summary>
    /// Gets or sets the centre of rotation.
    /// </summary>
    /// <value>The centre of rotation.</value>
    public Vector3 CentreOfRotation
    {
        get { return this.centreOfRotation; }
        set { this.centreOfRotation = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the rotation <see cref="T:Rotater"/> is clockwise.
    /// </summary>
    /// <value><c>true</c> if is clockwise; otherwise, <c>false</c>.</value>
    public bool IsClockwise
    {
        get { return this.isClockwise; }
        set { this.isClockwise = value; }
    }
}
