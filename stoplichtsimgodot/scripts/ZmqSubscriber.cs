using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Godot;
using NetMQ;
using NetMQ.Sockets;

namespace StoplichtSimGodot.scripts;

public partial class ZmqSubscriber : Node
{
	[Export] private string _ip;
	[Export] private int _port;
	[Export] private string[] _topics;

	private string _socketUri;
	private SubscriberSocket _subscriber;
	private readonly List<Action<(string topic, string message)>> _onReceiveMessage = [];
	private Task _receiveLoop;

	public override void _Ready()
	{
		_socketUri = $"tcp://{_ip}:{_port}";

		_subscriber = new SubscriberSocket();
		_subscriber.Connect(_socketUri); // Verbinden met de publisher
		foreach (var topic in _topics)
		{
			_subscriber.Subscribe(topic);
		}
		_receiveLoop = Task.Run(() =>
		{
			using var runtime = new NetMQRuntime();
			runtime.Run(ReceiveMessageLoop());
		});
	}


	public void DoOnMessage(Action<(string topic, string message)> action)
	{
		_onReceiveMessage.Add(action);
		GD.Print("Master Ontvangt");
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
				GD.Print(message);
				foreach (var action in _onReceiveMessage)
				{
					action.Invoke(message);
				}
			}
		}
		// ReSharper disable once FunctionNeverReturns
	}

	public override void _ExitTree()
	{
		_subscriber.Close();
	}
}
