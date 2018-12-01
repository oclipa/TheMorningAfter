using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants {

    public static int TotalTreasures = 0; // populated by Loader
    public const int MaxLives = 6;

    public const string RIGHT = "right";
    public const string LEFT = "left";
    public const string SPACE = "space";
    public const string HORIZONTAL = "Horizontal";
    public const string VERTICAL = "Vertical";
    public const string UP = "Up";
    public const string DOWN = "Down";
    public const string STATIONARY = "Stationary";
    public const string FORWARD = "forward";
    public const string BACKWARD = "backward";

    public const string GAMETITLETEXT = "GameTitleText";
    public const string ROOMTITLECANVAS = "RoomTitleCanvas";
    public const string ROOMTITLETEXT = "RoomTitleText";
    public const string SCOREBOARD = "ScoreBoard";
    public const string SCOREBOARDTITLEPANEL = "ScoreBoardTitlePanel";
    public const string SCOREBOARDSTATUSPANEL = "ScoreBoardStatusPanel";
    public const string FINALSTATETEXT = "FinalStateText";
    public const string FINALSCORETEXT = "FinalScoreText";
    public const string ITEMSCOLLECTEDTEXT = "ItemsCollectedText";
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
    public const string RAMP = "Ramp";
    public const string ROPE_ANCHOR = "RopeAnchor";
    public const string ROPE_SECTION = "RopeSection";
    public const string ITEM = "Item";
    public const string DOOR = "Door";
    public const string MARIA = "Maria";
    public const string BED = "Bed";

    public const string MAINMENU = "MainMenu";
    public const string MAINMENUCANVAS = "MainMenuCanvas";

    public const float PLAYER_WALK_SPEED = 2f;
    public const float PLAYER_CLIMB_SPEED = 2f;
    public const float PLAYER_JUMP_SPEED_X = 25f;
    public const float PLAYER_JUMP_SPEED_Y = 140f; //140f; // 275f

    public const float OBSTACLE_WALK_SPEED_X = 2f;
    public const float OBSTACLE_WALK_SPEED_Y = 1.2f;

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
}
