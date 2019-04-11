using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When the player is in this state, they are simply standing still.
/// jumping.
/// </summary>
public class StandingState : PlayerState 
{
    public override void UpdateInput(PlayerController playerController)
    {
        foreach (Command command in playerController.CommandHandler.HandleInput(playerController))
        {
            command.Execute(playerController);
        }
    }

    public override void UpdatePhysics(PlayerController playerController)
    {
        playerController.CurrentDirection = GameConstants.STATIONARY;

        // handle case where character is standing on slope
        // (don't want it to slide down slope)
        handleSlope(playerController, GameConstants.SLOPE_FRICTION);
    }

    public override void UpdateAnimation(PlayerController playerController)
    {
        if (playerController.CurrentAnimationState == GameConstants.ANIMATION_IDLE)
            return;

        playerController.CurrentAnimationState = GameConstants.ANIMATION_IDLE;

        Animator animator = playerController.Animator;
        animator.speed = 0;
    }
}
