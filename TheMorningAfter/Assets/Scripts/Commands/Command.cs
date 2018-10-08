using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    public abstract void Execute(PlayerController playerController);

    protected void changeDirection(PlayerController playerController, string direction)
    {
        if (playerController.CurrentDirection != direction)
            playerController.CurrentDirection = direction;
    }
}
