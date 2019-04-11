using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Go up command.
/// </summary>
public class GoUpCommand : Command
{
    public override void Execute(PlayerController playerController)
    {
        // If player is on a ladder, this means they are climbing
        if (playerController.IsOnLadder)
            playerController.PlayerState = PlayerState.CLIMBING;

        changeDirection(playerController, GameConstants.UP);
    }
}
