using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All important game constants.
/// 
/// Yes this is static class, but it should be thread-safe enough, 
/// in the content of this game (if that is required in the future)
/// </summary>
public static class GameConstants {

    /// <summary>
    /// Theoretically TotalTreasures and TotalCreatures are not thread-safe, since
    /// they can be mutated at any time, however in practice they are
    /// only mutated during the Loader, so this should not be a problem.
    /// (afterwards they are effectively read-only)
    /// </summary>
    public static int TotalTreasures = 0; // static, rather than const, because populated by Loader
    public static int TotalCreatures = 0; // static, rather than const, because populated by Loader

    // const values are thread-safe since they cannot be mutated.
    // Also, strings are inherently thread-safe since they are
    // only ever read-only.

    public const string DefaultRoom = "TheBathroom";

    public const int MaxLives = 7;

    public const float EasyDifficulty = 0.50f;
    public const float MediumDifficulty = 0.75f;
    public const float HardDifficulty = 1.00f;

    public const string RIGHT = "right";
    public const string LEFT = "left";
    public const string SPACE = "space";
    public const string HORIZONTAL = "Horizontal";
    public const string VERTICAL = "Vertical";
    public const string CIRCLING = "Circling";
    public const string UP = "Up";
    public const string DOWN = "Down";
    public const string STATIONARY = "Stationary";
    public const string FORWARD = "forward";
    public const string BACKWARD = "backward";

    public const string GAMETITLETEXT = "GameTitleText";
    public const string ROOMTITLECANVAS = "RoomTitleCanvas";
    public const string ROOMTITLETEXT = "RoomTitleText";
    public const string SCOREBOARD = "ScoreBoard";
    public const string FINALSTATETEXT = "FinalStateText";
    public const string FINALSCORETEXT = "FinalScoreText";
    public const string TIMETEXT = "TimeText";
    public const string ITEMSCOLLECTEDTEXT = "ItemsCollectedText";
    public const string MESSAGETEXT = "MessageText";
    public const string BLANKSCREENBACKGROUND = "BlankScreenBackground";
    public const string LIVESPRITE = "LivesSprite";
    public const string LOADINGTITLE = "LoadingTitle";
    public const string LOADINGTEXT = "LoadingText";
    public const string PLAYER = "Player";
    public const string WALL = "Wall";
    public const string PLATFORM = "Platform";
    public const string OBSTACLE = "Obstacle";
    public const string STATIC_OBSTACLE = "StaticObstacle";
    public const string MOVING_OBSTACLE = "MovingObstacle";
    public const string SCENERY_OBSTACLE = "SceneryObstacle";
    public const string LADDER = "Ladder";
    public const string AERIAL = "Aerial";
    public const string CRATE_SHAPE = "CrateShape";
    public const string RAMP = "Ramp";
    public const string ROPE_ANCHOR = "RopeAnchor";
    public const string ROPE_SECTION = "RopeSection";
    public const string ITEM = "Item";
    public const string POWERUP = "PowerUp";
    public const string EXTRALIFE = "ExtraLife";
    public const string WEAPON = "TheWeapon";
    public const string KEY = "Key";
    public const string DOOR = "Door";
    public const string MAID = "Maid";
    public const string PHYSICS = "Physics";
    public const string PORTAL = "Portal";
    public const string BED = "Bed";
    public const string GAMEOVER = "GameOver";

    public const string MAINMENU = "MainMenu";
    public const string MAINMENUCANVAS = "MainMenuCanvas";

    public const float PLAYER_WALK_SPEED = 2.5f; // V1 = 2.0f // V2 = 2.5f
    public const float PLAYER_CLIMB_SPEED = 2.5f;
    public const float PLAYER_JUMP_SPEED_X = 1.1f; // V1 = 1.3f // V2 = 1.1f
    public const float PLAYER_JUMP_SPEED_Y = 5.7f; // V2 = 5.6f 

    public const float OBSTACLE_WALK_SPEED_X = 2.5f; // 1.9f
    public const float OBSTACLE_WALK_SPEED_Y = 1.2f;
    public const float OBSTACLE_BOUNCE_FORCE = 300f;

    public const float MAX_FALL_DISTANCE = 3.84f; // 4 body lengths

    // these tolerances can be used to determine how close to the
    // end of a platform the obstacles can get (the greater the distance,
    // the easier it is for the player)
    public const float OBSTACLE_X_TOLERANCE = 0.00f; // 0.48f; 0.64f; 0.80f;
    public const float OBSTACLE_Y_TOLERANCE = 0.96f; // 0.96f; 1.12f; 1.28f;

    // Provide SLOPE_FRICTION to stabilize movement
    public const float SLOPE_FRICTION = 10f;

    public const int ANIMATION_WALK_RIGHT = 0;
    public const int ANIMATION_WALK_LEFT = 1;
    public const int ANIMATION_IDLE = 2;
    public const int ANIMATION_JUMP = 3;
    public const int ANIMATION_CLIMB = 4;

    /// <summary>
    /// DateTime cannot be const.  Given this, in theory this is not thread-safe,
    /// since it could be mutated at any time, however in practice DEFAULT_TIME
    /// is never updated and is effectivelt read-only.
    /// </summary>
    public static DateTime DEFAULT_TIME = new DateTime(2018, 1, 1, 7, 30, 0);
}
