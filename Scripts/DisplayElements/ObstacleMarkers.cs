using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMarkers : RosComponent
{
    
    public GameObject ObstaclePrefab;

    private List<GameObject> markers;
    private RosSubscriber<ros.hololens_drive.ObstacleArray> sub;
    private String subtopic = "/formatted_grid/obs_array";
    private String subtype = "hololens_drive/ObstacleArray";

	// Use this for initialization
	void Start () {
        StartCoroutine(WaitForRosMessengerInitialisation("ObstacleMarker"));
        StartCoroutine(WaitUntilRosMessengerConnected("ObstacleMarker"));

        sub = new RosSubscriber<ros.hololens_drive.ObstacleArray>(RosManager,
                                                                  "ObstacleMarkerSub",
                                                                  subtopic,
                                                                  subtype);

        markers = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
        while (sub.MsgReady)
        {
            ros.hololens_drive.ObstacleArray obsArray = sub.GetNewMessage();
            int new_count = obsArray.obstacles.Count;
            int old_count = markers.Count;
            int max = (new_count > old_count) ? new_count : old_count;

            

            // Instantiate new markers if more are needed
            for (int i = 1; i <= max; i++)
            {
                if (i > old_count)
                {
                    // instantiate new marker
                    //GameObject marker = Instantiate(ObstaclePrefab);
                    GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    marker.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    markers.Add(marker);
                }
            }


            // Iterate through markers to change values
            for (int i = 0; i < max; i++)
            {
                GameObject marker = markers[i];
                if (i < new_count)
                {
                    // instantiate marker
                    marker.SetActive(true);
                    ros.geometry_msgs.Point p = obsArray.obstacles[i].rel_position;
                    marker.transform.position = new Vector3((float) p.x, Parameters.FloorDepth, (float) p.y);
                    //marker.GetComponent<ObstacleMarker>().SetDimensions((float)obsArray.obstacles[i].width, (float)obsArray.obstacles[i].height, 0.1f);
                }
                else
                {
                    // Deactivate marker
                    marker.SetActive(false);
                }
            }
        }
	}
}
