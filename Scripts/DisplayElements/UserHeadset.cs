using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserHeadset : RosComponent {

    private Camera cam;

    private RosPublisher<ros.geometry_msgs.Pose> headPosePub;
    private RosPublisher<ros.geometry_msgs.Point> gazePointPub;
 
	// Use this for initialization
	void Start () {
        cam = Camera.main; //GameObject.FindGameObjectWithTag("MainCamera");

        if (cam != null)
        {
            Debug.Log("[UserHeadset] Camera found");
            transform.parent = cam.transform;
            transform.localRotation = Quaternion.Euler(0, -90, 0);
        }
        else Debug.Log("[UserHeadset] Camera could not be found");

        Advertise("headPosePub", "/hololens/head_pose", 2, out headPosePub);
        Advertise("gazePosePub", "/hololens/gaze_point", 2, out gazePointPub);

    }
	
	// Update is called once per frame
	void Update () {    
        ros.geometry_msgs.Pose headPose = new ros.geometry_msgs.Pose(transform.position, transform.rotation);
        Publish(headPosePub, headPose);
    }
}
