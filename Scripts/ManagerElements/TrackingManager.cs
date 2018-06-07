using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingManager : RosComponent {

    private RosPublisher<ros.geometry_msgs.PoseStamped> trackingPub;
    private bool ContinuousTracking = false;
    
    
    // Use this for initialization
    void Start () {
        Advertise("WaypointPub", "/hololens/navigation/continuous_tracking", 5, out trackingPub);
        StartCoroutine(WaitForSpeechInit());
    }

    private IEnumerator WaitForSpeechInit()
    {
        yield return new WaitUntil(() => RosGazeManager.Instance != null && RosUserSpeechManager.Instance != null);

        RosUserSpeechManager.Instance.AddNewPhrase("Track my gaze", () =>
        {
            ContinuousTracking = true;
            RosUserSpeechManager.Instance.voicebox.StartSpeaking("Gaze tracking starting");
        });
        RosUserSpeechManager.Instance.AddNewPhrase("End tracking", () =>
        {
            ContinuousTracking = false;
            RosUserSpeechManager.Instance.voicebox.StartSpeaking("Gaze tracking ended");
        });
    }
	
	// Update is called once per frame
	void Update () {
        if (ContinuousTracking && RosGazeManager.Instance.Focused)
        {
            ros.geometry_msgs.PoseStamped msg = new ros.geometry_msgs.PoseStamped();
            msg.header.frame_id = "/Unity";

            Quaternion camRot = Camera.main.transform.rotation * Quaternion.Euler(0, -90, 0);
            Quaternion pointRot = Quaternion.Euler(0, camRot.eulerAngles.y, 0);
            msg.pose.position = new ros.geometry_msgs.Point(RosGazeManager.Instance.position);
            msg.pose.orientation = new ros.geometry_msgs.Quaternion(pointRot);

            Publish(trackingPub, msg);
        }
    }
}
