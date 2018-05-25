using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : RosComponent {

    private RosPublisher<ros.geometry_msgs.PoseArray> pub;
    private ros.geometry_msgs.PoseArray buffer;
    public bool AddingMultipleWaypoints { get; private set; }

    private GazeGestureManager gazeManager;

	// Use this for initialization
	void Start () {
        
        bool stat = Advertise("WaypointPub", "/hololens/waypoints", 5, out pub);

        Debug.Log("Waypoint publisher exists: " +  stat.ToString());

        AddingMultipleWaypoints = false;
        gazeManager = GazeGestureManager.Instance;
    }

    public void SingleWaypoint()
    {
        buffer = new ros.geometry_msgs.PoseArray();
        buffer.header.frame_id = "/hololens";

        if (gazeManager.Focused)
        {
            ros.geometry_msgs.Pose newPoint = new ros.geometry_msgs.Pose(gazeManager.position, Quaternion.identity);
            buffer.poses.Add(newPoint);
        }

        PublishWaypoints();
    }

    public bool AddWaypoint()
    {
        if (!AddingMultipleWaypoints)
        {
            AddingMultipleWaypoints = true;
            buffer = new ros.geometry_msgs.PoseArray();
            buffer.header.frame_id = "/hololens";
        }
        
        if (gazeManager.Focused)
        {
            ros.geometry_msgs.Pose newPoint = new ros.geometry_msgs.Pose(gazeManager.position, Quaternion.identity);
            buffer.poses.Add(newPoint);

            return true;
        }

        if (buffer.poses.Count == 0)
        {
            AddingMultipleWaypoints = false;
        }
        return false;

    }

    public void PublishWaypoints()
    {
        if (buffer != null) Publish(pub, buffer);
        buffer = null;
        AddingMultipleWaypoints = false;
    }

    void Update () {
		
	}
}
