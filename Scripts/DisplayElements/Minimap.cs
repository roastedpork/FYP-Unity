using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : RosComponent {

    private RosSubscriber<ros.std_msgs.String> sub;
    private String subtopic = "hololens/display/encoded_minimap";
    private String subtype = "std_msgs/String";

    private RawImage rawimage;

    private const String valuemap = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    // Use this for initialization
    void Start () {
        StartCoroutine(WaitForRosMessengerInitialisation("Minimap"));
        StartCoroutine(WaitUntilRosMessengerConnected("Minimap"));

        sub = new RosSubscriber<ros.std_msgs.String>(RosManager,
                                                     "MinimapSub",
                                                     subtopic,
                                                     subtype);

        rawimage = GetComponent<RawImage>();


        
    }
	
    
    
    byte[] DecodeString(String str)
    {
        List<byte> buff = new List<byte>();
        int pad = str.Count(c => c == '=');

        String strip = str.Substring(0, str.Length - pad);
        for (int i = 0; i < pad; i++) strip += "A";
        
        for(int i=0; i<strip.Length; i += 4)
        {
            String chunk = strip.Substring(i, 4);
            byte[] base64 = new byte[4];

            for (int j=0; j<4; j++)
            {
                char c = chunk[j];
                base64[j] = (byte) valuemap.IndexOf(c);
            }

            buff.Add((byte)((base64[0] << 2) + (base64[1] >> 4)));
            buff.Add((byte)((base64[1] << 4) + (base64[2] >> 2)));
            buff.Add((byte)((base64[2] << 6) + (base64[3])));
        }
        for (int i = 0; i < pad; i++) buff.RemoveAt(buff.Count-1);
        return buff.ToArray();
    }
    

	// Update is called once per frame
	void Update () {
        if (sub.MsgReady)
        {
            String encoded = sub.GetNewMessage().data;
            byte[] image = DecodeString(encoded);

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(image);
            rawimage.texture = tex;

        }
	}
}
