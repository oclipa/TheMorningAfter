using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Jump command.
/// </summary>
public class JumpCommand : Command
{
    public override void Execute(PlayerController playerController)
    {
        playerController.PlayerState = PlayerState.JUMPING;
    }
}
