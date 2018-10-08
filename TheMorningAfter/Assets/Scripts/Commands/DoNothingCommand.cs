using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNothingCommand : Command
{
    public override void Execute(PlayerController playerController)
    {
        playerController.PlayerState = PlayerState.STANDING;
    }
}
