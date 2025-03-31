using Godot;
using NetMQ;
using NetMQ.Sockets;

public partial class ZMQPublisher : Node
{
	private PublisherSocket publisher;

	public override void _Ready()
	{
		publisher = new PublisherSocket();
		publisher.Bind("tcp://127.0.0.1:5556"); // Bind naar poort

		GD.Print("Publisher gestart op poort 5556...");

		// Start een thread om berichten te sturen zonder = teken
		System.Threading.Thread sendThread = new System.Threading.Thread(SendMessages);
		sendThread.Start();  // Start de thread
	}

	private void SendMessages()
	{
		while (true)
		{
			// Stuur berichten naar een topic
			publisher.SendMoreFrame("sensoren_rijbaan").SendFrame("\"1.1\": {\"voor\": false,\"achter\": false}");

			// Wacht even tussen berichten
			System.Threading.Thread.Sleep(1000);  // Bericht elke seconde
		}
	}

	public override void _ExitTree()
	{
		publisher.Close();
	}
}
