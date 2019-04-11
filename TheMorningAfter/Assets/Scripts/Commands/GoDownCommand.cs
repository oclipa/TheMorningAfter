using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Go down command.
/// </summary>
public class GoDownCommand : Command
{
    public override void Execute(PlayerController playerController)
    {
        // If player is on a ladder, this means they are climbing
        if (playerController.IsOnLadder)
            playerController.PlayerState = PlayerState.CLIMBING;

        changeDirection(playerController, GameConstants.DOWN);

        // If player goes down while on a ramp, the player
        // can pass through the ramp.
        if (playerController.CurrentSlopeNormalX != 0.0f)
            playerController.IsSolid = false;
    }
}
