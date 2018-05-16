using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheelchair : MonoBehaviour {

    private GameObject Cam;


	// Use this for initialization
	void Start () {
	    Cam = GameObject.FindGameObjectWithTag("MainCamera");
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = Cam.transform.position;
        Vector3 cam_angles = Cam.transform.rotation.eulerAngles;
        transform.localEulerAngles = new Vector3(0, cam_angles.y, 0);
    }
}
