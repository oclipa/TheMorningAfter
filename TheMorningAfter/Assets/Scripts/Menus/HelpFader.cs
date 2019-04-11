using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fades a help sprite over a period of time.
/// </summary>
public class HelpFader : MonoBehaviour {

    SpriteRenderer sr;
    GameState gameState;

    bool ignoreHideHelp;

    // Use this for initialization
    void Start () 
    {
        gameState = Camera.main.GetComponent<GameState>();
        sr = GetComponent<SpriteRenderer>();

        // initial help is only shown in TheBathroom
        if (gameState.HelpShown || !gameState.CurrentRoom.Equals(GameConstants.DefaultRoom))
        {
            // if we are no longer on TheBathroom, assume help has been shown already
            gameState.HelpShown = true;

            HideHelp();
        }
        else
        {
            GameObject parent = transform.parent.gameObject;
            // don't show help on all scenery obstacles - only the bath (and non-scenery obstacles)
            if (parent.GetComponent<SceneryObstacleController>() == null || parent.name.Equals("Bath"))
            {
                sr.color = Color.white;
            }
            else
            {
                HideHelp();
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate () 
    {
        FadeHelp();
    }

    private void FadeHelp()
    {
        Color color = sr.color;
        if (color.a > 0f)
        {
            color.a -= 0.001f; // bigger number, faster fade
            sr.color = color;

            if (color.a <= 0f)
                gameState.HelpShown = true;
        }
    }

    private void HideHelp()
    {
        if (!ignoreHideHelp)
        {
            Color color = Color.white;
            color.a = 0f; // transparent
            sr.color = color;

        }
    }

    public void ShowHelp()
    {
        sr = GetComponent<SpriteRenderer>();
        gameState = Camera.main.GetComponent<GameState>();

        // If this method is called from TheLimitingFactor, this means
        // the weapon help has been requested to be displayed.
        if (gameState.CurrentRoom.Equals("TheLimitingFactor"))
        {
            sr.color = Color.white;

            // the help will only be shown when this method is called,
            // so we can ignore the HideHelp method.
            ignoreHideHelp = true;
        }
    }
}
