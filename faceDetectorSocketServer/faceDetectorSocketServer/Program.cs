using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace faceDetectorSocketServer
{
    class Program
    {
        public static string dataIn = null;

        public static void StartListening()
        {
            // Incoming data
            byte[] bytes = new Byte[1024];

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

            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        static void Main(string[] args)
        {
            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
            //Console.WriteLine("Hello World!");
            //Console.ReadKey();
            StartListening();
            




            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
        }
    }
}
