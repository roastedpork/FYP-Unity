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
    private String SubTopic = "/hololens/audio/voice_over";

    private RosPublisher<ros.std_msgs.String> pub;
    private String PubTopic = "/hololens/audio/transcript";

    // Voice & Microphone
    private TextToSpeech voicebox;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<String, Action> keywords = new Dictionary<string, Action>();

    private void Start()
    {
        StartCoroutine(WaitForRosMessengerInitialisation("TextToSpeechManager"));
        StartCoroutine(WaitUntilRosMessengerConnected("TextToSpeechManager"));

        sub = new RosSubscriber<ros.std_msgs.String>(RosManager,
                                                     "Voice_Sub",
                                                     SubTopic);

        pub = new RosPublisher<ros.std_msgs.String>(RosManager,
                                                    "Voice_Pub",
                                                    PubTopic);


        voicebox = GetComponent<TextToSpeech>();
        voicebox.Voice = TextToSpeechVoice.Zira;

        // Activation phrase for dictation
        keywords.Add("Hello", () =>
        {
            ros.std_msgs.String msg = new ros.std_msgs.String("Hello!");
            pub.SendMessage(msg);
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
