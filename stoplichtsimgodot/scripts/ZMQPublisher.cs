using System.Diagnostics;
using Godot;
using NetMQ;
using NetMQ.Sockets;

public partial class ZMQPublisher : Node, IMessagePublisher
{
	private PublisherSocket publisher;
	private string ip = "tcp://10.121.17.54:5557";

	public override void _Ready()
	{
		publisher = new PublisherSocket();
		publisher.Bind(ip); // Bind naar poort
	}

	public void Send(string topic, string message)
	{
		publisher.SendMoreFrame(topic).SendFrame(message);
		GD.Print(topic, "", message);
	}

	public override void _ExitTree()
	{
		publisher.Close();
	}
}
