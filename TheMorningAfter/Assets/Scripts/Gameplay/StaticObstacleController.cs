using UnityEngine;
using System.Collections;

/// <summary>
/// Handles a staic obstacle
/// </summary>
public class StaticObstacleController : ObstacleController
{
    public StaticObstacleController()
        : base()
    {
        this.direction = MovementDirection.STATIC;
    }
}