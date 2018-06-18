using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wheelchair : ros.Singleton<Wheelchair> {
    
    private RosSubscriber<ros.geometry_msgs.Pose> sub;
    private GameObject rosframe;

    public Transform RosFrame {
        get
        {
            return rosframe.transform;
        }
    }


    public GameObject FramePrefab;


	// Use this for initialization
	void Start () {
        Subscribe("WheelchairPoseSub", "/hololens/wheelchair_pose", 10, out sub);
        rosframe = new GameObject();
    }
	
	// Update is called once per frame
	void Update () {
        ros.geometry_msgs.Pose msg;
        if (Receive(sub, out msg))
        {
            rosframe.transform.position = msg.position.AsUnityVector;
            rosframe.transform.rotation = msg.orientation.AsUnityQuaternion;
        }
    }
}
