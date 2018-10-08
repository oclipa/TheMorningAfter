using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class TheBedroom : Room
{
    public const string NAME = "The Bedroom";

    public TheBedroom()
        : base()
    {
        // type, positionX, positionY, width, depth
        string s1 = "Platform,0,-2.24,17.78,0.32";
        string s2 = "Platform,0,4.84,17.78,0.32";
        string s3 = "Wall,8.73,1.32,0.32,6.77";
        string s4 = "Wall,3.62,1.77,0.64,5.81";
        string s5 = "Wall,-8.73,1.8,0.32,5.79";

        roomParameters.Add(s1);
        roomParameters.Add(s2);
        roomParameters.Add(s3);
        roomParameters.Add(s4);
        roomParameters.Add(s5);

        string sc1 = "SceneryObstacle,7.95,-1.835,1.2,0.46,Bed";

        roomParameters.Add(sc1);

        string maria = "Maria,3.6,-1.60,1.2,0.64";

        roomParameters.Add(maria);

        string d1 = "Door,-8.92,-1.58,0.04,1.2,TheBathroom";

        roomParameters.Add(d1);
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <returns>The name.</returns>
    public override string GetName()
    {
        return NAME;
    }

    /// <summary>
    /// Gets the unique id for this room.
    /// </summary>
    /// <returns>The id.</returns>
    public override string GetID()
    {
        return NAME.Replace(" ", string.Empty); ;
    }
}
