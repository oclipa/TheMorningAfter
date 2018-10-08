using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoDownCommand : Command
{
    public override void Execute(PlayerController playerController)
    {
        if (playerController.IsOnLadder)
            playerController.PlayerState = PlayerState.CLIMBING;

        changeDirection(playerController, GameConstants.DOWN);

        if (playerController.CurrentSlopeNormalX != 0.0f)
            playerController.IsSolid = false;
    }
}
