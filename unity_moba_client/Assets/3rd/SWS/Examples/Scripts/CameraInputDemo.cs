/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SWS;

/// <summary>
/// Example: user input script which moves through waypoints one by one.
/// <summary>
public class CameraInputDemo : MonoBehaviour
{
    /// <summary>
    /// Information text per waypoint, set at start and via messages.
    /// <summary>
    public string infoText = "Welcome to this customized input example";

    //reference to the movement script
    private splineMove myMove;


    //get references at start
    //initialize movement but don't start it yet
    void Start()
    {
        myMove = gameObject.GetComponent<splineMove>();
        myMove.StartMove();
        myMove.Pause();
    }
      

    //listens to user input
    void Update()
    {
        //do nothing in moving state
        if (myMove.tween == null || myMove.tween.IsPlaying())
            return;

        //on up arrow, move forwards
        if (Input.GetKeyDown(KeyCode.UpArrow))
            myMove.Resume();
    }


    //display GUI stuff on screen
    void OnGUI()
    {
        //do nothing in moving state
        if (myMove.tween != null && myMove.tween.IsPlaying())
            return;

        //draw top right box with info text received from messages
        GUI.Box(new Rect(Screen.width - 150, Screen.height / 2, 150, 100), "");
        Rect infoPos = new Rect(Screen.width - 130, Screen.height / 2 + 10, 110, 90);
        GUI.Label(infoPos, infoText);
    }


    /// <summary>
    /// Receives text from messages.
    /// <summary>
    public void ShowInformation(string text)
    {
        infoText = text;
    }
}