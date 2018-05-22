using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wheelchair : RosComponent {
    
    private RosSubscriber<ros.geometry_msgs.Pose2D> sub;
    
	// Use this for initialization
	void Start () {
        StartCoroutine(WaitUntilRosMessengerConnected("WheelchairPose"));
        Subscribe("WheelchairPoseSub", "/hololens/wheelchair_pose", 10, out sub);
    }
	
	// Update is called once per frame
	void Update () {

        while (sub.MsgReady)
        {
            ros.geometry_msgs.Pose2D msg = sub.GetNewMessage();
            transform.position = new Vector3((float)msg.x, 0, (float) msg.y);
            transform.rotation = Quaternion.Euler(0, -(float) (Mathf.Rad2Deg * msg.theta), 0);
        }

        
    }
}
