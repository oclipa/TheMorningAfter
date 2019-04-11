using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class for states where the player is moving.
/// This contains the actual movement code and controls the 
/// animation.
/// </summary>
public abstract class MovingState : PlayerState
{
    public override void UpdateInput(PlayerController playerController)
    {
        // by default, do nothing
    }

    /// <summary>
    /// The default behaviour when moving is that the player sprite will either 
    /// be facing left or right (where "left" and "right" depend on whether the
    /// movement controls are "normal" or "reversed").
    /// </summary>
    /// <param name="playerController">Player controller.</param>
    public override void UpdateAnimation(PlayerController playerController)
    {
        PlayerControlBehavior playerControlBehavior = playerController.PlayerControlBehavior;

        int animationState = 0;

        switch (playerControlBehavior)
        {
            case PlayerControlBehavior.NORMAL:
                if (playerController.CurrentDirection == GameConstants.RIGHT)
                    animationState = GameConstants.ANIMATION_WALK_RIGHT;
                else if (playerController.CurrentDirection == GameConstants.LEFT)
                    animationState = GameConstants.ANIMATION_WALK_LEFT;
                break;
            case PlayerControlBehavior.REVERSED:
                if (playerController.CurrentDirection == GameConstants.RIGHT)
                    animationState = GameConstants.ANIMATION_WALK_LEFT;
                else if (playerController.CurrentDirection == GameConstants.LEFT)
                    animationState = GameConstants.ANIMATION_WALK_RIGHT;
                break;
        }

        changeAnimationState(playerController, animationState);
    }

    /// <summary>
    /// Changes the state of the animation (e.g. walking, climbing, standing, etc.)
    /// </summary>
    /// <param name="playerController">Player controller.</param>
    /// <param name="state">State.</param>
    protected void changeAnimationState(PlayerController playerController, int state)
    {
        if (playerController.CurrentAnimationState != state)
        {
            playerController.Animator.speed = 1;
            playerController.Animator.SetInteger("state", state);
            playerController.CurrentAnimationState = state;
        }
    }

    protected void moveRight(PlayerController playerController)
    {
        Transform transform = playerController.Transform;
        PlayerControlBehavior playerControlBehavior = playerController.PlayerControlBehavior;

        Vector3 right = Vector3.right;
        switch (playerControlBehavior)
        {
            case PlayerControlBehavior.REVERSED:
                right = Vector3.left;
                break;
        }

        transform.Translate(right * getWalkRate(playerController) * Time.fixedDeltaTime);
    }

    protected void moveLeft(PlayerController playerController)
    {
        Transform transform = playerController.Transform;
        PlayerControlBehavior playerControlBehavior = playerController.PlayerControlBehavior;

        Vector3 left = Vector3.left;
        switch (playerControlBehavior)
        {
            case PlayerControlBehavior.REVERSED:
                left = Vector3.right;
                break;
        }

        transform.Translate(left * getWalkRate(playerController) * Time.fixedDeltaTime);
    }

    protected void moveUp(PlayerController playerController)
    {
        Transform transform = playerController.Transform;

        Vector3 up = Vector3.up;
        if (Mathf.Sign(Physics2D.gravity.y) > 0)
            up = Vector3.down;

        transform.Translate(up * getClimbRate(playerController) * Time.fixedDeltaTime);
    }

    protected void moveDown(PlayerController playerController)
    {
        Transform transform = playerController.Transform;

        Vector3 down = Vector3.down;
        if (Mathf.Sign(Physics2D.gravity.y) > 0)
            down = Vector3.up;

        transform.Translate(down * getClimbRate(playerController) * Time.fixedDeltaTime);
    }

    protected void jumpRight(PlayerController playerController)
    {
        Rigidbody2D rigidbody2D = playerController.Rigidbody2D;
        PlayerControlBehavior playerControlBehavior = playerController.PlayerControlBehavior;

        Vector2 up = -1 * Mathf.Sign(Physics2D.gravity.y) * Vector2.up;

        Vector2 right = Vector2.right;
        switch (playerControlBehavior)
        {
            case PlayerControlBehavior.REVERSED:
                right = Vector2.left;
                break;
        }

        rigidbody2D.velocity = right * getJumpRateX(playerController) + up * getJumpRateY(playerController);
    }

    protected void jumpLeft(PlayerController playerController)
    {
        Rigidbody2D rigidbody2D = playerController.Rigidbody2D;
        PlayerControlBehavior playerControlBehavior = playerController.PlayerControlBehavior;

        Vector2 up = -1 * Mathf.Sign(Physics2D.gravity.y) * Vector2.up;

        Vector2 left = Vector2.left;
        switch (playerControlBehavior)
        {
            case PlayerControlBehavior.REVERSED:
                left = Vector2.right;
                break;
        }

        rigidbody2D.velocity = left * getJumpRateX(playerController) + up * getJumpRateY(playerController);
    }

    protected void jumpUp(PlayerController playerController)
    {
        Rigidbody2D rigidbody2D = playerController.Rigidbody2D;

        Vector2 up = -1 * Mathf.Sign(Physics2D.gravity.y) * Vector2.up;

        rigidbody2D.velocity = up * GameConstants.PLAYER_JUMP_SPEED_Y;
    }

    protected float getWalkRate(PlayerController playerController)
    {
        float playerSpeedMultiplier = getPlayerSpeedMultiplier(playerController);

        return GameConstants.PLAYER_WALK_SPEED * playerSpeedMultiplier;
    }

    private static float getPlayerSpeedMultiplier(PlayerController playerController)
    {
        float playerSpeedMultiplier = playerController.PlayerSpeedMultiplier;
        if (playerSpeedMultiplier > 1.0f)
        {
            float difficultyMultiplier = GameConstants.MediumDifficulty; //playerController.DifficultyMultiplier;
            playerSpeedMultiplier = playerSpeedMultiplier * difficultyMultiplier;
        }

        return playerSpeedMultiplier;
    }

    protected float getClimbRate(PlayerController playerController)
    {
        float playerSpeedMultiplier = getPlayerSpeedMultiplier(playerController);

        return GameConstants.PLAYER_CLIMB_SPEED * playerSpeedMultiplier;
    }

    protected float getJumpRateX(PlayerController playerController)
    {
        float playerSpeedMultiplier = getPlayerSpeedMultiplier(playerController);

        return GameConstants.PLAYER_JUMP_SPEED_X * playerSpeedMultiplier;
    }

    protected float getJumpRateY(PlayerController playerController)
    {
        return GameConstants.PLAYER_JUMP_SPEED_Y;
    }
}