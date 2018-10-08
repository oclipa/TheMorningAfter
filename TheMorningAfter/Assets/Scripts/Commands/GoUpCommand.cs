using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoUpCommand : Command
{
    public override void Execute(PlayerController playerController)
    {
        if (playerController.IsOnLadder)
            playerController.PlayerState = PlayerState.CLIMBING;

        changeDirection(playerController, GameConstants.UP);
    }
}
