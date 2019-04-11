using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When the player is in this state, they are in the process of
/// firing the weapon.
/// </summary>
public class FiringState : PlayerState
{
    public override void UpdateInput(PlayerController playerController)
    {
        foreach (Command command in playerController.CommandHandler.HandleInput(playerController))
        {
            command.Execute(playerController);
        }
    }

    public override void UpdatePhysics(PlayerController playerController)
    {
        // nothing to do here
    }

    public override void UpdateAnimation(PlayerController playerController)
    {
        // nothing to do here
    }
}
