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


    private WaypointManager wpmanager;

    public GameObject AlignmentManager;
    private WorldAlignment alignManager;

    // Voice & Microphone
    private TextToSpeech voicebox;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<String, Action> keywords = new Dictionary<string, Action>();

    private void Start()
    {
        StartCoroutine(WaitUntilRosMessengerConnected("SpeechInputManager"));

        Advertise("VoicePub", "/hololens/audio/transcript", 10, out pub);
        Subscribe("VoiceSub", "/hololens/audio/voice_over", 10, out sub);

        wpmanager = GetComponent<WaypointManager>();
        alignManager = AlignmentManager.GetComponent<WorldAlignment>();
        
        voicebox = GetComponent<TextToSpeech>();
        voicebox.Voice = TextToSpeechVoice.Zira;

        // Activation phrase for dictation
        keywords.Add("Hello", () =>
        {
            ros.std_msgs.String msg = new ros.std_msgs.String("Hello!");
            pub.SendMessage(msg);
            voicebox.StartSpeaking("Hello");
        });

        keywords.Add("Move there", () => { if (!wpmanager.AddingMultipleWaypoints) wpmanager.SingleWaypoint(); });
        keywords.Add("Add point", () => {
            if (wpmanager.AddWaypoint())
            {
                voicebox.StartSpeaking("Point added");
            }
            else
            {
                voicebox.StartSpeaking("Could not add point");
            }
            
        });
        keywords.Add("goodbye", () => {
            if (wpmanager.AddingMultipleWaypoints)
            {
                wpmanager.PublishWaypoints();
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
            if (alignManager.SetMarker(1))
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
            if (alignManager.SetMarker(2))
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
            if (alignManager.SetMarker(3))
            {
                voicebox.StartSpeaking("Marker three set");
            }
            else
            {
                voicebox.StartSpeaking("Could not set marker three");
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
        if (sub != null)
        {
            while (sub.MsgReady)
            {
                ros.std_msgs.String msg = sub.GetNewMessage();
                voicebox.StartSpeaking(msg.data);
            }
        }
        
	}
}
