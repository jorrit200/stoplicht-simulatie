using UnityEngine;
using NetMQ;
using NetMQ.Sockets;
using System.Diagnostics;
using System;

public class ZMQClient : MonoBehaviour
{
    void Start()
    {
        using (var client = new RequestSocket(">tcp://localhost:5555")) // Verbind met de server
        {
            client.SendFrame("Hallo, server!");
            string response = client.ReceiveFrameString();
            Console.Write($"Server antwoordde: {response}");
        }
    }
}
