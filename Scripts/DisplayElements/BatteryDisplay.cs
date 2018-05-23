using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryDisplay : RosComponent {

    private int batt = 100;
    private Text display;

    private RosSubscriber<ros.std_msgs.Float64> sub;
    
    // Use this for initialization
    void Start () {
        Subscribe("BatteryDisplaySub", "/hololens/display/battery/set_battery_value", 10, out sub);
        display = transform.Find("Text").GetComponent<Text>();


	}



    // Update is called once per frame
    void Update () {
        ros.std_msgs.Float64 msg;
        if(Receive(sub, out msg))
        {
            System.Double value = msg.data;
            batt = (int)(value*100);
            display.text = "Battery: " + batt.ToString() + "%";
        }
	}
}
