using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Go left command.
/// </summary>
public class GoLeftCommand : Command
{
    public override void Execute(PlayerController playerController)
    {
        // if player is going left, assume they are walking
        playerController.PlayerState = PlayerState.WALKING;

        changeDirection(playerController, GameConstants.LEFT);
    }
}
