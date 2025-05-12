using System;
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
	private Timer sendTimer;
	private string liveSimTime;
	private DateTime simStartTime;
	private string parsedSimTime;

	public override void _Ready()
	{
		_publisher = new PublisherSocket();
		_socketUri = $"tcp://{_ip}:{_port}";
		_publisher.Bind(_socketUri);
		
		simStartTime = DateTime.Now;
		sendTimer = new Timer();
		sendTimer.WaitTime = 1.0; // elke seconde
		sendTimer.OneShot = false;
		sendTimer.Autostart = true;
		AddChild(sendTimer);
		sendTimer.Timeout += OnSendTimerTimeout;
	}
	
	private void OnSendTimerTimeout()
	{
		liveSimTime = (DateTime.Now - simStartTime).ToString();
		parsedSimTime = ParseTime(liveSimTime);
		GD.Print(parsedSimTime);

		Send("sensoren_bruggen", "{\"81.1\":{\"state\":\"dicht\"}}");
		Send("tijd", $"{{\"simulatie_tijd_ms\":{parsedSimTime}}}");
	}
	
	private string ParseTime(string time)
	{
		TimeSpan timeSpan = TimeSpan.Parse(time);
		int wholeMilliseconds = (int)timeSpan.TotalMilliseconds;
		string milliseconds = wholeMilliseconds.ToString();
		return milliseconds;
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
