using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;


public class TextToSpeechManager : RosComponent
{

    // ROS communication
    private RosSubscriber<ros.std_msgs.String> sub;
    private RosPublisher<ros.std_msgs.String> pub;


    public WaypointManager wpManager;
    public WorldAlignment alignManager;

    // Voice & Microphone
    private TextToSpeech voicebox;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<String, Action> keywords = new Dictionary<string, Action>();


    private bool connected = false;




    private void Start()
    {
        if (RosMessenger.Instance.Con)
        {
            Advertise("VoicePub", "/hololens/audio/transcript", 10, out pub);
            Subscribe("VoiceSub", "/hololens/audio/voice_over", 10, out sub);
            connected = true;
        }



        transform.parent = wpManager.gameObject.transform;
        voicebox = GetComponent<TextToSpeech>();
        voicebox.Voice = TextToSpeechVoice.Zira;
        


        // Activation phrase for dictation
        keywords.Add("Hello", () =>
        {
            ros.std_msgs.String msg = new ros.std_msgs.String("Hello!");
            if (pub != null) pub.SendMessage(msg);
            voicebox.StartSpeaking("Hello");
        });

        keywords.Add("Move there", () => {
            if (!wpManager.gameObject.activeSelf) voicebox.StartSpeaking("Waypoint manager is not active");
            else if (!wpManager.AddingMultipleWaypoints) wpManager.SingleWaypoint();
        });
        keywords.Add("Add point", () => {
            if (!wpManager.gameObject.activeSelf) voicebox.StartSpeaking("Waypoint manager is not active");
            else if(wpManager.AddWaypoint())
            {
                voicebox.StartSpeaking("Point added");
            }
            else
            {
                voicebox.StartSpeaking("Could not add point");
            }
            
        });
        keywords.Add("goodbye", () => {
            if (!wpManager.gameObject.activeSelf) voicebox.StartSpeaking("Waypoint manager is not active");
            else if(wpManager.AddingMultipleWaypoints)
            {
                wpManager.PublishWaypoints();
                voicebox.StartSpeaking("Moving along path");
            }
            else
            {
                voicebox.StartSpeaking("This should not happen");
            }
        });

        // Activation phrases for setting world markers
        keywords.Add("Set marker one", () =>
        {
            if (!alignManager.gameObject.activeSelf)
            {
                voicebox.StartSpeaking("Alignment manager is not active");
            }
            else if (alignManager.SetMarker(1))
            {
                voicebox.StartSpeaking("Marker one set");
            }
            else
            {
                voicebox.StartSpeaking("Could not set marker one");
            }
        });

        keywords.Add("Set marker two", () =>
        {
            if (!alignManager.gameObject.activeSelf)
            {
                voicebox.StartSpeaking("Alignment manager is not active");
            }
            else if (alignManager.SetMarker(2))
            {
                voicebox.StartSpeaking("Marker two set");
            }
            else
            {
                voicebox.StartSpeaking("Could not set marker two");
            }
        });

        keywords.Add("Set marker three", () =>
        {
            if (!alignManager.gameObject.activeSelf)
            {
                voicebox.StartSpeaking("Alignment manager is not active");
            }
            else if (alignManager.SetMarker(3))
            {
                voicebox.StartSpeaking("Marker three set");
            }
            else
            {
                voicebox.StartSpeaking("Could not set marker three");
            }
        });

        keywords.Add("Start scan", () =>
        {
            if (ScanManager.Instance.isActiveAndEnabled)
            {
                voicebox.StartSpeaking("Starting scan");
                ScanManager.Instance.StartScan();
            }
            else
            {
                voicebox.StartSpeaking("Scan Manager is not active");
            }
        });


        keywords.Add("Stop scan", () =>
        {
            if (ScanManager.Instance.isActiveAndEnabled)
            {
                voicebox.StartSpeaking("Stopping scan");
                ScanManager.Instance.StopScan();

                GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                floor.transform.position = new Vector3(0, Parameters.FloorDepth, 0);
                floor.transform.localScale = new Vector3(1, 1, 1) * 15;
                floor.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                voicebox.StartSpeaking("Scan Manager is not active");
            }
        });

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();


    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

    // Update is called once per frame
    void Update () {
        if (!connected && RosMessenger.Instance.Con)
        {
            Advertise("VoicePub", "/hololens/audio/transcript", 10, out pub);
            Subscribe("VoiceSub", "/hololens/audio/voice_over", 10, out sub);
            connected = true;
        }

        if (sub != null)
        {
            ros.std_msgs.String msg;
            if(Receive(sub, out msg))
            {
                voicebox.StartSpeaking(msg.data);
            }
        }
        
	}
}
