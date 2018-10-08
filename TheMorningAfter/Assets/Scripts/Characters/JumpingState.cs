using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : MovingState
{
    public override void UpdatePhysics(PlayerController playerController)
    {
        Transform transform = playerController.Transform;
        Rigidbody2D rigidbody2D = playerController.Rigidbody2D;

        if (playerController.CurrentDirection == GameConstants.RIGHT)
        {
            transform.Translate(Vector3.right * GameConstants.PLAYER_WALK_SPEED * Time.fixedDeltaTime);
            rigidbody2D.AddForce(new Vector2(GameConstants.PLAYER_JUMP_SPEED_X, GameConstants.PLAYER_JUMP_SPEED_Y));

            playerController.PlayerState = PlayerState.WALKING;
        }
        else if (playerController.CurrentDirection == GameConstants.LEFT)
        {
            transform.Translate(Vector3.left * GameConstants.PLAYER_WALK_SPEED * Time.fixedDeltaTime);
            rigidbody2D.AddForce(new Vector2(-1 * GameConstants.PLAYER_JUMP_SPEED_X, GameConstants.PLAYER_JUMP_SPEED_Y));

            playerController.PlayerState = PlayerState.WALKING;
        }
        else if (playerController.CurrentDirection == GameConstants.STATIONARY)
        {
            rigidbody2D.AddForce(new Vector2(0, GameConstants.PLAYER_JUMP_SPEED_Y));

            playerController.PlayerState = PlayerState.STANDING;
        }
    }
}
