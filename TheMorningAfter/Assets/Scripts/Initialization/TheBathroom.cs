using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class TheBathroom : Room 
{
    private const string NAME = "The Bathroom";

    public TheBathroom()
        : base()
    {
        // type, positionX, positionY, width, depth
        string s1 = "Platform,0,-2.24,17.78,0.32";
        string s2 = "Platform,4.365,-0.84,8.41,0.32";
        string s3 = "Platform,-4.365,0.56,8.41,0.32";
        string s4 = "Platform,4.865,1.96,7.41,0.32";
        string s5 = "Platform,-4.365,3.36,8.41,0.32";
        string s6 = "Wall,8.73,0.02,0.32,4.18";
        string s7 = "Wall,8.73,4.05,0.32,1.89";
        string s8 = "Wall,-8.73,0.72,0.32,5.6";
        string s9 = "Wall,-8.73,4.75,0.32,0.51";
        string s11 = "Platform,0,4.84,17.78,0.32";

        roomParameters.Add(s1);
        roomParameters.Add(s2);
        roomParameters.Add(s3);
        roomParameters.Add(s4);
        roomParameters.Add(s5);
        roomParameters.Add(s6);
        roomParameters.Add(s7);
        roomParameters.Add(s8);
        roomParameters.Add(s9);
        roomParameters.Add(s11);

        // type, movement, starting x, starting y, 
        //                                  min constraint, max constraint
        string o1 = "MovingObstacle,-4,4.00,-8.73,8.73,Horizontal";

        roomParameters.Add(o1);

        // ladders must not touch bottom platform and must extend above top platform
        // type, positionx, position y, qwidth, length
        string l1 = "Ladder,-7.06,2.145,0.32,2.85";

        roomParameters.Add(l1);

        // type, positionx, positiony, width, length
        string r1 = "Ramp,-1.19,-1.4,3,0.1";

        roomParameters.Add(r1);

        //// type, positionx, positiony, length (in sections)
        //string rope1 = "RopeAnchor,-0.50,4.78,25";

        //roomParameters.Add(rope1);

        // type, positionx, positiony, id, points
        string i1 = "Item,-8.4,-0.02,TheSponge,1";
        string i2 = "Item,8.4,-1.18,ThePlunger,5";
        string i3 = "Item,-4.38,4.50,TheShowerHat,10";

        roomParameters.Add(i1);
        roomParameters.Add(i2);
        roomParameters.Add(i3);

        string sc1 = "SceneryObstacle,8.36,-0.36,0.4,0.64,Toilet";
        string sc2 = "SceneryObstacle,4,-0.40,1.6,0.53,Bath";
        string sc3 = "SceneryObstacle,7.0,2.28,0.82,0.64,Sign";

        roomParameters.Add(sc1);
        roomParameters.Add(sc2);
        roomParameters.Add(sc3);

        string d1 = "Door,8.92,2.611,0.04,1.2,TheBedroom";
        string d2 = "Door,-8.92,3.994,0.04,1.2,TheLanding";

        roomParameters.Add(d1);
        roomParameters.Add(d2);
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
