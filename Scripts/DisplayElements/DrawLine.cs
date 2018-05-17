using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : RosComponent
{

    private LineRenderer lineRenderer;
    private float depth = -0.9f;

    private RosSubscriber<ros.nav_msgs.GridCells> sub;
    private string subtopic = "/hololens/display/trajectory";
    private string subtype = "nav_msgs/GridCells";




    // Use this for initialization
    void Start () {
        StartCoroutine(WaitForRosMessengerInitialisation("LineRenderer"));
        StartCoroutine(WaitUntilRosMessengerConnected("LineRenderer"));

        sub = new RosSubscriber<ros.nav_msgs.GridCells>(RosManager,
                                                         "LineRenderSub",
                                                         subtopic,
                                                         subtype);
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.useWorldSpace = false;
        
        
    }
    	
	// Update is called once per frame
	void Update () {
        while (sub.MsgReady)
        {
            ros.nav_msgs.GridCells path = sub.GetNewMessage();

            List<Vector3> pointList = new List<Vector3>();
            //List<Material> matList = new List<Material>();

            int count = path.cells.Count;
            float dalpha = (float) (1.0 / count);
            lineRenderer.positionCount = count;

            for (int i=0; i<count; i++)
            {
                Vector3 point = new Vector3((float)path.cells[i].x,
                                            Parameters.FloorDepth,
                                            (float)path.cells[i].y);



                pointList.Add(point); // + transform.parent.position);
            }

            lineRenderer.SetPositions(pointList.ToArray());
        }
	}
}
