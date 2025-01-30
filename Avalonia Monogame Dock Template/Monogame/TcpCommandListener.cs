using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Avalonia_Monogame_Dock_Template.Monogame;

class TcpCommandListener
{
    private readonly int port;
    private TcpListener listener;
    private bool isRunning;

    public TcpCommandListener(int port)
    {
        this.port = port;
    }

    public void Start()
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        isRunning = true;

        Console.WriteLine($"TCP Listener started on port {port}");

        // Spusti prijímanie klientov v samostatnom vlákne
        Thread listenerThread = new Thread(ListenForClients);
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    private void ListenForClients()
    {
        while (isRunning)
        {
            try
            {
                // Akceptuj prichádzajúceho klienta
                TcpClient client = listener.AcceptTcpClient();
                Thread clientThread = new Thread(HandleClient);
                clientThread.IsBackground = true;
                clientThread.Start(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        StreamReader reader = new StreamReader(stream, Encoding.UTF8);

        try
        {
            string command;
            while ((command = reader.ReadLine()) != null)
            {
                Console.WriteLine($"Received: {command}");

                // Tu spracuj príkazy
                ProcessCommand(command);

                // Pošli späť odpoveď (voliteľné)
                string response = "Command received";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client connection error: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }

    private void ProcessCommand(string command)
    {
        // Tu implementuj spracovanie príkazov
        Console.WriteLine($"Processing command: {command}");
    }

    public void Stop()
    {
        isRunning = false;
        listener.Stop();
    }
}
