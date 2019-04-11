using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When the player is in this state, they are simply walking.
/// jumping.
/// </summary>
public class WalkingState : MovingState
{
    public override void UpdateInput(PlayerController playerController)
    {
        base.UpdateInput(playerController);

        if (playerController.IsOnLadder)
        {
            // if the player is touching a ladder, switch to the climbing
            // state just in case the player wants to move up or down.
            playerController.PlayerState = PlayerState.CLIMBING;
        }
        else
        {
            foreach (Command command in playerController.CommandHandler.HandleInput(playerController))
            {
                command.Execute(playerController);
            }
        }
    }

    public override void UpdatePhysics(PlayerController playerController)
    {
        if (playerController.CurrentDirection == GameConstants.RIGHT)
        {
            moveRight(playerController);
        }
        else if (playerController.CurrentDirection == GameConstants.LEFT)
        {
            moveLeft(playerController);
        }

        // handle case where character is walking up or down a slope
        // (need to smooth speed up/down slope)
        handleSlope(playerController, GameConstants.SLOPE_FRICTION);
    }
}
