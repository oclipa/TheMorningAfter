using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoRightCommand : Command
{
    public override void Execute(PlayerController playerController)
    {
        playerController.PlayerState = PlayerState.WALKING;

        changeDirection(playerController, GameConstants.RIGHT);
    }
}
