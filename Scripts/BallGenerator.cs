using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGenerator : RosComponent
{

    private RosSubscriber<ros.geometry_msgs.Vector3> sub;
    private String SubTopic = "/hololens/spawn_point";
    private String SubType = "geometry_msgs/Vector3";

    private RosPublisher<ros.std_msgs.String> pub;
    private String PubTopic = "/hololens/spawn_ack";
    private String PubType = "std_msgs/String";

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(WaitForRosMessengerInitialisation());
        StartCoroutine(WaitUntilRosMessengerConnected("BallGenerator"));
        
        sub = new RosSubscriber<ros.geometry_msgs.Vector3>(RosManager,
                                                           "BallGenerator_Sub",
                                                           SubTopic,
                                                           SubType);

        pub = new RosPublisher<ros.std_msgs.String>(RosManager,
                                                    "BallGenerator_Pub",
                                                    PubTopic,
                                                    PubType);

    }

    // Update is called once per frame
    void Update () {
        if (sub != null)
        {
            while (sub.MsgReady)
            {
                ros.geometry_msgs.Vector3 spawnPoint = sub.GetNewMessage();
                GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ball.transform.position = spawnPoint.AsUnityVector;
                ball.transform.localScale = new Vector3(1, 1, 1) * (float)0.05;

                ros.std_msgs.String resp = new ros.std_msgs.String("Spawning a ball at location " + spawnPoint.AsUnityVector.ToString());

                pub.SendMessage(resp);
            }
        }
	}
}
