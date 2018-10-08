using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class TheLoft : Room
{
    private const string NAME = "The Loft";

    public TheLoft()
        : base()
    {
        // type, positionX, positionY, width, depth
        string s1 = "Platform,0,-2.24,17.78,0.32";
        string s2 = "Platform,0,4.84,17.78,0.32";

        string s3 = "Platform,0,3.52,2.56,0.32";

        string s4 = "Platform,0,2.08,6.40,0.32";

        string s5 = "Platform,-5.98,2.50,0.64,0.32";
        string s6 = "Platform,5.98,2.50,0.64,0.32";

        string s7 = "Platform,-3.32,0.64,3.20,0.32";
        string s8 = "Platform,3.32,0.64,3.20,0.32";

        string s9 = "Platform,-4.98,-0.80,3.20,0.32";
        string s10 = "Platform,0,-0.80,2.56,0.32";
        string s11 = "Platform,4.98,-0.80,3.20,0.32";

        string s12 = "Wall,-8.73,1.32,0.32,6.77";
        string s13 = "Wall,8.73,1.32,0.32,6.77";

        roomParameters.Add(s1);
        roomParameters.Add(s2);
        roomParameters.Add(s3);
        roomParameters.Add(s4);
        roomParameters.Add(s5);
        roomParameters.Add(s6);
        roomParameters.Add(s7);
        roomParameters.Add(s8);
        roomParameters.Add(s9);
        roomParameters.Add(s10);
        roomParameters.Add(s11);
        roomParameters.Add(s12);
        roomParameters.Add(s13);

        // type, movement, starting x, starting y, 
        //                                  min constraint, max constraint
        string o1 = "MovingObstacle,-6.40,-0.16,-8.73,8.73,Horizontal";
        string o2 = "MovingObstacle,6.40,-0.16,-8.73,8.73,Horizontal";

        string o3 = "MovingObstacle,-4.92,1.28,-8.73,8.73,Horizontal";
        string o4 = "MovingObstacle,4.92,1.28,-8.73,8.73,Horizontal";

        string o5 = "MovingObstacle,-3.00,2.72,-8.73,8.73,Horizontal";
        string o6 = "MovingObstacle,3.00,2.72,-8.73,8.73,Horizontal";

        string o7 = "MovingObstacle,-2.56,-1.48,-8.73,8.73,Vertical";
        string o8 = "MovingObstacle,2.56,-1.48,-8.73,8.73,Vertical";
        string o9 = "MovingObstacle,0,-0.64,-8.73,8.73,Vertical";

        roomParameters.Add(o1);
        roomParameters.Add(o2);
        roomParameters.Add(o3);
        roomParameters.Add(o4);
        roomParameters.Add(o5);
        roomParameters.Add(o6);
        roomParameters.Add(o7);
        roomParameters.Add(o8);
        roomParameters.Add(o9);

        // ladders must not touch bottom platform and must extend above top platform
        // type, positionx, position y, qwidth, length
        string l2 = "Ladder,-7.43,-2.24,0.32,0.64";

        roomParameters.Add(l2);

        // type, positionx, positiony, id, points
        string i1 = "Item,0,4.48,TheBox,1";
        string i2 = "Item,0,-0.48,TheThing,1";

        roomParameters.Add(i1);
        roomParameters.Add(i2);

        // TODO: Add The Roof - roof should allow drop down into Bathroom?
        string d1 = "Door,-7.43,-2.436,1.2,0.04,TheLanding";

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
