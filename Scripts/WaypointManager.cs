using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : RosComponent {

    private RosPublisher<ros.geometry_msgs.PoseArray> pub;
    private ros.geometry_msgs.PoseArray buffer;
    public bool AddingMultipleWaypoints { get; private set; }


	// Use this for initialization
	void Start () {
        StartCoroutine(WaitUntilRosMessengerConnected("WaypointManager"));
        bool stat = Advertise("WaypointPub", "/hololens/waypoints", 5, out pub);

        Debug.Log("Waypoint publisher exists: " +  stat.ToString());

        AddingMultipleWaypoints = false;
    }

    public void SingleWaypoint()
    {
        buffer = new ros.geometry_msgs.PoseArray();
        buffer.header.frame_id = "/odom";

        RaycastHit hitInfo;
        if (Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                out hitInfo,
                20.0f,
                Physics.DefaultRaycastLayers))
        {
            ros.geometry_msgs.Pose newPoint = new ros.geometry_msgs.Pose(hitInfo.point, Quaternion.identity);
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
        }

        RaycastHit hitInfo;
        if (Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                out hitInfo,
                20.0f,
                Physics.DefaultRaycastLayers))
        {
            ros.geometry_msgs.Pose newPoint = new ros.geometry_msgs.Pose(hitInfo.point, Quaternion.identity);
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



    // Update is called once per frame
    void Update () {
		
	}
}
