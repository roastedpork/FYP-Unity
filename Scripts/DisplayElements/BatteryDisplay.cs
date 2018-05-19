using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryDisplay : RosComponent {

    private int batt = 100;
    private Text display;

    private RosSubscriber<ros.std_msgs.Float64> sub;
    private string subTopic = "/hololens/display/battery/set_battery_value";

    // Use this for initialization
    void Start () {
        sub = new RosSubscriber<ros.std_msgs.Float64>(RosManager,
                                                      "BatteryDisplay_Sub",
                                                      subTopic);

        display = transform.Find("Text").GetComponent<Text>();


	}



    // Update is called once per frame
    void Update () {
        while (sub.MsgReady)
        {
            System.Double value = sub.GetNewMessage().data;
            batt = (int)(value*100);
            display.text = "Battery: " + batt.ToString() + "%";
        }
	}
}
