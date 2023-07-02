using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                Console.WriteLine("Enter a message to send (or 'exit' to quit): ");
                var input = Console.ReadLine();

                if (input == "exit")
                    break;

                // Send user input message.
                messageBytes = Encoding.UTF8.GetBytes(input);
                await client.SendAsync(messageBytes, SocketFlags.None);
                Console.WriteLine($"Socket client sent message: \"{input}\"");

                // Receive server response.
                var buffer = new byte[1_024];
                var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                Console.WriteLine($"Socket client received response: \"{response}\"");
            }

            Console.WriteLine("Press Enter to exit: ");
            Console.ReadLine();
            client.Shutdown(SocketShutdown.Both);
        }
    }
}
