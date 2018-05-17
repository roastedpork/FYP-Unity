using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wheelchair : RosComponent {

    private GameObject Cam;


    private RosSubscriber<ros.geometry_msgs.Pose2D> sub;
    private String subtopic = "hololens/wheelchair_pose";
    private String subtype = "geometry_msgs/Pose2D";

	// Use this for initialization
	void Start () {
	    Cam = GameObject.FindGameObjectWithTag("MainCamera");

        sub = new RosSubscriber<ros.geometry_msgs.Pose2D>(RosManager,
                                                          "WheelchairPoseSub",
                                                          subtopic,
                                                          subtype);
    }
	
	// Update is called once per frame
	void Update () {

        while (sub.MsgReady)
        {
            ros.geometry_msgs.Pose2D msg = sub.GetNewMessage();
            transform.position = new Vector3((float)msg.x, 0, (float) msg.y);
            transform.rotation = Quaternion.Euler(0, -(float) msg.theta, 0);
        }

        
    }
}
