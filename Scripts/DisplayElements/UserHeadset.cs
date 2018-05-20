using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserHeadset : MonoBehaviour {

    private GameObject cam;
    
	// Use this for initialization
	void Start () {
        cam = GameObject.FindGameObjectWithTag("MainCamera");

        if (cam != null)
        {
            Debug.Log("[UserHeadset] Camera found");
            transform.parent = cam.transform;
            transform.localRotation = Quaternion.Euler(0, -90, 0);
        }
        else Debug.Log("[UserHeadset] Camera could not be found");
    }
	
	// Update is called once per frame
	void Update () {
    }
}
