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
using System.Threading;

public class Mood : MonoBehaviour
{
    private DynamicCharacterAvatar avatar;
    private ExpressionPlayer expression;
    private bool connected = false;
    private bool socketConnected = false;

    static string dataIn = null;
    byte[] bytes = new Byte[1024];
    String[] splitData = new string[2];
    int[] faceX = new int[68];
    int[] faceY = new int[68];

    String rawData;
    String rawDataX;
    String rawDataY;
    private float headTilt = 0f;
    Thread socketThread;

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
                Debug.Log("Waiting for a connection...");
                Socket handler = listener.Accept();
                dataIn = null;
                if (handler.Connected)
                {
                    socketConnected = true;
                    Console.WriteLine("Connected");
                    Debug.Log("Connected");
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
                        splitData = dataIn.Remove(dataIn.Length - 1).Split('/');
                        faceX = Array.ConvertAll(splitData[0].Split(','), int.Parse);
                        faceY = Array.ConvertAll(splitData[1].Split(','), int.Parse);
                        Console.WriteLine(dataIn);
                        //headTilt = (faceY[0] - faceY[16]) / 100;
                        
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
        socketThread = new Thread(new ThreadStart(doSocket));
        socketThread.Start();

    }

    private void OnDisable()
    {
        avatar.CharacterCreated.RemoveListener(OnCreated);
        socketThread.Abort();
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

    float turnHead = 0f;
    float dimHead = 0.05f;
    float jawOC = 0f;
    float headUD = 0f;
    float mountNarrow = 0f;
    float headLR = 0f;
    float eyeR = 0f;
    float eyeL = 0f;

    private void Update()
    {
        
        if (connected == true)
        {
            Debug.Log(Math.Sqrt(Math.Pow(faceX[27] - faceX[30],2) + Math.Pow(faceY[27] - faceY[30], 2)));
            //expression.headLeft_Right = turnHead;
            headUD = (float) (Math.Sqrt(Math.Pow(faceX[27] - faceX[30], 2) + Math.Pow(faceY[27] - faceY[30], 2)) - 33f);
            expression.headTiltLeft_Right = (float) ((faceY[0] - faceY[16]) * -0.01);
            jawOC = (float)(Math.Sqrt(Math.Pow(faceX[62] - faceX[66], 2) + Math.Pow(faceY[62] - faceY[66], 2)) - 8);
            mountNarrow = (float)(Math.Sqrt(Math.Pow(faceX[48] - faceX[54], 2) + Math.Pow(faceY[48] - faceY[54], 2)) - 65) * 5f * 0.01f;
            headLR = (float)((Math.Sqrt(Math.Pow(faceX[0] - faceX[28], 2) + Math.Pow(faceY[0] - faceY[28], 2)) - Math.Sqrt(Math.Pow(faceX[16] - faceX[28], 2) + Math.Pow(faceY[16] - faceY[28], 2)))) * 0.01f;
            eyeR = (float) (Math.Sqrt(Math.Pow(faceX[44] - faceX[46], 2) + Math.Pow(faceY[44] - faceY[46], 2)) - 7) * 0.33f ;
            eyeL = (float) (Math.Sqrt(Math.Pow(faceX[37] - faceX[41], 2) + Math.Pow(faceY[37] - faceY[41], 2)) - 7) * 0.33f;
            //Debug.Log(mountNarrow);

            //Mouth Open
            if (jawOC < 0)
            {
                expression.jawOpen_Close = 0f;
            }
            else
            {
                expression.jawOpen_Close = jawOC * 4 * 0.01f;
            }

            //mountNarrow Controller
            if(mountNarrow <= -1f)
            {
                expression.mouthNarrow_Pucker = -1f;
                expression.leftMouthSmile_Frown = 0f;
                expression.rightMouthSmile_Frown = 0f;
            }
            else if (mountNarrow >= 1f)
            {
                expression.mouthNarrow_Pucker = 0f;
                expression.leftMouthSmile_Frown = 1f;
                expression.rightMouthSmile_Frown = 1f;
            }
            else
            {
                if(mountNarrow > 0.2f)
                {
                    expression.mouthNarrow_Pucker = 0f;
                    expression.leftMouthSmile_Frown = mountNarrow;
                    expression.rightMouthSmile_Frown = mountNarrow;
                }
                else
                {
                    expression.mouthNarrow_Pucker = mountNarrow;
                    expression.leftMouthSmile_Frown = 0f;
                    expression.rightMouthSmile_Frown = 0f;
                }
                
            }

            // Head Tilt Left and Right Controller
            if (headLR >= 1)
            {
                expression.headLeft_Right = 1f;
            }
            else if (headLR <= -1f)
            {
                expression.headLeft_Right = -1f;
            }
            else
            {
                expression.headLeft_Right = headLR;
            }

            // Head Tilt Up and Down Controller
            if(headUD < -15)
            {
                expression.headUp_Down = 1f;
            }
            else if( headUD > 7)
            {
                expression.headUp_Down = -1f;
            }
            else
            {
                if(headUD > 30)
                {
                    expression.headUp_Down = headUD * -0.05f;
                }
                else
                {
                    expression.headUp_Down = headUD * -0.1f;
                }
                
            }
            
            //Left Eye Controller
            if(eyeL <= -1)
            {
                expression.leftEyeOpen_Close = -1;
            }
            else if (eyeL >= 1)
            {
                expression.leftEyeOpen_Close = 1;
            }
            else
            {
                expression.leftEyeOpen_Close = eyeL;
            }

            //Right Eye Controller
            if (eyeR <= -1)
            {
                expression.rightEyeOpen_Close = -1;
            }
            else if (eyeR >= 1)
            {
                expression.rightEyeOpen_Close = 1;
            }
            else
            {
                expression.rightEyeOpen_Close = eyeR;
            }
        }
    }
}
