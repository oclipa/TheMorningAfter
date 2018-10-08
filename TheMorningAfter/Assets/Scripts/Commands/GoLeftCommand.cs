using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoLeftCommand : Command
{
    public override void Execute(PlayerController playerController)
    {
        playerController.PlayerState = PlayerState.WALKING;

        changeDirection(playerController, GameConstants.LEFT);
    }
}
