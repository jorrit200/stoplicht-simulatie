using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using NetMQ;
using NetMQ.Sockets;

public partial class ZMQSubscriber : Node
{
	private SubscriberSocket subscriber;

	private Task recieveLoop;

	private List<Action<(string topic, string message)>> onRecieveMessage = [];

	public override async void _Ready()
	{
		subscriber = new SubscriberSocket();
		subscriber.Connect("tcp://10.121.17.45:5558"); // Verbinden met de publisher
		subscriber.Subscribe("stoplichten"); // Abonneer op "topic1"

		GD.Print("Subscriber verbonden en geabonneerd op stoplichten...");
		await RecieveMessageLoop();
	}


	public void DoOnMessage(Action<(string topic, string message)> action)
	{
		onRecieveMessage.Add(action);
	}


	// todo: deze method neemt aan dat het een multipart message krijgt met precies 2 parts. Als 1 bericht buiten de beugel valt gaat dit kapot.
	private async Task<(string topic, string message)> ReceiveMessages()
	{
		// Wacht tot er een bericht aankomt
		var message = await subscriber.ReceiveMultipartMessageAsync(expectedFrameCount: 2);
		if (message.FrameCount < 2)
		{
			throw new InvalidOperationException("Expected at least 2 frames in the message.");
		}
		var topic = message[0].ConvertToString();
		var content = message[1].ConvertToString();

		GD.Print($"Ontvangen bericht op {topic}: {message}");
		return (topic, content);
	}

	private async Task RecieveMessageLoop()
	{
		
		using (subscriber)
		{
			GD.Print("Waiting for multipart messages...");

			while (true)
			{
				// Receive the multipart message asynchronously as a list of strings
				List<string> messageParts = await Task.Run(() => subscriber.ReceiveMultipartStrings());

				GD.Print("Received multipart message:");
				for (int i = 0; i < messageParts.Count; i++)
				{
					GD.Print($"  Part {i + 1}: {messageParts[i]}");
				}
			}
		}
	}

	public override void _ExitTree()
	{
		subscriber.Close();
	}
}
