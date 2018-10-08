using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class KeyboardCommandHandler : CommandHandler
{
    public override IEnumerable<Command> HandleInput(PlayerController playerController)
    {
        List<Command> commands = new List<Command>();

        if (!Input.anyKey)
        {
            commands.Add(doNothing);
        }
        else
        {
            if (Input.GetAxisRaw(GameConstants.HORIZONTAL) < 0)
            {
                commands.Add(goLeft);
            }
            else if (Input.GetAxisRaw(GameConstants.HORIZONTAL) > 0)
            {
                commands.Add(goRight);
            }

            if (playerController.IsOnLadder || playerController.CurrentSlopeNormalX != 0.0f)
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

            if (Input.GetKey(GameConstants.SPACE) && playerController.IsGrounded && !playerController.IsOnLadder)
            {
                commands.Add(jump);
            }
        }

        foreach (Command command in commands)
        {
            //commandHistory.Add(Time.time, command);

            yield return command;
        }
    }
}
