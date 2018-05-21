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


        headPosePub = new RosPublisher<ros.geometry_msgs.Pose>(RosManager, "headPosePub", "/hololens/head_pose");
        gazePointPub = new RosPublisher<ros.geometry_msgs.Point>(RosManager, "gazePointPub", "/hololens/gaze_point");

        prevTimeStamp[headPosePub.name] = Time.unscaledTime;
        prevTimeStamp[gazePointPub.name] = Time.unscaledTime;

    }
	
	// Update is called once per frame
	void Update () {    
        ros.geometry_msgs.Pose headPose = new ros.geometry_msgs.Pose(transform.position, transform.rotation);
        Publish(headPosePub, headPose, 0.5);

        RaycastHit hitInfo;
        if (Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                out hitInfo,
                20.0f,
                Physics.DefaultRaycastLayers))
        {
            ros.geometry_msgs.Point gazePoint = new ros.geometry_msgs.Point(hitInfo.point);
            Publish(gazePointPub, gazePoint, 0.5);
        }

    }
}
