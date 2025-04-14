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

	public override void _Ready()
	{
		subscriber = new SubscriberSocket();
		subscriber.Connect("tcp://10.121.17.88:5555"); // Verbinden met de publisher
		subscriber.Subscribe("stoplichten"); // Abonneer op "topic1"

		GD.Print("Subscriber verbonden en geabonneerd op stoplichten...");
		recieveLoop = RecieveMessageLoop();
	}


	public void DoOnMessage(Action<(string topic, string message)> action) {
		onRecieveMessage.Add(action);
	}


	// todo: deze method neemt aan dat het een multipart message krijgt met precies 2 parts. Als 1 bericht buiten de beugel valt gaat dit kapot.
	private async Task<(string topic, string message)> ReceiveMessages()
	{
		// Wacht tot er een bericht aankomt
		var (topic, continueing) = await subscriber.ReceiveFrameStringAsync();
		var (message, continueinger) = await subscriber.ReceiveFrameStringAsync();

		GD.Print($"Ontvangen bericht op {topic}: {message}");
		return (topic, message);
	}

	private async Task RecieveMessageLoop()
	{
		while (true)
		{
			var message = await ReceiveMessages();
			foreach (Action<(string topic, string message)> action in onRecieveMessage)
			{
				action.Invoke(message);
			}
		}
	}

	public override void _ExitTree()
	{
		subscriber.Close();
	}
}
