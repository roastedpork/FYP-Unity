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
