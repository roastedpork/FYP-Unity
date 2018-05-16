using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameLabel : MonoBehaviour {

    public GameObject Label;
    private GameObject Cam;
    private bool isLabelOn = false;

	// Use this for initialization
	void Awake () {
        Cam = GameObject.FindGameObjectWithTag("MainCamera");

        if (Label == null)
        {
            Debug.Log("Could not get Text GameObject");
        }
        else
        {
            isLabelOn = true;
            Label.GetComponent<Text>().text = "";
            Debug.Log("Label successfully instantiated");
        }
        

    }
	
	// Update is called once per frame
	void Update () {
        if (isLabelOn)
        {
            Label.transform.LookAt(Cam.transform);
            Label.transform.Rotate(new Vector3(0, 180, 0));
        }
	}

    public void Rename(System.String newLabel)
    {
        if (isLabelOn)
        {
            StartCoroutine(WaitForLabel());
            Label.GetComponent<Text>().text = newLabel;
        }
    }
    private IEnumerator WaitForLabel()
    {
        yield return new WaitUntil(() => Label != null);
    }
}
