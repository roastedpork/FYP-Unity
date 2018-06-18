using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : RosComponent
{
    private RosSubscriber<ros.nav_msgs.GridCells> sub;
    private LineRenderer lineRenderer;

    // Use this for initialization
    void Start () {

        Subscribe("LineRenderSub", "/hololens/display/trajectory", 10, out sub);
        transform.parent = Wheelchair.Instance.RosFrame;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.useWorldSpace = false;
    }
    	
	// Update is called once per frame
	void Update () {
        ros.nav_msgs.GridCells path;

        if(Receive(sub, out path))
        { 
            List<Vector3> pointList = new List<Vector3>();

            int count = path.cells.Count;
            float dalpha = (float) (1.0 / count);
            lineRenderer.positionCount = count;

            for (int i=0; i<count; i++)
            {
                Vector3 point = new Vector3((float)path.cells[i].x,
                                            0,
                                            (float)path.cells[i].y);



                pointList.Add(point);
            }
            lineRenderer.SetPositions(pointList.ToArray());
        }
	}
}
