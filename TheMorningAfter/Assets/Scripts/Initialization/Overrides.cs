using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is purely for debugging/testing purposes since it allows
/// various common properties to be overridden (to jump to certain game states).
/// 
/// In theory this class is not thread-safe, but in practice it should be read-only
/// at run-time, so it is probably OK.
/// </summary>
public static class Overrides
{
    public static bool Enabled = false;
    public static string InitialRoom = "TheBathroom";
    public static Vector3 StartPosition = new Vector3(1.71f, -0.21f, 0);
    //public static string InitialRoom = "TheBedroom";
    //public static Vector3 StartPosition = new Vector3(1.71f, -0.21f, 0);
    //public static string InitialRoom = "TheLimitingFactor";
    //public static Vector3 StartPosition = new Vector3(8.04f, 4.17f, 0);
    //public static string InitialRoom = "TheCellar";
    //public static Vector3 StartPosition = new Vector3(6.09f, -1.61f, 0);
    //public static string InitialRoom = "TheLoft";
    //public static Vector3 StartPosition = new Vector3(-8.14f, -1.61f, 0);
    //public static string InitialRoom = "TheNoseOfTJEckleburg";
    //public static Vector3 StartPosition = new Vector3(8.04f, 4.17f, 0);
    //public static string InitialRoom = "TheFilingCabinet";
    //public static Vector3 StartPosition = new Vector3(8.04f, 4.17f, 0);
    //public static string InitialRoom = "TheSewer";
    //public static Vector3 StartPosition = new Vector3(-8.14f, -1.61f, 0);
    public static bool MaidGivenInstructions = true;
    public static bool MaidMentionedWeapon = true;
    public static bool PlayerHasWeapon = true;
    public static int ItemsCollected = 0; // if >0, will force start in TheBathroom
    //public static int CreaturesKilled = 0;
}
