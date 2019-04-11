using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// <see langword="abstract"/> base class for input commands from player
/// </summary>
public abstract class Command
{
    public abstract void Execute(PlayerController playerController);

    protected void changeDirection(PlayerController playerController, string direction)
    {
        if (playerController.CurrentDirection != direction)
            playerController.CurrentDirection = direction;
    }
}
