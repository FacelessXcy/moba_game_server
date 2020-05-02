/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;


namespace SWS
{
    /// <summary>
    /// Movement script: linear or curved splines.
    /// <summary>
    [AddComponentMenu("Simple Waypoint System/splineMove")]
    public class splineMove : MonoBehaviour
    {
        /// <summary>
        /// Path component to use for movement.
        /// <summary>
        public PathManager pathContainer;

        /// <summary>
        /// Whether this object should start its movement at game launch.
        /// <summary>
        public bool onStart = false;

        /// <summary>
        /// Whether this object should walk to the first waypoint or spawn there.
        /// <summary>
        public bool moveToPath = false;

        /// <summary>
        /// reverse the movement direction on the path, typically used for "pingPong" behavior.
        /// <summary>
        public bool reverse = false;

        /// <summary>
        /// Waypoint index where this object should start its path.
        /// <summary>
        public int startPoint = 0;

        /// <summary>
        /// Current waypoint indicator on the path. 
        /// <summary>
        [HideInInspector]
        public int currentPoint = 0;

        /// <summary>
        /// Option for closing the path on the "loop" looptype.
        /// <summary>
        public bool closeLoop = false;

        /// <summary>
        /// Whether local positioning should be used when tweening this object.
        /// <summary>
        public bool local = false;

        /// <summary>
        /// Value to look ahead on the path when orientToPath is enabled (0-1).
        /// <summary>
        public float lookAhead = 0;

        /// <summary>
        /// Additional units to add on the y-axis.
        /// <summary>
        public float sizeToAdd = 0;

        /// <summary>
        /// Selection for speed-based movement or time in seconds per segment. 
        /// <summary>
        public enum TimeValue
        {
            time,
            speed
        }
        public TimeValue timeValue = TimeValue.speed;

        /// <summary>
        /// Speed or time value depending on the selected TimeValue type.
        /// <summary>
        public float speed = 5;
        //original speed when changing the tween's speed
        private float originSpeed;

        /// <summary>
        /// Custom curve when AnimationCurve has been selected as easeType.
        /// <summary>
        public AnimationCurve animEaseType;

        /// <summary>
        /// Supported movement looptypes when moving on the path. 
        /// <summary>
        public enum LoopType
        {
            none,
            loop,
            pingPong,
            random,
            yoyo
        }
        public LoopType loopType = LoopType.none;

        /// <summary>
        /// Waypoint array references of the requested path.
        /// <summary>
        [HideInInspector]
        public Vector3[] waypoints;
        //array of modified waypoint positions for the tween
        private Vector3[] wpPos;

        /// <summary>
        /// List of Unity Events invoked when reaching waypoints.
        /// <summary>
        [HideInInspector]
        public List<UnityEvent> events = new List<UnityEvent>();

        /// <summary>
        /// Animation path type, linear or curved.
        /// <summary>
        public DG.Tweening.PathType pathType = DG.Tweening.PathType.CatmullRom;

        /// <summary>
        /// Whether this object should orient itself to a different Unity axis
        /// <summary>
        public DG.Tweening.PathMode pathMode = DG.Tweening.PathMode.Full3D;

        /// <summary>
        /// Animation easetype on TimeValue type time.
        /// <summary>
        public DG.Tweening.Ease easeType = DG.Tweening.Ease.Linear;

        /// <summary>
        /// Option for locking a position axis.
        /// <summary>
        public DG.Tweening.AxisConstraint lockPosition = DG.Tweening.AxisConstraint.None;

        /// <summary>
        /// Option for locking a rotation axis with orientToPath enabled.
        /// <summary>
        public DG.Tweening.AxisConstraint lockRotation = DG.Tweening.AxisConstraint.None;

        //---DOTween animation helper variables---
        [HideInInspector]
        public Tweener tween;
        //looptype random generator
        private System.Random rand = new System.Random();
        //looptype random waypoint index array
        private int[] rndArray;


        //check for automatic initialization
        void Start()
        {
            if (onStart)
                StartMove();
        }


        /// <summary>
        /// Starts movement. Can be called from other scripts to allow start delay.
        /// <summary>
        public void StartMove()
        {
            //don't continue without path container
            if (pathContainer == null)
            {
                Debug.LogWarning(gameObject.name + " has no path! Please set Path Container.");
                return;
            }

            //get array with waypoint positions
            waypoints = pathContainer.GetPathPoints(local);
            //cache original speed for future speed changes
            originSpeed = speed;
            
            //initialize waypoint positions
            startPoint = Mathf.Clamp(startPoint, 0, waypoints.Length - 1);
            int index = startPoint;
            if (reverse)
            {
                Array.Reverse(waypoints);
                index = waypoints.Length - 1 - index;
            }
            Initialize(index);

            Stop();
            CreateTween();
        }


