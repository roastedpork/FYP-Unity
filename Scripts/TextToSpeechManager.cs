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
