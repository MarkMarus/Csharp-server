using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Sockets.Client
{
    class Program
    {
        static void ConnectToSocket(string[] args)
        {
            Console.WriteLine("Press enter to connect to the server:");
            Console.ReadLine();

            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 43665);

                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(remoteEP);

                    Console.WriteLine($"Connected to {sender.RemoteEndPoint}");

                    while (true)
                    {
                        Console.WriteLine("Enter a message to send to the server (or 'exit' to quit):");
                        string message = Console.ReadLine();

                        if (message.ToLower() == "exit")
                            break;

                        byte[] msg = Encoding.ASCII.GetBytes(message + "<EOF>");

                        sender.Send(msg);

                        byte[] buffer = new byte[1024];
                        int bytesRec = sender.Receive(buffer);
                        string response = Encoding.ASCII.GetString(buffer, 0, bytesRec);
                        Console.WriteLine($"Server response: {response}");
                    }

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Press enter to exit the client:");
            Console.ReadLine();
        }
    }
}
