/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using System.Collections;
using DG.Tweening;
using SWS;

/// <summary>
/// Example: object controlled by user input, speed decreases when not pressing a key.
/// <summary>
public class RapidInputDemo : MonoBehaviour
{
    /// <summary>
    /// Reference to the text mesh for displaying speed values.
    /// <summary>
    public TextMesh speedDisplay;

    /// <summary>
    /// Reference to the text mesh for displaying time values.
    /// <summary>
    public TextMesh timeDisplay;

    /// <summary>
    /// Maximum speed value that could be reached.
    /// <summary>
    public float topSpeed = 15;

    /// <summary>
    /// Speed value to add per keypress.
    /// <summary>
    public float addSpeed = 2;

    /// <summary>
    /// Delay before slowing down.
    /// <summary>
    public float delay = 0.05f;

    /// <summary>
    /// Time for slowing down to zero speed.
    /// <summary>
    public float slowTime = 0.5f;

    /// <summary>
    /// Minimum audio pitch when idling.
    /// <summary>
    public float minPitch = 0;

    /// <summary>
    /// Maximum audio pitch at max speed.
    /// <summary>
    public float maxPitch = 2;

    //reference to movement script
    private splineMove move;
    //current speed
    private float currentSpeed;
    //time passed since start
    private float timeCounter = 0f;


    //get references
    void Start()
    {
        move = GetComponent<splineMove>();
        if (!move)
        {
            Debug.LogWarning(gameObject.name + " missing movement script!");
            return;
        }

        //set speed to an arbitrary small value
        //otherwise the tween can't be initialized
        move.speed = 0.01f;
        //initialize movement but don't start it yet
        move.StartMove();
        move.Pause();
        move.speed = 0f;
    }


    void Update()
    {
        //do not continue if the tween reached its end
        if (move.tween == null || !move.tween.IsActive() || move.tween.IsComplete())
            return;

        //check for user input
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //resume tween the first time the game starts
            if (!move.tween.IsPlaying()) move.Resume();

            //get desired speed after pressing the button
            //we add the desired value to the current speed for acceleration
            float speed = currentSpeed + addSpeed;
            //limit the speed value by the maximum value
            if (speed >= topSpeed) speed = topSpeed;

            //change the speed of the tween by the calculated value
            move.ChangeSpeed(speed);

            //restart slow down
            StopAllCoroutines();
            StartCoroutine("SlowDown");
        }

        //display values and increase timer
        speedDisplay.text = "YOUR SPEED: " + Mathf.Round(move.speed * 100f) / 100f;
        timeCounter += Time.deltaTime;
        timeDisplay.text = "YOUR TIME: " + Mathf.Round(timeCounter * 100f) / 100f;
    }


    //coroutine for slowing down the object
    IEnumerator SlowDown()
    {
        //wait desired delay before affecting speed
        yield return new WaitForSeconds(delay);

        //temp time value (0-1)
        float t = 0f;
        //time rate based on slowTime
        float rate = 1f / slowTime;
        //cache actual current speed
        float speed = move.speed;

        //slow down until slowTime is elapsed
        while (t < 1)
        {
            //increase time value over time
            t += Time.deltaTime * rate;
            //smoothly slow down speed value to zero over time
            //cache smoothed current speed value
            currentSpeed = Mathf.Lerp(speed, 0, t);
            //apply current speed to the tween
            move.ChangeSpeed(currentSpeed);

            //get pitch factor as difference between min and max pitch
            float pitchFactor = maxPitch - minPitch;
            //calculate pitch based on the current speed multiplied by the pitch factor
            float pitch = minPitch + (move.speed / topSpeed) * pitchFactor;
            //smooth pitch value over 0.2 seconds and assign it to the audio clip 
            if (GetComponent<AudioSource>()) GetComponent<AudioSource>().pitch = Mathf.SmoothStep(GetComponent<AudioSource>().pitch, pitch, 0.2f);

            //yield loop
            yield return null;
        }
    }
}
