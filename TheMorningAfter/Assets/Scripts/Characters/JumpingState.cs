using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When the player is in this state, they are in the process of
/// jumping.
/// </summary>
public class JumpingState : MovingState
{
    public override void UpdatePhysics(PlayerController playerController)
    {
        // avoid double-jumping
        if (playerController.IsGrounded)
        {
            playerController.IsGrounded = false;

            AudioManager.Instance.PlayOneShot(AudioClipName.Jump);

            // after jumping, the player will enter another state
            if (playerController.CurrentDirection == GameConstants.RIGHT)
            {
                jumpRight(playerController);

                playerController.PlayerState = PlayerState.WALKING;
            }
            else if (playerController.CurrentDirection == GameConstants.LEFT)
            {
                jumpLeft(playerController);

                playerController.PlayerState = PlayerState.WALKING;
            }
            else if (playerController.CurrentDirection == GameConstants.STATIONARY)
            {
                jumpUp(playerController);

                playerController.PlayerState = PlayerState.STANDING;
            }
        }
    }
}
