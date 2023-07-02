using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Sockets.Client.Login
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Start the server");
            Console.WriteLine("2. Login as a client");

            string option = Console.ReadLine();

            if (option == "1")
            {
                int port = 1234; // Specify the port number you want to listen on
                SocketServer server = new SocketServer();
                server.Start(port);

                string ipAddress = GetLocalIPAddress();
                Console.WriteLine($"Server started at {ipAddress}:{port}");

                // Wait for the Enter key press to stop the server
                Console.WriteLine("Press Enter to stop the server.");
                Console.ReadLine();

                server.Stop();
            }

            else if (option == "2")
            {
                Console.WriteLine("Enter the server IP address:");
                string ipAddress = Console.ReadLine();

                Console.WriteLine("Enter the server port number:");
                int port = Convert.ToInt32(Console.ReadLine());

                IPEndPoint serverEndPoint;
                if (!IPAddress.TryParse(ipAddress, out IPAddress serverAddress))
                {
                    Console.WriteLine("Invalid IP address format.");
                    return;
                }
                serverEndPoint = new IPEndPoint(serverAddress, port);

                Login.LoginAsListener(serverEndPoint).GetAwaiter().GetResult();

            }
            else
            {
                Console.WriteLine("Invalid option selected.");
            }
        }

        static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ipAddress in host.AddressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ipAddress.ToString();
                }
            }
            return string.Empty;
        }
    }
}