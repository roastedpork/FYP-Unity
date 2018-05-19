using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameGenerator : RosComponent
{

    private RosSubscriber<ros.geometry_msgs.PoseStamped> sub;
    private System.String RosTopic = "/hololens/frame";

    public GameObject FramePrefab;


	// Use this for initialization
	void Start () {
        StartCoroutine(WaitForRosMessengerInitialisation());
        StartCoroutine(WaitUntilRosMessengerConnected("FrameGenerator"));

        sub = new RosSubscriber<ros.geometry_msgs.PoseStamped>(RosManager,
                                                               "FrameGenerator_Sub",
                                                               RosTopic);
    }

    // Update is called once per frame
    void Update () {
        if (sub != null)
        {
            while (sub.MsgReady)
            {
                ros.geometry_msgs.PoseStamped new_pose = sub.GetNewMessage();

                GameObject f = Instantiate(FramePrefab,
                                           new_pose.pose.position.AsUnityVector,
                                           new_pose.pose.orientation.AsUnityQuaternion);

                f.GetComponent<FrameLabel>().Rename(new_pose.header.frame_id);

                f.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                Debug.Log("Spawning a frame at " + new_pose.pose.ToString());
            }
        }
        
	}
}
