using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Transform transform = playerController.Transform;

        if (playerController.CurrentDirection == GameConstants.RIGHT)
        {
            transform.Translate(Vector3.right * GameConstants.PLAYER_WALK_SPEED * Time.fixedDeltaTime);
        }
        else if (playerController.CurrentDirection == GameConstants.LEFT)
        {
            transform.Translate(Vector3.left * GameConstants.PLAYER_WALK_SPEED * Time.fixedDeltaTime);
        }
        else if (playerController.CurrentDirection == GameConstants.UP)
        {
            transform.Translate(Vector3.up * GameConstants.PLAYER_CLIMB_SPEED * Time.fixedDeltaTime);
        }
        else if (playerController.CurrentDirection == GameConstants.DOWN)
        {
            transform.Translate(Vector3.down * GameConstants.PLAYER_CLIMB_SPEED * Time.fixedDeltaTime);
        }
    }
}
