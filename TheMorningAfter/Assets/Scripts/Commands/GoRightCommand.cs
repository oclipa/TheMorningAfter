using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Go right command.
/// </summary>
public class GoRightCommand : Command
{
    public override void Execute(PlayerController playerController)
    {
        // if player is going left, assume they are walking
        playerController.PlayerState = PlayerState.WALKING;

        changeDirection(playerController, GameConstants.RIGHT);
    }
}
