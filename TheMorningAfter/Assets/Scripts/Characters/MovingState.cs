using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingState : PlayerState 
{
    public override void UpdateInput(PlayerController playerController)
    {
    }

    public override void UpdateAnimation(PlayerController playerController)
    {
        if (playerController.CurrentDirection == GameConstants.RIGHT)
            changeAnimationState(playerController, GameConstants.ANIMATION_WALK_RIGHT);
        else if (playerController.CurrentDirection == GameConstants.LEFT)
            changeAnimationState(playerController, GameConstants.ANIMATION_WALK_LEFT);
    }

    protected void changeAnimationState(PlayerController playerController, int state)
    {
        if (playerController.CurrentAnimationState != state)
        {
            playerController.Animator.speed = 1;
            playerController.Animator.SetInteger("state", state);
            playerController.CurrentAnimationState = state;
        }
    }
}
