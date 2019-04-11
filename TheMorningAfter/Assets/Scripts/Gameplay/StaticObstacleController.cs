using UnityEngine;
using System.Collections;

/// <summary>
/// Handles static creatures.
/// 
/// Strictly speaking this should be called StaticCreatureController, although
/// the original reqirements did not explicitly require all obstacles
/// to be creatures (hence the more generic "obstacle").
/// </summary>
public class StaticObstacleController : ObstacleController
{
    public StaticObstacleController()
        : base()
    {
        this.direction = MovementDirection.STATIC;
    }
}