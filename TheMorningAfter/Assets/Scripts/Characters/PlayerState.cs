using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState {

    // This (and related classes) is based on the State pattern

    // If we ever have to consider multi-threading, these should not be static
    public static StandingState STANDING = new StandingState();
    public static WalkingState WALKING = new WalkingState();
    public static JumpingState JUMPING = new JumpingState();
    public static ClimbingState CLIMBING = new ClimbingState();
    //public static SwingingState SWINGING = new SwingingState();

    // Update is called once per frame
    public abstract void UpdateInput(PlayerController playerController);
    public abstract void UpdatePhysics(PlayerController playerController);
    public abstract void UpdateAnimation(PlayerController playerController);

    protected void handleSlope(PlayerController playerController, float slopeFriction)
    {
        if (playerController.CurrentSlopeNormalX != 0.0f)
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
