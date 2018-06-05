using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : RosComponent {

    private RosPublisher<ros.geometry_msgs.PoseArray> waypointPub;
    private RosPublisher<ros.geometry_msgs.PoseStamped> trackingPub;
    private ros.geometry_msgs.PoseArray buffer;

    private bool AddingMultipleWaypoints = false;
    private bool ContinuousTracking = false;

	// Use this for initialization
	void Start () {
        
        Advertise("WaypointPub", "/hololens/waypoints", 5, out waypointPub);
        Advertise("WaypointPub", "/hololens/continous_tracking", 5, out trackingPub);
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
            buffer.header.frame_id = "Unity";

            if (RosGazeManager.Instance.Focused)
            {
                ros.geometry_msgs.Pose newPoint = new ros.geometry_msgs.Pose(RosGazeManager.Instance.position, Quaternion.identity);
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
                ros.geometry_msgs.Pose newPoint = new ros.geometry_msgs.Pose(RosGazeManager.Instance.position, Quaternion.identity);
                buffer.poses.Add(newPoint);
                RosUserSpeechManager.Instance.voicebox.StartSpeaking("New waypoint set");
            }
             else
            {
                RosUserSpeechManager.Instance.voicebox.StartSpeaking("Unable to set a waypoint");
            }

        });

        RosUserSpeechManager.Instance.AddNewPhrase("traverse path", () =>
        {
            if (AddingMultipleWaypoints)
            {
                Publish(waypointPub, buffer);
                AddingMultipleWaypoints = false;
                RosUserSpeechManager.Instance.voicebox.StartSpeaking("Moving along specified path");
            }
        });
        RosUserSpeechManager.Instance.AddNewPhrase("Track my gaze", () =>
        {
            ContinuousTracking = true;
            RosUserSpeechManager.Instance.voicebox.StartSpeaking("Gaze tracking starting");
        });
        RosUserSpeechManager.Instance.AddNewPhrase("End tracking", () =>
        {
            ContinuousTracking = false;
            RosUserSpeechManager.Instance.voicebox.StartSpeaking("Gaze tracking ended");
        });
    }

    
    void Update () {
		if (ContinuousTracking && RosGazeManager.Instance.Focused)
        {
            ros.geometry_msgs.PoseStamped msg = new ros.geometry_msgs.PoseStamped();
            msg.header.frame_id = "/Unity";

            msg.pose.position = new ros.geometry_msgs.Point(RosGazeManager.Instance.position);
        }
	}
}
