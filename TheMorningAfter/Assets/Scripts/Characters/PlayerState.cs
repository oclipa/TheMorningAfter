using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState {

    // This (and related classes) is based on the State pattern

    // TODO: These should probably be refactored to make them thread-safe
    public static StandingState STANDING = new StandingState();
    public static WalkingState WALKING = new WalkingState();
    public static JumpingState JUMPING = new JumpingState();
    public static ClimbingState CLIMBING = new ClimbingState();
    public static FiringState FIRING = new FiringState();
    //public static SwingingState SWINGING = new SwingingState();

    // Update is called once per frame
    public abstract void UpdateInput(PlayerController playerController);
    public abstract void UpdatePhysics(PlayerController playerController);
    public abstract void UpdateAnimation(PlayerController playerController);

    /// <summary>
    /// When walking up a slope, the player needs some help in order to allow
    /// a smooth motion (so they don't get stuck on the slope).  This method 
    /// applies a force opposite to the slope force.
    /// </summary>
    /// <param name="playerController">Player controller.</param>
    /// <param name="slopeFriction">Slope friction.</param>
    protected void handleSlope(PlayerController playerController, float slopeFriction)
    {
        if (playerController.CurrentSlopeNormalX != 0.0f && !playerController.IsOnLadder)
        {
            Vector2 playerVelocity = playerController.Rigidbody2D.velocity;
            Vector3 playerPosition = playerController.Transform.position;
            float slopeNormalX = playerController.CurrentSlopeNormalX;

            float playerVelocityX = playerVelocity.x;
            float playerVelocityY = playerVelocity.y;

            // Apply the opposite force against the slope force 
            playerVelocity = new Vector2(playerVelocityX
                                         - (slopeNormalX * slopeFriction),
                                                playerVelocityY);

            //Move Player up or down to compensate for the slope below them
            playerPosition.y += -slopeNormalX
                                    * Mathf.Abs(playerVelocityX)
                                    * Time.fixedDeltaTime
                                    * (playerVelocityX - slopeNormalX > 0 ? 1 : -1);

            playerController.Rigidbody2D.velocity = playerVelocity;
            playerController.Transform.position = playerPosition;

        }
    }
}
