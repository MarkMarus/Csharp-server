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

                // Keep the server running until the user presses Enter
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

                LoginAsClient(ipAddress, port).GetAwaiter().GetResult();
            }
            else
            {
                Console.WriteLine("Invalid option selected.");
            }
        }

static async Task LoginAsClient(string ipAddress, int port)
        {
            using Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                IPAddress serverAddress;
                if (!IPAddress.TryParse(ipAddress, out serverAddress))
                {
                    Console.WriteLine("Invalid IP address format.");
                    return;
                }

                IPEndPoint serverEndPoint = new IPEndPoint(serverAddress, port);

                await client.ConnectAsync(serverEndPoint);

                while (true)
                {
                    // Send login message.
                    var loginMessage = "listener";
                    var messageBytes = Encoding.UTF8.GetBytes(loginMessage);
                    await client.SendAsync(messageBytes, SocketFlags.None);
                    Console.WriteLine($"Socket client sent login message: \"{loginMessage}\"");

                    // Receive login acknowledgment.
                    var buffer = new byte[1_024];
                    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                    var response = Encoding.UTF8.GetString(buffer, 0, received);
                    if (response == "login_ack")
                    {
                        Console.WriteLine($"Socket client received login acknowledgment: \"{response}\"");
                        break;
                    }

                    // Handle unsuccessful login acknowledgment.
                    Console.WriteLine($"Socket client received invalid login acknowledgment: \"{response}\"");
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
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