using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionDisplay : RosComponent
{

    private RosSubscriber<ros.sensor_msgs.Joy> sub;
    private String subtopic = "/arta/arta_joystick";
    private String subtype = "sensor_msgs/Joy";

    public Double Size { get; private set; }

    // Use this for initialization
    void Start () {
        StartCoroutine(WaitForRosMessengerInitialisation("Arrow"));
        StartCoroutine(WaitUntilRosMessengerConnected("Arrow"));
        sub = new RosSubscriber<ros.sensor_msgs.Joy>(RosManager,
                                                     "DirectionArrowSub",
                                                     subtopic,
                                                     subtype);

        Size = 0.0;
	}
	
	// Update is called once per frame
	void Update () {
        
        while (sub.MsgReady)
        {
            ros.sensor_msgs.Joy msg = sub.GetNewMessage();
            float x = msg.axes[0];
            float y = msg.axes[1];

            Size = Math.Sqrt(x*x + y*y);
            transform.localScale = new Vector3(0.025f, 0.025f, 0.025f) * (float)Size;

            float angle = (float)(Mathf.Rad2Deg * Math.Atan2(y, x)) - 90f;
            if (angle > 360) angle -= 360f;
            if (angle <-360) angle += 360f;

            transform.eulerAngles = new Vector3(0,0,angle);
        }
        
	}
}
