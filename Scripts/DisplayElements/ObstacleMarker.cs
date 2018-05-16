using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMarker : MonoBehaviour {

    public GameObject TextObject;
    private GameObject Cam;

    private List<Transform> xRods;
    private List<Transform> yRods;
    private List<Transform> zRods;
    private List<Transform> vertices;
    private float edgeSize = 0.005f;
    

    private static System.Int16[] s0 = new System.Int16[] { 1, 1, 1, 1, -1, -1, -1, -1 };
    private static System.Int16[] s1 = new System.Int16[] { 1, 1, -1, -1, 1, 1, -1, -1 };
    private static System.Int16[] s2 = new System.Int16[] { 1, -1, 1, -1, 1, -1, 1, -1 };


    private bool ready = false;
    private bool preloaded = true;
    private float[] dims;


    // Use this for initialization
    void Start () {
        xRods = new List<Transform>();
        yRods = new List<Transform>();
        zRods = new List<Transform>();
        vertices = new List<Transform>();

        Cam = GameObject.FindGameObjectWithTag("MainCamera");

        String[] children = new String[] {"x1","x2","x3","x4","y1","y2","y3","y4","z1","z2","z3","z4",
                                            "p1","p2","p3","p4","p5","p6","p7","p8"};
        foreach (String child in children)
        {
            StartCoroutine(WaitUntilInitialised(child));

            if (child.Contains("x")) xRods.Add(transform.Find(child));
            if (child.Contains("y")) yRods.Add(transform.Find(child));
            if (child.Contains("z")) zRods.Add(transform.Find(child));
            if (child.Contains("p"))
            {
                Transform t = transform.Find(child);
                t.localScale = new Vector3(edgeSize, edgeSize, edgeSize);
                vertices.Add(t);
                
            }
        }
        ready = true;
    }
    
    private IEnumerator WaitUntilInitialised(String child)
    {
        yield return new WaitUntil(() => transform.Find(child) != null);
    }

    public void SetDimensions(float length, float width, float height = 0.5f)
    {
        if (ready)
        {
            float xsize = length / 2;
            float ysize = width / 2;
            float zsize = height / 2;

            for (int i = 0; i < 4; i++)
            {
                xRods[i].localScale = new Vector3(edgeSize, xsize, edgeSize);
                xRods[i].localPosition = new Vector3(0, s1[i] * zsize, s2[i] * ysize);
            
                yRods[i].localScale = new Vector3(edgeSize, ysize, edgeSize);
                yRods[i].localPosition = new Vector3(s1[i] * xsize, s2[i] * zsize, 0);
            
                zRods[i].localScale = new Vector3(edgeSize, zsize, edgeSize);
                zRods[i].localPosition = new Vector3(s1[i] * xsize, 0, s2[i] * ysize);
            }
            for (int i = 0; i < 8; i++)
            {
                vertices[i].localPosition = new Vector3(s0[i] * xsize, s1[i] * zsize, s2[i] * ysize);
            }
        }
        else
        {
            dims = new float[] { length, width, height };
            preloaded = false;
        }
    }

	void Update ()
    {
	    if (ready && !preloaded)
        {
            SetDimensions(dims[0], dims[1], dims[2]);
            preloaded = true;
        }

        TextObject.transform.LookAt(Cam.transform);
        TextObject.transform.Rotate(new Vector3(0, 180, 0));
	}
}
