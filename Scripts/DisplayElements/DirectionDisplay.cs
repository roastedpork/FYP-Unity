using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionDisplay : RosComponent
{
    private RosSubscriber<ros.sensor_msgs.Joy> sub;
    
    // Use this for initialization
    void Start () {
        Subscribe("DirectionArrowSub", "/arta/arta_joystick", 10, out sub);
	}
	
	// Update is called once per frame
	void Update () {
        ros.sensor_msgs.Joy msg;    
        if(Receive(sub, out msg))
        {
            float x = msg.axes[0];
            float y = msg.axes[1];

            float Size = (float)Math.Sqrt(x*x + y*y);
            transform.localScale = new Vector3(0.025f, 0.025f, 0.025f) * Size;

            float angle = (float)(Mathf.Rad2Deg * Math.Atan2(y, x)) - 90f;
            if (angle > 360) angle -= 360f;
            if (angle <-360) angle += 360f;

            transform.eulerAngles = new Vector3(0,0,angle);
        }
        
	}
}
