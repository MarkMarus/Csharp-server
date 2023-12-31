﻿using System;
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
            Console.WriteLine("3. Start a global server");

            string option = Console.ReadLine();

            if (option == "1")
            {
                Console.WriteLine("Enter the server port number:");
                int port = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Enter the server password:");
                string password = Console.ReadLine();

                SocketServer server = new SocketServer();
                server.Start(port, true, password);

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

                Console.WriteLine("Enter the server password:");
                string password = Console.ReadLine();

                IPEndPoint serverEndPoint;
                if (!IPAddress.TryParse(ipAddress, out IPAddress serverAddress))
                {
                    Console.WriteLine("Invalid IP address format.");
                    return;
                }
                serverEndPoint = new IPEndPoint(serverAddress, port);

                Login.LoginAsListener(serverEndPoint).GetAwaiter().GetResult();
            }
            else if (option == "3")
            {
                Console.WriteLine("Enter the server port number:");
                int port = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Enter the server password:");
                string password = Console.ReadLine();

                SocketServer server = new SocketServer();
                server.Start(port, true, password);

                string ipAddress = GetPublicIPAddress();
                Console.WriteLine($"Global server started at {ipAddress}:{port}");

                // Wait for the Enter key press to stop the server
                Console.WriteLine("Press Enter to stop the server.");
                Console.ReadLine();

                server.Stop();
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

        static string GetPublicIPAddress()
        {
            using (var client = new WebClient())
            {
                try
                {
                    string response = client.DownloadString("https://api.ipify.org");
                    return response.Trim();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to retrieve public IP address: {ex.Message}");
                }
            }

            return string.Empty;
        }
    }
}
