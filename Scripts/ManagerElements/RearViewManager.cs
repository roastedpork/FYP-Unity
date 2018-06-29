using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RearViewManager : MonoBehaviour {

    private RosImageSubscriber rawImage;
    private bool active = false;

	// Use this for initialization
	void Start () {
        rawImage = GetComponent<RosImageSubscriber>();
        StartCoroutine(WaitForSpeechInit());
    }

    private IEnumerator WaitForSpeechInit()
    {
        yield return new WaitUntil(() => RosGazeManager.Instance != null && RosUserSpeechManager.Instance != null);

        RosUserSpeechManager.Instance.AddNewPhrase("open rear view", () =>
        {
           
            RosUserSpeechManager.Instance.StartBeep.Play();
            active = true;

            rawImage.ShowDisplay();


        });

        RosUserSpeechManager.Instance.AddNewPhrase("close rear view", () =>
        {
            rawImage.HideDisplay();
            RosUserSpeechManager.Instance.StopBeep.Play();
            active = false;
        });
    }

    // Update is called once per frame
    void Update () {
        if (active)
        {
            transform.position = Camera.main.transform.position + Camera.main.transform.forward;
            transform.LookAt(Camera.main.transform);
            transform.rotation *= Quaternion.Euler(0, 180, 0);
        }
        
    }
}
