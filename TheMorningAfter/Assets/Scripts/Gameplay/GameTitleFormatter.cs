
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Cycles the game title through a range of colours
/// </summary>
public class GameTitleFormatter : MonoBehaviour {

    Text gameTitleText;
    public float Speed = 1;

    // Use this for initialization
    void Start () {
        gameTitleText = GameObject.FindGameObjectWithTag(GameConstants.GAMETITLETEXT).GetComponent<Text>();
	}

    void FixedUpdate()
    {

        float time = Time.time;

        //Color32 textColor32 = new Color32(
         //(byte)Mathf.Abs(255 * Mathf.Sin(time)),        // R
         //(byte)Mathf.Abs(255 * Mathf.Sin(time)),        // G
         //(byte)Mathf.Abs(255 * Mathf.Sin(time)),        // B
         //(byte)255);                                    // A

        Color color = HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed, 1), 1, 1));

        gameTitleText.color = color;
    }
}
