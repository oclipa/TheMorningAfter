using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all obstacles
/// </summary>
public abstract class ObstacleController : MonoBehaviour
{
    // An obstacle has a motion (including static)
    protected MovementDirection direction;

    // An obstacle has a Rigidbody2D for collisions
    protected Rigidbody2D rb;

    // Use this for initialization
    protected virtual void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
    }
}