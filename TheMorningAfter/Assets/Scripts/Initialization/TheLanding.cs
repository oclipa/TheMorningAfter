using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class TheLanding : Room
{
    private static string NAME = "The Landing";

    public TheLanding()
        : base()
    {
        // type, positionX, positionY, width, depth
        string s1 = "Platform,0,-2.24,17.78,0.32";
        string s2 = "Platform,0,4.84,17.78,0.32";
        string s3 = "Platform,5.93,3.36,5.92,0.32";
        string s4 = "Platform,2.81,3.04,0.32,0.32";
        string s5 = "Platform,-5.93,3.36,5.92,0.32";
        string s6 = "Platform,-2.81,3.04,0.32,0.32";
        string s7 = "Platform,0,0.34,10.09,0.32";
        string s8 = "Platform,-8.05,-0.85,1.66,0.32";
        string s9 = "Platform,8.05,-0.85,1.66,0.32";
        string s10 = "Platform,-1,1.73,1.73,0.32";
        string s11 = "Platform,1,1.73,1.66,0.32";

        string s12 = "Wall,-8.73,1.32,0.32,6.77";
        string s13 = "Wall,8.73,0.72,0.32,5.6";
        string s14 = "Wall,8.73,4.75,0.32,0.51";
        string s15 = "Wall,0,2.6,0.32,4.2";

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
        roomParameters.Add(s14);
        roomParameters.Add(s15);

        // type, movement, starting x, starting y, 
        //                                  min constraint, max constraint
//        string o1 = "MovingObstacle,-8.60,-1.57,-8.73,8.73,Horizontal";
        string o3 = "MovingObstacle,-4.50,0.99,-8.73,8.73,Horizontal";
        string o4 = "MovingObstacle,4.47,0.99,-8.73,8.73,Horizontal";
        string o5 = "StaticObstacle,-8.31,-0.19";
        string o6 = "StaticObstacle,8.31,-0.19";
        string o7 = "MovingObstacle,6.08,-0.34,-8.73,8.73,Vertical";
        string o8 = "MovingObstacle,-6.08,-0.34,-8.73,8.73,Vertical";

        //roomParameters.Add(o1);
        roomParameters.Add(o3);
        roomParameters.Add(o4);
        roomParameters.Add(o5);
        roomParameters.Add(o6);
        roomParameters.Add(o7);
        roomParameters.Add(o8);

        // ladders must not touch bottom platform and must extend above top platform
        // type, positionx, position y, qwidth, length
        string l2 = "Ladder,-7.43,4.312,0.32,1.56";

        roomParameters.Add(l2);

        // type, positionx, positiony, points
        string i1 = "Item,-0.32,2.51,TheVase,1";
        string i2 = "Item,0.32,2.51,TheBook,1";
        string i3 = "Item,7.89,-1.19,TheLight,1";
        string i4 = "Item,-7.89,-1.19,TheMirror,1";

        roomParameters.Add(i1);
        roomParameters.Add(i2);
        roomParameters.Add(i3);
        roomParameters.Add(i4);

        // TODO: Add The Stairs?  Stairs -> Hallway -> Garden -> Tree -> Roof
        string d1 = "Door,8.92,4.10,0.04,1.2,TheBathroom";
        //string d2 = "Door,7.40,-2.436,1.2,0.04,TheStairs";
        string d3 = "Door,-7.43,5.02,1.2,0.04,TheLoft";

        roomParameters.Add(d1);
        //roomParameters.Add(d2);
        roomParameters.Add(d3);
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
