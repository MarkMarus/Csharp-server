using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Sockets.Client
{
    public class SocketServer
    {
        private TcpListener listener;
        private bool isRunning;

        public void Start(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            isRunning = true;
            Console.WriteLine("Server started. Waiting for connections...");

            // Start a new thread to handle incoming connections
            Thread listenThread = new Thread(ListenForClients);
            listenThread.Start();
        }

        public void Stop()
        {
            isRunning = false;
            listener.Stop();
            Console.WriteLine("Server stopped.");
        }

        private void ListenForClients()
        {
            while (isRunning)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected!");

                // Start a new thread to handle client communication
                Thread clientThread = new Thread(HandleClientCommunication);
                clientThread.Start(client);
            }
        }

        private void HandleClientCommunication(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;
            NetworkStream stream = client.GetStream();

            // Receive the login message from the client
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string loginMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received login message from client: {loginMessage}");

            // Check the login message and send the appropriate acknowledgment
            string response;
            if (loginMessage == "listener")
            {
                response = "login_ack";
            }
            else
            {
                response = "invalid_login";
            }

            // Send the login acknowledgment response to the client
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);
            Console.WriteLine($"Sent login acknowledgment to client: {response}");

            // Keep the connection alive and continue communication
            while (isRunning)
            {
                // Receive messages from the client
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received message from client: {message}");

                // TODO: Handle the received message as needed

                // Example response to the client
                string responseMessage = "Server received your message";
                byte[] responseMessageBytes = Encoding.UTF8.GetBytes(responseMessage);
                stream.Write(responseMessageBytes, 0, responseMessageBytes.Length);
                Console.WriteLine($"Sent response to client: {responseMessage}");
            }

            // Clean up resources
            stream.Close();
            client.Close();
            Console.WriteLine("Client disconnected.");
        }
    }
}
