using System;
using System.Diagnostics;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public class ZMQServer : MonoBehaviour
{
    private Thread serverThread;
    private bool running = true;

    void Start()
    {
        serverThread = new Thread(ServerLoop);
        serverThread.Start();
    }

    private void ServerLoop()
    {
        using (var server = new ResponseSocket("@tcp://*:5555")) // Bind aan poort 5555
        {
            while (running)
            {
                string message = server.ReceiveFrameString();
                Console.Write($"Ontvangen: {message}");
                server.SendFrame("Hallo van de server!");
            }
        }
    }

    void OnDestroy()
    {
        running = false;
        serverThread.Join();
    }
}
