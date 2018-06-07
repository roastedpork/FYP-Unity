using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : RosComponent {

    private RosPublisher<ros.geometry_msgs.PoseArray> waypointPub;
    private ros.geometry_msgs.PoseArray buffer;
    private bool AddingMultipleWaypoints = false;

    public GameObject DirectionArrowPrefab;
    private List<GameObject> arrows;


	// Use this for initialization
	void Start () {
        arrows = new List<GameObject>();
        buffer = new ros.geometry_msgs.PoseArray();

        Advertise("WaypointPub", "/hololens/navigation/waypoints", 5, out waypointPub);
        
        StartCoroutine(WaitForSpeechInit());
    }

    private IEnumerator WaitForSpeechInit()
    {
        yield return new WaitUntil(() => RosGazeManager.Instance != null && RosUserSpeechManager.Instance != null);


        RosUserSpeechManager.Instance.AddNewPhrase("Move here", () => 
        {
            if (AddingMultipleWaypoints)
            {
                RosUserSpeechManager.Instance.voicebox.StartSpeaking("Removing previous set of waypoints");
                AddingMultipleWaypoints = false;
            }

            buffer = new ros.geometry_msgs.PoseArray();
            buffer.header.frame_id = "/Unity";

            if (RosGazeManager.Instance.Focused)
            {
                Quaternion camRot = Camera.main.transform.rotation * Quaternion.Euler(0, -90, 0);
                Quaternion pointRot = Quaternion.Euler(0, camRot.eulerAngles.y, 0);
                
                ros.geometry_msgs.Pose newPoint = new ros.geometry_msgs.Pose(RosGazeManager.Instance.position, pointRot);
                buffer.poses.Add(newPoint);
                Publish(waypointPub, buffer);
                RosUserSpeechManager.Instance.voicebox.StartSpeaking("Moving to new location");

            }
            else
            {
                RosUserSpeechManager.Instance.voicebox.StartSpeaking("Unable to set a waypoint, please scan the room first");
            }

            
        });

        RosUserSpeechManager.Instance.AddNewPhrase("Add point", () =>
        {
            if (!AddingMultipleWaypoints)
            {
                buffer = new ros.geometry_msgs.PoseArray();
                buffer.header.frame_id = "Unity";
                AddingMultipleWaypoints = true;
            }
            
            if (RosGazeManager.Instance.Focused)
            {
                Quaternion pointRot;
                if (buffer.poses.Count > 0)
                {
                    Vector3 prevPoint = buffer.poses[buffer.poses.Count - 1].position.AsUnityVector;
                    Vector3 delta = (RosGazeManager.Instance.position - prevPoint).normalized;

                    float angle = -(Mathf.Rad2Deg * Mathf.Atan2(delta.z, delta.x));
                    pointRot = Quaternion.Euler(0, angle, 0);
                }
                else
                {
                    Quaternion camRot = Camera.main.transform.rotation * Quaternion.Euler(0, -90, 0);
                    pointRot = Quaternion.Euler(0, camRot.eulerAngles.y, 0);
                }

                ros.geometry_msgs.Pose newPoint = new ros.geometry_msgs.Pose(RosGazeManager.Instance.position, pointRot);
                buffer.poses.Add(newPoint);
                RosUserSpeechManager.Instance.voicebox.StartSpeaking("New waypoint set");
            }
             else
            {
                RosUserSpeechManager.Instance.voicebox.StartSpeaking("Unable to set a waypoint");
            }

        });

        RosUserSpeechManager.Instance.AddNewPhrase("start moving", () =>
        {
            if (AddingMultipleWaypoints)
            {
                Publish(waypointPub, buffer);
                AddingMultipleWaypoints = false;
                RosUserSpeechManager.Instance.voicebox.StartSpeaking("Moving along specified path");
            }
        });
        
    }

    
    void Update () {

        if (TrackingManager.Instance.ContinuousTracking)
        {
            buffer.poses.Clear();
        }


        int max = (buffer.poses.Count > arrows.Count) ? buffer.poses.Count : arrows.Count;

        for (int i = 1; i <= max; i++)
        {
            if (i > arrows.Count)
            {
                arrows.Add(Instantiate(DirectionArrowPrefab));
            }
        }


        for (int i=0; i<max; i++)
        {
            GameObject arrow = arrows[i];
            if(i < buffer.poses.Count)
            {
                arrow.SetActive(true);
                arrow.transform.SetPositionAndRotation(buffer.poses[i].position.AsUnityVector, buffer.poses[i].orientation.AsUnityQuaternion);
            }
            else
            {
                arrow.SetActive(false);
            }

        }


	}
}
