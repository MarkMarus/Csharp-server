using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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

        // Example: Send a welcome message to the client
        string welcomeMessage = "Welcome to the server!";
        byte[] welcomeMessageBytes = Encoding.UTF8.GetBytes(welcomeMessage);
        stream.Write(welcomeMessageBytes, 0, welcomeMessageBytes.Length);

        // TODO: Implement your game-specific logic here to send information to clients

        // Clean up resources
        stream.Close();
        client.Close();
        Console.WriteLine("Client disconnected.");
    }
}
