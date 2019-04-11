using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Jump command.
/// </summary>
public class FireCommand : Command
{
    public override void Execute(PlayerController playerController)
    {
        GameObject weapon = GameObject.FindGameObjectWithTag(GameConstants.WEAPON);
        if (weapon != null)
        {
            weapon.GetComponent<WeaponController>().FireOneShot();
        }
    }
}
