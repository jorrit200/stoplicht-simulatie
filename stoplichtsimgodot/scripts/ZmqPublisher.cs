using System.Net;
using Godot;
using NetMQ;
using NetMQ.Sockets;
using StoplichtSimGodot.interfaces;

namespace StoplichtSimGodot.scripts;

public partial class ZmqPublisher : Node, IMessagePublisher
{
	[Export] private string _ip;
	[Export] private int _port ;
	
	private PublisherSocket _publisher;
	private string _socketUri;

	public override void _Ready()
	{
		_publisher = new PublisherSocket();
		_socketUri = $"tcp://{_ip}:{_port}";
		_publisher.Bind(_socketUri);
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
