using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleMarkers : RosComponent
{
    
    public GameObject ObstaclePrefab;

    private List<GameObject> markers;
    private RosSubscriber<ros.hololens_drive.ObstacleArray> sub;
    private String subtopic = "/formatted_grid/obs_array";

	// Use this for initialization
	void Start () {
        StartCoroutine(WaitForRosMessengerInitialisation("ObstacleMarker"));
        StartCoroutine(WaitUntilRosMessengerConnected("ObstacleMarker"));

        sub = new RosSubscriber<ros.hololens_drive.ObstacleArray>(RosManager,
                                                                  "ObstacleMarkerSub",
                                                                  subtopic);

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
                    // instantiate new markers if this update exceeds the old count
                    GameObject marker = Instantiate(ObstaclePrefab);
                    marker.transform.parent = transform;
                    markers.Add(marker);
                }
            }


            // Iterate through markers to change transform values
            for (int i = 0; i < max; i++)
            {
                GameObject marker = markers[i];
                if (i < new_count)
                {
                    // activate markers if they are needed for this update
                    marker.SetActive(true);
                    ros.hololens_drive.Obstacle newObs = obsArray.obstacles[i];
                    ros.geometry_msgs.Point p = newObs.rel_position;
                    Quaternion rotation = Quaternion.Euler(0, -(float)(Mathf.Rad2Deg * newObs.box_angle), 0);


                    marker.transform.localPosition = new Vector3((float) p.x, Parameters.FloorDepth, (float) p.y);
                    marker.GetComponent<ObstacleMarker>().SetDimensions((float)newObs.height, (float)newObs.width, 0.1f);
                    marker.transform.localRotation = rotation;

                    Text label = marker.GetComponent<ObstacleMarker>().TextObject.GetComponent<Text>();

                    //label.text = (Mathf.Rad2Deg * newObs.box_angle).ToString();


                }
                else
                {
                    // Deactivate marker if unused
                    marker.SetActive(false);
                }
            }
        }
	}
}
