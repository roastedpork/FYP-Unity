using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageDisplay : RosComponent
{

    public GameObject Minimap;
    public GameObject LeftArrow;
    public GameObject RightArrow;

    private RawImage MinimapImage;
    private RawImage LeftArrowImage;
    private RawImage RightArrowImage;


    private RosSubscriber<ros.std_msgs.String> minimapSub;
    private String mapsubtopic = "hololens/display/encoded_minimap";

    private RosSubscriber<ros.std_msgs.Float64> rotationSub;
    private String rotsubtopic = "hololens/display/rotation_magnitude";
    

    private const String valuemap = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    // Use this for initialization
    void Start()
    {
        StartCoroutine(WaitForRosMessengerInitialisation("Minimap"));
        StartCoroutine(WaitUntilRosMessengerConnected("Minimap"));

        minimapSub = new RosSubscriber<ros.std_msgs.String>(RosManager,
                                                            "MinimapSub",
                                                            mapsubtopic);

        rotationSub = new RosSubscriber<ros.std_msgs.Float64>(RosManager,
                                                             "RotationSub",
                                                             rotsubtopic);

        MinimapImage = Minimap.GetComponent<RawImage>();
        LeftArrowImage = LeftArrow.GetComponent<RawImage>();
        RightArrowImage = RightArrow.GetComponent<RawImage>();

        MinimapImage.color = new Color(1, 1, 1, 0);
        LeftArrowImage.color = new Color(1, 1, 1, 0);
        RightArrowImage.color = new Color(1, 1, 1, 0);
    }

    byte[] DecodeString(String str)
    {
        List<byte> buff = new List<byte>();
        int pad = str.Count(c => c == '=');

        String strip = str.Replace("=", "A");
        
        for (int i = 0; i < strip.Length; i += 4)
        {
            String chunk = strip.Substring(i, 4);
            byte[] base64 = new byte[4];

            for (int j = 0; j < 4; j++)
            {
                char c = chunk[j];
                base64[j] = (byte)valuemap.IndexOf(c);
            }

            buff.Add((byte)((base64[0] << 2) + (base64[1] >> 4)));
            buff.Add((byte)((base64[1] << 4) + (base64[2] >> 2)));
            buff.Add((byte)((base64[2] << 6) + (base64[3])));
        }
        for (int i = 0; i < pad; i++) buff.RemoveAt(buff.Count - 1);
        return buff.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        if (minimapSub.MsgReady)
        {
            String encoded = minimapSub.GetNewMessage().data;
            byte[] image = DecodeString(encoded);

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(image);
            MinimapImage.texture = tex;

        }

        if (rotationSub.MsgReady)
        {
            Double rotation = rotationSub.GetNewMessage().data;
            if (rotation > 0)
            {
                LeftArrowImage.color = new Color(1, 1, 1, (float)rotation);
                RightArrowImage.color = new Color(1, 1, 1, 0);
            }
            else
            {
                LeftArrowImage.color = new Color(1, 1, 1, 0);
                RightArrowImage.color = new Color(1, 1, 1, -(float)rotation);
            }
        }
    }
}