        //initialize or update modified waypoint positions
        //fills array with original positions and adds custom height
        //check for message count and reinitialize if necessary
        private void Initialize(int startAt = 0)
        {
            if (!moveToPath) startAt = 0;
            wpPos = new Vector3[waypoints.Length - startAt];
            for (int i = 0; i < wpPos.Length; i++)
                wpPos[i] = waypoints[i + startAt] + new Vector3(0, sizeToAdd, 0);

            //message count is smaller than waypoint count,
            //add empty message per waypoint
            for (int i = events.Count; i <= pathContainer.GetEventsCount() - 1; i++)
                events.Add(new UnityEvent());
        }


        //creates a new tween with given arguments that moves along the path
        private void CreateTween()
        {
            //prepare DOTween's parameters, you can look them up here
            //http://dotween.demigiant.com/documentation.php

            TweenParams parms = new TweenParams();
            //differ between speed or time based tweening
            if (timeValue == TimeValue.speed)
                parms.SetSpeedBased();
            if (loopType == LoopType.yoyo)
                parms.SetLoops(-1, DG.Tweening.LoopType.Yoyo);

            //apply ease type or animation curve
            if (easeType == DG.Tweening.Ease.Unset)
                parms.SetEase(animEaseType);
            else
                parms.SetEase(easeType);

            if (moveToPath)
                parms.OnWaypointChange(OnWaypointReached);
            else
            {
                //on looptype random initialize random order of waypoints
                if (loopType == LoopType.random)
                    RandomizeWaypoints();
                else if (loopType == LoopType.yoyo)
                    parms.OnStepComplete(ReachedEnd);

                Vector3 startPos = wpPos[0];
                if (local) startPos = pathContainer.transform.TransformPoint(startPos);
                transform.position = startPos;

                parms.OnWaypointChange(OnWaypointChange);
                parms.OnComplete(ReachedEnd);
            }

            if (local)
            {
                tween = transform.DOLocalPath(wpPos, originSpeed, pathType, pathMode)
                                 .SetAs(parms)
                                 .SetOptions(closeLoop, lockPosition, lockRotation)
                                 .SetLookAt(lookAhead);
            }
            else
            {
                tween = transform.DOPath(wpPos, originSpeed, pathType, pathMode)
                                 .SetAs(parms)
                                 .SetOptions(closeLoop, lockPosition, lockRotation)
                                 .SetLookAt(lookAhead);
            }

            if (!moveToPath && startPoint > 0)
            {
                GoToWaypoint(startPoint);
                startPoint = 0;
            }

            //continue new tween with adjusted speed if it was changed before
            if (originSpeed != speed)
                ChangeSpeed(speed);
        }


        //called when moveToPath completes
        private void OnWaypointReached(int index)
        {
            if (index <= 0) return;

            Stop();
            moveToPath = false;
            Initialize();
            CreateTween();
        }


        //called at every waypoint to invoke events
        private void OnWaypointChange(int index)
		{
			index = pathContainer.GetWaypointIndex(index);
			if (index == -1) return;
            if (loopType != LoopType.yoyo && reverse)
                index = waypoints.Length - 1 - index;
            if (loopType == LoopType.random)
                index = rndArray[index];

            currentPoint = index;

            if (events == null || events.Count - 1 < index || events[index] == null
                || loopType == LoopType.random && index == rndArray[rndArray.Length - 1])
                return;

            events[index].Invoke();
        }


        private void ReachedEnd()
        {
            //each looptype has specific properties
            switch (loopType)
            {
                //none means the tween ends here
                case LoopType.none:
                    return;

                //in a loop we start from the beginning
                case LoopType.loop:
                    currentPoint = 0;
                    CreateTween();
                    break;

                //reversing waypoints then moving again
                case LoopType.pingPong:
                    reverse = !reverse;
                    Array.Reverse(waypoints);
                    Initialize();

                    CreateTween();
                    break;

                //indicate backwards direction
                case LoopType.yoyo:
                    reverse = !reverse;
                    break;

                //randomize waypoints to new order
                case LoopType.random:
                    RandomizeWaypoints();
                    CreateTween();
                    break;
            }
        }


