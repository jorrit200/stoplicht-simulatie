using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using NetMQ;
using NetMQ.Sockets;

namespace StoplichtSimGodot.scripts;

public partial class ZmqSubscriber : Node
{
    private SubscriberSocket _subscriber;

    private Task _receiveLoop;

    private readonly List<Action<(string topic, string message)>> _onReceiveMessage = [];

    public override async void _Ready()
    {
        _subscriber = new SubscriberSocket();
        _subscriber.Connect("tcp://127.0.0.1:5556"); // Verbinden met de publisher
        _subscriber.Subscribe("stoplichten"); // Abonneer op "topic1"

        GD.Print("Subscriber verbonden en geabonneerd op stoplichten...");
        _receiveLoop = Task.Run(() =>
        {
            using var runtime = new NetMQRuntime();
            runtime.Run(ReceiveMessageLoop());
        });
    }


    public void DoOnMessage(Action<(string topic, string message)> action)
    {
        _onReceiveMessage.Add(action);
    }


    // todo: deze method neemt aan dat het een multipart message krijgt met precies 2 parts. Als 1 bericht buiten de beugel valt gaat dit kapot.
    private async Task<(string topic, string body)> WaitForMessage()
    {
        var messageParts = await _subscriber.ReceiveMultipartMessageAsync(expectedFrameCount: 2);
        return (topic: messageParts[0].ConvertToString(), body: messageParts[1].ConvertToString());
    }

    private async Task ReceiveMessageLoop()
    {
        using (_subscriber)
        {
            while (true)
            {
                var message = await WaitForMessage();
                foreach (var action in _onReceiveMessage)
                {
                    action.Invoke(message);
                }
            }
        }
    }

    public override void _ExitTree()
    {
        _subscriber.Close();
    }
}