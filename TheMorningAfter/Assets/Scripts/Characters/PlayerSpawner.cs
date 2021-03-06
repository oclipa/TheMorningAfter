﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Does what it says on the tin: spawns a new player (at the specified position)
/// </summary>
public class PlayerSpawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
        EventManager.AddPlayerDiedListener(SpawnNewPlayer);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void SpawnNewPlayer(Vector3 position)
    {
        GameObject player = Object.Instantiate(Resources.Load("Objects/" + GameConstants.PLAYER)) as GameObject;
        player.transform.position = position;
    }
}
