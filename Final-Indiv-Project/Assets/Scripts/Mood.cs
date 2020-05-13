using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;
using UMA.PoseTools;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class Mood : MonoBehaviour
{
    private DynamicCharacterAvatar avatar;
    private ExpressionPlayer expression;
    private bool connected = false;
    private bool socketConnected = false;
    static string dataIn = null;
    byte[] bytes = new Byte[1024];

    public void doSocket()
    {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); //Name of host running application
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000);

        //Create TCP Socket
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);


            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                Socket handler = listener.Accept();
                dataIn = null;
                if (handler.Connected)
                {
                    socketConnected = true;
                    Console.WriteLine("Connected");
                }

                while (true)
                {

                    //byte[] data = new byte[handler.Available];
                    int byteRec = handler.Receive(bytes, 0, bytes.Length, 0);
                    Array.Resize(ref bytes, byteRec);
                    //Console.WriteLine(Encoding.UTF8.GetString(bytes));
                    dataIn += Encoding.UTF8.GetString(bytes);
                    if (dataIn.IndexOf("X") > -1)
                    {
                        Console.WriteLine(dataIn);
                        dataIn = null;

                    }

                }

                //Console.WriteLine("Text received : {0}", dataIn);
                // Echo the data back to the client.  
                //byte[] msg = Encoding.ASCII.GetBytes(dataIn);

                //handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
    private void OnEnable()
    {
        avatar = GetComponent<DynamicCharacterAvatar>();
        avatar.CharacterCreated.AddListener(OnCreated);
    }

    private void OnDisable()
    {
        avatar.CharacterCreated.RemoveListener(OnCreated);
    }

    public void OnCreated(UMAData data)
    {
        expression = GetComponent<ExpressionPlayer>();
        expression.enableBlinking = true;
        expression.enableSaccades = true;
        expression.overrideMecanimHead = true;
        expression.overrideMecanimNeck = true;
        connected = true;
    }

    private void Update()
    {
        float turnHead = 0f;
        float dimHead = 0.05f;
        if (connected == true)
        {
            Debug.Log(turnHead);
            expression.headLeft_Right = turnHead;
            turnHead += dimHead;
            if (turnHead >= 1f)
            {
                dimHead = -0.05f;
            }
            else if (turnHead <= -1f)
            {
                dimHead = 0.05f;
            }


        }
    }
}
