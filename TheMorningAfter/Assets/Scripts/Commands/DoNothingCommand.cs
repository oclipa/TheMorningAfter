using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Do nothing command (e.g. standing on the spot)
/// </summary>
public class DoNothingCommand : Command
{
    public override void Execute(PlayerController playerController)
    {
        playerController.PlayerState = PlayerState.STANDING;
    }
}
