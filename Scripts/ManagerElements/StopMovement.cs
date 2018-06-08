using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopMovement : RosComponent {

    private RosPublisher<ros.geometry_msgs.PoseStamped> pub;
    private bool stop = false;


	// Use this for initialization
	void Start () {
        Advertise("StopMovement", "/hololens/navigation/stop", 5, out pub);

        StartCoroutine(WaitForSpeechInit());
    }

    private IEnumerator WaitForSpeechInit()
    {
        yield return new WaitUntil(() => RosGazeManager.Instance != null && RosUserSpeechManager.Instance != null);

        RosUserSpeechManager.Instance.AddNewPhrase("stop", () =>
        {
            WaypointManager.Instance.Stop();
            TrackingManager.Instance.Stop();

            stop = true;
            RosUserSpeechManager.Instance.voicebox.StartSpeaking("Stopping now");
        });
    }
    // Update is called once per frame
    void Update () {
		if (stop)
        {
            stop = false;
            ros.geometry_msgs.PoseStamped msg = new ros.geometry_msgs.PoseStamped();
            msg.header.frame_id = "base_link";

            Publish(pub, msg);
        }
	}
}
