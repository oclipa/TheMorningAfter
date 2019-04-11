using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// Keyboard command handler.
/// </summary>
public class KeyboardCommandHandler : CommandHandler
{
    /// <summary>
    /// Reads all commands supplied in current frame
    /// </summary>
    /// <returns>The input.</returns>
    /// <param name="playerController">Player controller.</param>
    public override IEnumerable<Command> HandleInput(PlayerController playerController)
    {
        List<Command> commands = new List<Command>();

        if (!Input.anyKey) // nothing pressed
        {
            commands.Add(doNothing);
        }
        else
        {
            if (Input.GetAxisRaw(GameConstants.HORIZONTAL) < 0) // left
            {
                commands.Add(goLeft);
            }
            else if (Input.GetAxisRaw(GameConstants.HORIZONTAL) > 0) // right
            {
                commands.Add(goRight);
            }

            if (playerController.IsOnLadder || playerController.CurrentSlopeNormalX != 0.0f) // up/down
            {
                if (Input.GetAxisRaw(GameConstants.VERTICAL) > 0)
                {
                    commands.Add(goUp);
                }
                else if (Input.GetAxisRaw(GameConstants.VERTICAL) < 0)
                {
                    commands.Add(goDown);
                }
            }

            // can only jump if grounded (to avoid double-jumping) 
            // and not on a ladder (for no particular reason other than I say so)
            if (Input.GetKey(GameConstants.SPACE) && playerController.IsGrounded && !playerController.IsOnLadder)
            {
                commands.Add(jump);
            }

            // if player has the weapon, respond to fire commands
            if (playerController.HasWeapon && (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)))
            {
                commands.Add(fire);
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
