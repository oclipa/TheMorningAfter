using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When the player is in this state, the controls allow them to move
/// as if climbing.
/// </summary>
public class ClimbingState : MovingState
{
    public override void UpdateInput(PlayerController playerController)
    {
        foreach(Command command in playerController.CommandHandler.HandleInput(playerController))
        {
            command.Execute(playerController);
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
        else if (playerController.CurrentDirection == GameConstants.UP)
        {
            moveUp(playerController);
        }
        else if (playerController.CurrentDirection == GameConstants.DOWN)
        {
            moveDown(playerController);
        }
    }

    public override void UpdateAnimation(PlayerController playerController)
    {
        changeAnimationState(playerController, GameConstants.ANIMATION_CLIMB);
    }
}