        private void RandomizeWaypoints()
        {
            Initialize();
            //create array with ongoing index numbers to keep them in mind,
            //this gets shuffled with all waypoint positions at the next step 
            rndArray = new int[wpPos.Length];
            for (int i = 0; i < rndArray.Length; i++)
            {
                rndArray[i] = i;
            }

            //get total array length
            int n = wpPos.Length;
            //shuffle wpPos and rndArray
            while (n > 1)
            {
                int k = rand.Next(n--);
                Vector3 temp = wpPos[n];
                wpPos[n] = wpPos[k];
                wpPos[k] = temp;

                int tmpI = rndArray[n];
                rndArray[n] = rndArray[k];
                rndArray[k] = tmpI;
            }

            //since all waypoints are shuffled the first waypoint does not
            //correspond with the actual current position, so we have to
            //swap the first waypoint with the actual waypoint.
            //start by caching the first waypoint position and number
            Vector3 first = wpPos[0];
            int rndFirst = rndArray[0];
            //loop through wpPos array and find corresponding waypoint
            for (int i = 0; i < wpPos.Length; i++)
            {
                //currentPoint is equal to this waypoint number
                if (rndArray[i] == currentPoint)
                {
                    //swap rnd index number and waypoint positions
                    rndArray[i] = rndFirst;
                    wpPos[0] = wpPos[i];
                    wpPos[i] = first;
                }
            }
            //set current rnd index number to the actual current point
            rndArray[0] = currentPoint;
        }


        /// <summary>
        /// Teleports to the defined waypoint index on the path.
        /// </summary>
        public void GoToWaypoint(int index)
        {
            if (tween == null)
                return;

            if (reverse) index = waypoints.Length - 1 - index;

            tween.ForceInit();
            tween.GotoWaypoint(index, true);
        }


        /// <summary>
        /// Pauses the current movement routine for a defined amount of time.
        /// <summary>
        public void Pause(float seconds = 0f)
        {
            StopCoroutine(Wait());
            if (tween != null)
                tween.Pause();

            if (seconds > 0)
                StartCoroutine(Wait(seconds));
        }


        //waiting routine
        private IEnumerator Wait(float secs = 0f)
        {
            yield return new WaitForSeconds(secs);
            Resume();
        }


        /// <summary>
        /// Resumes the currently paused movement routine.
        /// <summary>
        public void Resume()
        {
			StopCoroutine(Wait());
            if (tween != null)
                tween.Play();
        }


        /// <summary>
        /// Reverses movement at any time.
        /// <summary>
        public void Reverse()
        {
            //inverse direction toggle
            reverse = !reverse;
            //calculate opposite remaining path time i.e. if we're at 80% progress in one direction,
            //this then returns 20% time value when starting from the opposite direction and so on
            float timeRemaining = 0f;
            if(tween != null)
                timeRemaining = tween.Duration(false) - tween.Elapsed(false);

            //invert starting point from current waypoint
            startPoint = waypoints.Length - 1 - currentPoint;
            StartMove();
            tween.ForceInit();
            //set moving object to the reversed time progress
            tween.fullPosition = timeRemaining;
        }


        /// <summary>
        /// Changes the current path of this object and starts movement.
        /// <summary>
        public void SetPath(PathManager newPath)
        {
            //disable any running movement methods
            Stop();
            //set new path container
            pathContainer = newPath;
            //restart movement
            StartMove();
        }


        /// <summary>
        /// Disables any running movement routines.
        /// <summary>
        public void Stop()
        {
            StopAllCoroutines();

            if (tween != null)
                tween.Kill();
            tween = null;
        }


        /// <summary>
        /// Stops movement and resets to the first waypoint.
        /// <summary>
        public void ResetToStart()
        {
            Stop();
            currentPoint = 0;
            if (pathContainer)
            {
                transform.position = pathContainer.waypoints[currentPoint].position + new Vector3(0, sizeToAdd, 0);
            }
        }


        /// <summary>
        /// Change running tween speed by manipulating its timeScale.
        /// <summary>
        public void ChangeSpeed(float value)
        {
            //calulate new timeScale value based on original speed
            float newValue;
            if (timeValue == TimeValue.speed)
                newValue = value / originSpeed;
            else
                newValue = originSpeed / value;

            //set speed, change timeScale percentually
            speed = value;
            if (tween != null)
                tween.timeScale = newValue;
        }
    }
}