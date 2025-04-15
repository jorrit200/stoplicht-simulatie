using Godot;
using NetMQ;
using NetMQ.Sockets;
using StoplichtSimGodot.interfaces;

namespace StoplichtSimGodot.scripts;

public partial class ZmqPublisher : Node, IMessagePublisher
{
	private PublisherSocket _publisher;
	private string _ip = "tcp://*:5559";

	public override void _Ready()
	{
		_publisher = new PublisherSocket();
		_publisher.Bind(_ip); // Bind naar poort
	}

	public void Send(string topic, string message)
	{
		_publisher.SendMoreFrame(topic).SendFrame(message);
	}

	public override void _ExitTree()
	{
		_publisher.Close();
	}
}
