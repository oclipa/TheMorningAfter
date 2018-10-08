using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : MovingState
{
    public override void UpdateInput(PlayerController playerController)
    {
        base.UpdateInput(playerController);

        if (playerController.IsOnLadder)
        {
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
        Transform transform = playerController.Transform;

        if (playerController.CurrentDirection == GameConstants.RIGHT)
        {
            transform.Translate(Vector3.right * GameConstants.PLAYER_WALK_SPEED * Time.fixedDeltaTime);
        }
        else if (playerController.CurrentDirection == GameConstants.LEFT)
        {
            transform.Translate(Vector3.left * GameConstants.PLAYER_WALK_SPEED * Time.fixedDeltaTime);
        }

        // handle case where character is walking up or down a slope
        // (need to smooth speed up/down slope)
        handleSlope(playerController, GameConstants.SLOPE_FRICTION);
    }
}
