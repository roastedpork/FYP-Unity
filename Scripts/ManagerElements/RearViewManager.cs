using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearViewManager : MonoBehaviour {


	// Use this for initialization
	void Start () {
        StartCoroutine(WaitForSpeechInit());
    }

    private IEnumerator WaitForSpeechInit()
    {
        yield return new WaitUntil(() => RosGazeManager.Instance != null && RosUserSpeechManager.Instance != null);

        RosUserSpeechManager.Instance.AddNewPhrase("open rear view", () =>
        {
            gameObject.SetActive(true);

            transform.position = Camera.main.transform.forward;
            transform.LookAt(Camera.main.transform);
            transform.rotation *= Quaternion.Euler(0, 180, 0);


        });

        RosUserSpeechManager.Instance.AddNewPhrase("close rear view", () =>
        {
            gameObject.SetActive(false);
        });
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
