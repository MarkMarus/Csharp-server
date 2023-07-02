using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Sockets.Server
{
    public class SyncServerSocket
    {
        private static string data = null;
        private static SemaphoreSlim listenerSemaphore = new SemaphoreSlim(10);
        private static List<Socket> listenerSockets = new List<Socket>();

        public static void StartSpeaker()
        {
            // Speaker code here
            Console.WriteLine("Speaker started.");

            // Example: Broadcasting a message to all connected listeners
            while (true)
            {
                string message = "Hello, listeners! This is a game update.";
                // Convert the message to bytes
                byte[] msg = Encoding.ASCII.GetBytes(message);

                // Iterate over connected listeners and send the message
                foreach (Socket listenerSocket in listenerSockets)
                {
                    listenerSocket.Send(msg);
                }

                // Pause for a specific interval before sending the next update
                Thread.Sleep(5000);
            }
        }

        public static void StartListener(object obj)
        {
            // Listener code here
            int listenerId = (int)obj;
            Console.WriteLine($"Listener {listenerId} started.");

            // Wait until a connection slot is available
            listenerSemaphore.Wait();

            try
            {
                byte[] bytes = new byte[1024];

                IPAddress iPAddress = IPAddress.Parse("127.0.0.1"); // Use the IPv4 loopback address explicitly
                IPEndPoint localEndPoint = new IPEndPoint(iPAddress, 43665);
                Socket listener = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(10);

                    Console.WriteLine($"Speaker started. Listening on {localEndPoint}");

                    while (true)
                    {
                        Console.WriteLine($"Waiting for a connection (Listener {listenerId})");
                        Socket handler = listener.Accept();
                        data = null;

                        while (true)
                        {
                            int byteRec = handler.Receive(bytes);
                            data += Encoding.ASCII.GetString(bytes, 0, byteRec);
                            if (data.IndexOf("<EOF>") > -1)
                                break;
                        }

                        Console.WriteLine($"Text received: {data} (Listener {listenerId})");
                        byte[] msg = Encoding.ASCII.GetBytes(data);
                        handler.Send(msg);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            finally
            {
                // Release the connection slot
                listenerSemaphore.Release();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            SyncServerSocket.StartSpeaker();

            // Start 10 listener threads
            for (int i = 0; i < 10; i++)
            {
                Thread listenerThread = new Thread(SyncServerSocket.StartListener);
                listenerThread.Start(i);
            }
            Console.ReadLine();
        }
    }
}
