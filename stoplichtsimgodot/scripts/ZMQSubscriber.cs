using Godot;
using NetMQ;
using NetMQ.Sockets;

public partial class ZMQSubscriber : Node
{
	private SubscriberSocket subscriber;

	public override void _Ready()
	{
		subscriber = new SubscriberSocket();
		subscriber.Connect("tcp://10.121.17.6:5556"); // Verbinden met de publisher
		subscriber.Subscribe("stoplichten"); // Abonneer op "topic1"

		GD.Print("Subscriber verbonden en geabonneerd op stoplichten...");

		// Start een thread om berichten te ontvangen zonder = teken
		System.Threading.Thread receiveThread = new System.Threading.Thread(ReceiveMessages);
		receiveThread.Start();  // Start de thread
	}

	private void ReceiveMessages()
	{
		while (true)
		{
			// Wacht tot er een bericht aankomt
			string topic = subscriber.ReceiveFrameString();
			string message = subscriber.ReceiveFrameString();

			GD.Print($"Ontvangen bericht op {topic}: {message}");
		}
	}

	public override void _ExitTree()
	{
		subscriber.Close();
	}
}
