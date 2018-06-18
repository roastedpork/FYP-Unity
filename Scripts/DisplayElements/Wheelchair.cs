using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wheelchair : ros.Singleton<Wheelchair> {
    
    private RosSubscriber<ros.geometry_msgs.Pose> sub;
    private GameObject footprint;
    public Transform RosFrame {
        get
        {
            return transform;
        }
    }

    public GameObject FramePrefab;


	// Use this for initialization
	void Start () {
        Subscribe("WheelchairPoseSub", "/hololens/wheelchair_pose", 10, out sub);
        footprint = transform.Find("Footprint").gameObject;
        footprint.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        ros.geometry_msgs.Pose msg;
        if (Receive(sub, out msg))
        {
            transform.position = msg.position.AsUnityVector;
            transform.rotation = msg.orientation.AsUnityQuaternion;
            footprint.SetActive(true);
        }
    }
}
