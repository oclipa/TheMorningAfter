using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// Mobile command handler.
/// </summary>
public class MobileCommandHandler : CommandHandler
{
    private static float AccelerometerUpdateInterval = 1.0f / 60.0f;
    private static float LowPassKernelWidthInSeconds = 1.0f;
    private float LowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds; // tweakable

    private Vector3 lowPassValue = Vector3.zero;

    private Vector3 LowPassFilterAccelerometer()
    {
        lowPassValue = Vector3.Lerp(lowPassValue, Input.acceleration, LowPassFilterFactor);
        return lowPassValue;
    }

    /// <summary>
    /// Reads all commands supplied in current frame
    /// </summary>
    /// <returns>The input.</returns>
    /// <param name="playerController">Player controller.</param>
    public override IEnumerable<Command> HandleInput(PlayerController playerController)
    {
        List<Command> commands = new List<Command>();

        //if (lowPassValue == Vector3.zero)
            //lowPassValue = Input.acceleration;

        Vector3 accel = Input.acceleration;  //LowPassFilterAccelerometer();

        float x = accel.x;
        float y = accel.y;
        float z = accel.z;

        int touchCount = Input.touchCount;
        Vector2[] touches = new Vector2[touchCount];
        for(int t=0; t<touchCount; t++)
        {
            touches[t] = Camera.main.ScreenToWorldPoint(Input.GetTouch(t).position);
            //Debug.Log("touch = " + touches[t]);
        }

        //Debug.Log("x = " + x + System.Environment.NewLine + 
        //"y = " + y + System.Environment.NewLine +
        //", z = " + z + System.Environment.NewLine +
        //", magnitude = " + Input.acceleration.magnitude);

        float deltaX = 0.1f;
        float deltaY = 0.3f;

        if (touchCount == 0 && Mathf.Abs(x) < deltaX && Mathf.Abs(y) < deltaY) // no touches/insignificant motion
        {
            commands.Add(doNothing);
        }
        else
        {
            if (x < -1 * deltaX) // left
            {
                commands.Add(goLeft);
            }
            else if (x > deltaX) // right
            {
                commands.Add(goRight);
            }

            //if (playerController.IsOnLadder || playerController.CurrentSlopeNormalX != 0.0f) // up/down
            //{
            //    if (y > deltaY)
            //    {
            //        commands.Add(goUp);
            //    }
            //    else if (y < -1 * deltaY)
            //    {
            //        commands.Add(goDown);
            //    }
            //}

            //float screenWidth = Mathf.Abs(ScreenUtils.ScreenLeft) + ScreenUtils.ScreenRight;
            //float screenHeight = Mathf.Abs(ScreenUtils.ScreenBottom) + ScreenUtils.ScreenTop;
            float screenCentreX = (ScreenUtils.ScreenLeft + ScreenUtils.ScreenRight) / 2;
            float screenCentreY = (ScreenUtils.ScreenBottom + ScreenUtils.ScreenTop) / 2;

            //Debug.Log("screenCentreX = " + screenCentreX);
            //Debug.Log("IsGrounded = " + playerController.IsGrounded);
            //Debug.Log("IsOnLadder = " + playerController.IsOnLadder);

            foreach (Vector2 touch in touches)
            {
                if (playerController.IsOnLadder || playerController.CurrentSlopeNormalX != 0.0f) // up/down
                {
                    // top right of screen = UP
                    if (touch.x > screenCentreX && touch.y > screenCentreY)
                    {
                        commands.Add(goUp);
                    }
                    // bottom right of screen = DOWN
                    else if (touch.x > screenCentreX && touch.y < screenCentreY)
                    {
                        commands.Add(goDown);
                    }
                }

                // can only jump if grounded (to avoid double-jumping) 
                // and not on a ladder (for no particular reason other than I say so)
                // top left of screen = JUMP
                if (touch.x < screenCentreX && touch.y > screenCentreY && playerController.IsGrounded && !playerController.IsOnLadder)
                {
                    //Debug.Log("Jump");
                    commands.Add(jump);
                }

                // if player has the weapon, respond to fire commands
                // bottom left of screen = FIRE
                if (touch.x < screenCentreX && touch.y < screenCentreY && playerController.HasWeapon)
                {
                    //Debug.Log("Fire");
                    commands.Add(fire);
                }
            }
        }

        // return all Commands
        foreach (Command command in commands)
        {
            // Theoretically we could track the history of commands, allowing for
            // replay or undo, but this has not been implemented here.
            //commandHistory.Add(Time.time, command);

            yield return command;
        }
    }

    /// <summary>
    /// Has a confirm command been issued?
    /// </summary>
    /// <returns><c>true</c>, if confirm is selected, <c>false</c> otherwise.</returns>
    public override bool IsConfirmSelected()
    {
        return Input.GetKeyUp(KeyCode.Return);
    }

    /// <summary>
    /// Has a escape command been issued?
    /// </summary>
    /// <returns><c>true</c>, if escape is selected, <c>false</c> otherwise.</returns>
    public override bool IsEscapeSelected()
    {
        return Input.GetKeyUp(KeyCode.Escape);
    }
}
