/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them directly or indirectly
 *  from Rebound Games. You shall not license, sublicense, sell, resell, transfer, assign,
 *  distribute or otherwise make available to any third party the Service or the Content. 
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SWS
{
    /// <summary>
    /// Custom path inspector.
    /// <summary>
    [CustomEditor(typeof(PathManager))]
    public class PathEditor : Editor
    {
        //define Serialized Objects we want to use/control
        //this will be our serialized reference to the inspected script
        private SerializedObject m_Object;
        //serialized waypoint array
        private SerializedProperty m_Waypoint;
        //serialized waypoint array count
        private SerializedProperty m_WaypointsCount;
        //serialized path gizmo property booleans
        private SerializedProperty m_Check1;
        private SerializedProperty m_Check2;
        //serialized scene view gizmo colors
        private SerializedProperty m_Color1;
        private SerializedProperty m_Color2;
        //serialized custom waypoint renaming bool
        private SerializedProperty m_SkipNames;
        //serialized replace object gameobject
        private SerializedProperty m_WaypointPref;

        //waypoint array size, define path to know where to lookup for this variable
        //(we expect an array, so it's "name_of_array.data_type.size")
        private static string wpArraySize = "waypoints.Array.size";
        //.data gives us the data of the array,
        //we replace this {0} token with an index we want to get
        private static string wpArrayData = "waypoints.Array.data[{0}]";


        //called whenever this inspector window is loaded 
        public void OnEnable()
        {
            //we create a reference to our script object by passing in the target
            m_Object = new SerializedObject(target);

            //from this object, we pull out the properties we want to use
            //these are just the names of our variables in the manager
            m_Check1 = m_Object.FindProperty("drawCurved");
            m_Check2 = m_Object.FindProperty("drawDirection");
            m_Color1 = m_Object.FindProperty("color1");
            m_Color2 = m_Object.FindProperty("color2");
            m_SkipNames = m_Object.FindProperty("skipCustomNames");
            m_WaypointPref = m_Object.FindProperty("replaceObject");

            //set serialized waypoint array count by passing in the path to our array size
            m_WaypointsCount = m_Object.FindProperty(wpArraySize);
        }


        private Transform[] GetWaypointArray()
        {
            //get array count from serialized property and store its int value into var arrayCount
            var arrayCount = m_Object.FindProperty(wpArraySize).intValue;
            //create new waypoint transform array with size of arrayCount
            var transformArray = new Transform[arrayCount];
            //loop over waypoints
            for (var i = 0; i < arrayCount; i++)
            {
                //for each one use "FindProperty" to get the associated object reference
                //of waypoints array, string.Format replaces {0} token with index i
                //and store the object reference value as type of transform in transformArray[i]
                transformArray[i] = m_Object.FindProperty(string.Format(wpArrayData, i)).objectReferenceValue as Transform;
            }
            //finally return that array copy for modification purposes
            return transformArray;
        }


        //similiar to GetWaypointArray(), find serialized property which belongs to index
        //and set this value to parameter transform "waypoint" directly
        private void SetWaypoint(int index, Transform waypoint)
        {
            m_Object.FindProperty(string.Format(wpArrayData, index)).objectReferenceValue = waypoint;
        }


        //similiar to SetWaypoint(), this will find the waypoint from array at index position
        //and returns it instead of modifying
        private Transform GetWaypointAtIndex(int index)
        {
            return m_Object.FindProperty(string.Format(wpArrayData, index)).objectReferenceValue as Transform;
        }


        //get the corresponding waypoint and destroy the whole gameobject in editor
        private void RemoveWaypointAtIndex(int index)
        {
            Undo.DestroyObjectImmediate(GetWaypointAtIndex(index).gameObject);

            //iterate over the array, starting at index,
            //and replace it with the next one
            for (int i = index; i < m_WaypointsCount.intValue - 1; i++)
                SetWaypoint(i, GetWaypointAtIndex(i + 1));

            //decrement array count by 1
            m_WaypointsCount.intValue--;
        }


        private void AddWaypointAtIndex(int index)
        {
            //increment array count so the waypoint array is one unit larger
            m_WaypointsCount.intValue++;

            //backwards loop through array:
            //since we're adding a new waypoint for example in the middle of the array,
            //we need to push all existing waypoints after that selected waypoint
            //1 slot upwards to have one free slot in the middle. So:
            //we're doing exactly that and start looping at the end downwards to the selected slot
            for (int i = m_WaypointsCount.intValue - 1; i > index; i--)
                SetWaypoint(i, GetWaypointAtIndex(i - 1));

            //create new waypoint gameobject
            GameObject wp = new GameObject("Waypoint " + (index + 1));
            Undo.RegisterCreatedObjectUndo(wp, "Created WP");

            //set its position to the last one
            wp.transform.position = GetWaypointAtIndex(index).position;
            //parent it to the path gameobject
            wp.transform.parent = GetWaypointAtIndex(index).parent;
            //finally, set this new waypoint after the one clicked in waypoints array
            SetWaypoint(index + 1, wp.transform);
        }


        //called whenever the inspector gui gets rendered
        public override void OnInspectorGUI()
        {
            //don't draw inspector fields if the path contains less than 2 points
            //(a path with less than 2 points really isn't a path)
            if (m_WaypointsCount.intValue < 2)
                return;

            //this pulls the relative variables from unity runtime and stores them in the object
            m_Object.Update();

            //create new checkboxes for path gizmo property 
            m_Check1.boolValue = EditorGUILayout.Toggle("Draw Smooth Lines", m_Check1.boolValue);
            //m_Check2.boolValue = EditorGUILayout.Toggle("Draw Direction Handles", m_Check2.boolValue);

            //create new property fields for editing waypoint gizmo colors 
            EditorGUILayout.PropertyField(m_Color1);
            EditorGUILayout.PropertyField(m_Color2);

            //get waypoint array
            var waypoints = GetWaypointArray();

            //calculate path length of all waypoints
            Vector3[] wpPositions = new Vector3[waypoints.Length];
            for (int i = 0; i < waypoints.Length; i++)
                wpPositions[i] = waypoints[i].position;
            float pathLength = WaypointManager.GetPathLength(wpPositions);
            //path length label, show calculated path length
            GUILayout.Label("Path Length: " + pathLength);

            //waypoint index header
            GUILayout.Label("Waypoints: ", EditorStyles.boldLabel);

            //loop through the waypoint array
            for (int i = 0; i < waypoints.Length; i++)
            {
                GUILayout.BeginHorizontal();
                //indicate each array slot with index number in front of it
                GUILayout.Label(i + ".", GUILayout.Width(20));
                //create an object field for every waypoint
                EditorGUILayout.ObjectField(waypoints[i], typeof(Transform), true);

                //display an "Add Waypoint" button for every array row except the last one
                if (i < waypoints.Length && GUILayout.Button("+", GUILayout.Width(30f)))
                {
                    AddWaypointAtIndex(i);
                    break;
                }

                //display an "Remove Waypoint" button for every array row except the first and last one
                if (i > 0 && i < waypoints.Length - 1 && GUILayout.Button("-", GUILayout.Width(30f)))
                {
                    RemoveWaypointAtIndex(i);
                    break;
                }

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            //button to rename waypoints to current index order
            if (GUILayout.Button("Rename Waypoints"))
            {
                //disabled because of a Unity bug that crashes the editor
                //this is taken directly from the docs, thank you Unity.
                //http://docs.unity3d.com/ScriptReference/Undo.RegisterCompleteObjectUndo.html
                //Undo.RegisterCompleteObjectUndo(waypoints[0].gameObject, "Rename Waypoints");

                string wpName = string.Empty;
                string[] nameSplit;

                for (int i = 0; i < waypoints.Length; i++)
                {
                    //cache name and split into strings
                    wpName = waypoints[i].name;
                    nameSplit = wpName.Split(' ');

                    //ignore custom names and just rename
                    if(!m_SkipNames.boolValue)
                        wpName = "Waypoint " + i;
                    else if (nameSplit.Length == 2 && nameSplit[0] == "Waypoint")
                    {
                        //try parsing the current index and rename,
                        //not ignoring custom names here
                        int index;
                        if (int.TryParse(nameSplit[1], out index))
                        {
                            wpName = nameSplit[0] + " " + i;
                        }
                    }

                    //set the desired index or leave it
                    waypoints[i].name = wpName;
                }
            }

            EditorGUILayout.LabelField("Skip Custom", GUILayout.Width(80));
            m_SkipNames.boolValue = EditorGUILayout.Toggle(m_SkipNames.boolValue, GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();

            //button to move all waypoints down to the ground
            if (GUILayout.Button("Place to Ground"))
            {
                //for each waypoint of this path
                foreach (Transform trans in waypoints)
                {
                    //define ray to cast downwards waypoint position
                    Ray ray = new Ray(trans.position + new Vector3(0, 2f, 0), -Vector3.up);
                    Undo.RecordObject(trans, "Place To Ground");

                    RaycastHit hit;
                    //cast ray against ground, if it hit:
                    if (Physics.Raycast(ray, out hit, 100))
                    {
                        //position y values of waypoint to hit point
                        trans.position = hit.point;
                    }

                    //also try to raycast against 2D colliders
                    RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, -Vector2.up, 100);
                    if (hit2D)
                    {
                        trans.position = new Vector3(hit2D.point.x, hit2D.point.y, trans.position.z);
                    }
                }
            }

            //invert direction of whole path
            if (GUILayout.Button("Invert Direction"))
            {
                Undo.RecordObjects(waypoints, "Invert Direction");

                //to reverse the whole path we need to know where the waypoints were before
                //for this purpose a new copy must be created
                Vector3[] waypointCopy = new Vector3[waypoints.Length];
                for (int i = 0; i < waypoints.Length; i++)
                    waypointCopy[i] = waypoints[i].position;

                //looping over the array in reversed order
                for (int i = 0; i < waypoints.Length; i++)
                    waypoints[i].position = waypointCopy[waypointCopy.Length - 1 - i];
            }

            EditorGUILayout.Space();

            //draw object field for waypoint prefab
            EditorGUILayout.PropertyField(m_WaypointPref);

            //replace all waypoints with the prefab
            if (GUILayout.Button("Replace Waypoints with Object"))
            {
                if (m_WaypointPref == null)
                {
                    Debug.LogWarning("No replace object set. Cancelling.");
                    return;
                }

                ReplaceWaypoints();
            }

            //we push our modified variables back to our serialized object
            m_Object.ApplyModifiedProperties();
        }


        private void ReplaceWaypoints()
        {
            //get prefab object and path transform
            var waypointPrefab = m_WaypointPref.objectReferenceValue as GameObject;
            var path = GetWaypointAtIndex(0).parent;

            if (waypointPrefab == null)
            {
                Debug.LogWarning("You haven't specified a replace object. Cancelling.");
                return;
            }

            //loop through waypoint array of this path
            for (int i = 0; i < m_WaypointsCount.intValue; i++)
            {
                //get current waypoint at index position
                Transform curWP = GetWaypointAtIndex(i);
                //instantiate new waypoint at old position
                Transform newCur = ((GameObject)Instantiate(waypointPrefab, curWP.position, Quaternion.identity)).transform;
                Undo.RegisterCreatedObjectUndo(newCur.gameObject, "New");

                //parent new waypoint to this path
                newCur.parent = path;
                //replace old waypoint at index
                SetWaypoint(i, newCur);

                //destroy old waypoint object
                Undo.DestroyObjectImmediate(curWP.gameObject);
            }
        }


        //if this path is selected, display small info boxes above all waypoint positions
        //also display handles for the waypoints
        void OnSceneGUI()
        {
            //again, get waypoint array
            var waypoints = GetWaypointArray();
            //do not execute further code if we have no waypoints defined
            //(just to make sure, practically this can not occur)
            if (waypoints.Length == 0) return;
            Vector3 wpPos = Vector3.zero;
            float size = 1f;

            //loop through waypoint array
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (!waypoints[i]) continue;
                wpPos = waypoints[i].position;
                size = HandleUtility.GetHandleSize(wpPos) * 0.4f;

                //do not draw waypoint header if too far away
                if (size < 3f)
                {
                    //begin 2D GUI block
                    Handles.BeginGUI();
                    //translate waypoint vector3 position in world space into a position on the screen
                    var guiPoint = HandleUtility.WorldToGUIPoint(wpPos);
                    //create rectangle with that positions and do some offset
                    var rect = new Rect(guiPoint.x - 50.0f, guiPoint.y - 40, 100, 20);
                    //draw box at position with current waypoint name
                    GUI.Box(rect, waypoints[i].name);
                    Handles.EndGUI(); //end GUI block
                }

                //draw handles per waypoint, clamp size
                Handles.color = m_Color2.colorValue;
                size = Mathf.Clamp(size, 0, 1.2f);
                Vector3 newPos = Handles.FreeMoveHandle(wpPos, Quaternion.identity,
                                 size, Vector3.zero, Handles.SphereCap);
                Handles.RadiusHandle(Quaternion.identity, wpPos, size / 2);

                if (wpPos != newPos)
                {
                    Undo.RecordObject(waypoints[i], "Move Handles");
                    waypoints[i].position = newPos;
                }
            }
            
            //waypoint direction handles drawing
            if(!m_Check2.boolValue) return;
            Vector3[] pathPoints = new Vector3[waypoints.Length];           
            for(int i = 0; i < pathPoints.Length; i++)
                pathPoints[i] = waypoints[i].position;
            
            //create list of path segments (list of Vector3 list)
            List<List<Vector3>> segments = new List<List<Vector3>>();
            int curIndex = 0;
            float lerpVal = 0f;
            
            //differ between linear and curved display
            switch(m_Check1.boolValue)
            {
                case true:
                    //convert waypoints to curved path points
                    pathPoints = WaypointManager.GetCurved(pathPoints);
                    for(int i = 0; i < waypoints.Length - 1; i++)
                    {
                        //loop over path points to find single segments
                        segments.Add(new List<Vector3>());
                        for(int j = curIndex; j < pathPoints.Length; j++)
                        {
                            //the segment ends here, continue with new segment
                            if(pathPoints[j] == waypoints[i+1].position)
                            {
                                curIndex = j;
                                break;
                            }
                    
                            //add path point to current segment
                            segments[i].Add(pathPoints[j]);
                        }
                    }
                    break;
                 
                case false:
                    //detail for arrows between waypoints
                    int lerpMax = 16;
                    //loop over waypoints to add intermediary points
                    for(int i = 0; i < waypoints.Length - 1; i++)
                    {
                        segments.Add(new List<Vector3>());
                        for(int j = 0; j < lerpMax; j++)
                        {
                            //linear lerp between waypoints to get additional points for drawing arrows at
                            segments[i].Add(Vector3.Lerp(pathPoints[i], pathPoints[i+1], j / (float)lerpMax));
                        }
                    }
                    break;
            }
            
            //loop over segments
            for(int i = 0; i < segments.Count; i++)
            {
                //loop over single positions on the segment
                for(int j = 0; j < segments[i].Count; j++)
                {
                    //get current lerp value for interpolating rotation
                    //draw arrow handle on current position with interpolated rotation
                    size = Mathf.Clamp(HandleUtility.GetHandleSize(segments[i][j]) * 0.4f, 0, 1.2f);
                    lerpVal = j / (float)segments[i].Count;
                    Handles.ArrowCap( 0, segments[i][j], Quaternion.Lerp(waypoints[i].rotation, waypoints[i+1].rotation, lerpVal) * Quaternion.Euler( 0, 90, 0 ), size);
                }
            }
        }
    }
}