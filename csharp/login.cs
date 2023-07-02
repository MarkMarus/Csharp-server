using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Sockets.Client.Login
{
    class Login
    {
        public static async Task LoginAsListener(IPEndPoint serverEndPoint)
        {
            using Socket client = new Socket(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            await client.ConnectAsync(serverEndPoint);

            // Send login message.
            var loginMessage = "listener";
            var messageBytes = Encoding.UTF8.GetBytes(loginMessage);
            await client.SendAsync(messageBytes, SocketFlags.None);
            Console.WriteLine($"Socket client sent login message: \"{loginMessage}\"");

            while (true)
            {
                // Receive login acknowledgment.
                var buffer = new byte[1_024];
                var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                if (response == "login_ack")
                {
                    Console.WriteLine($"Socket client received login acknowledgment: \"{response}\"");
                    // Continue with your desired logic here for the connected state
                }
                else
                {
                    Console.WriteLine($"Socket client received invalid login acknowledgment: \"{response}\"");
                    break;
                }
            }

            Console.WriteLine("Press Enter to exit: ");
            Console.ReadLine();
            client.Shutdown(SocketShutdown.Both);
        }
    }
}