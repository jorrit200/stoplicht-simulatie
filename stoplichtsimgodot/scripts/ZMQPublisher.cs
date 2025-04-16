using System;
using System.Diagnostics;
using Godot;
using Microsoft.VisualBasic;
using NetMQ;
using NetMQ.Sockets;

public partial class ZMQPublisher : Node, IMessagePublisher
{
	private PublisherSocket publisher;
	private string ip = "tcp://10.121.17.54:5559";
	private Timer sendTimer;
	private string liveSimTime;
	private DateTime simStartTime;
	private string parsedSimTime;

	public override void _Ready()
	{
		publisher = new PublisherSocket();
		publisher.Bind(ip); // Bind naar poort
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
		GD.Print("Sending on topics");
		liveSimTime = (DateTime.Now - simStartTime).ToString();
		parsedSimTime = ParseTime(liveSimTime);
		GD.Print(parsedSimTime);

		Send("sensoren_bruggen", "{\"81\":{\"state\":\"dicht\"}}");
		Send("sensoren_speciaal", "{\"brug_wegdek\":false, \"brug_water\":false, \"brug_file\":false}");
		Send("tijd", $"{{\"simulatie_tijd_ms\":{parsedSimTime}}}");
		Send("voorrangsvoertuig", "{\"queue\": [{\"baan\":\"8.2\",\"simulatie_tijd_ms\": 1231542,\"prioriteit\": 1},{\"baan\": \"3.1\",\"simulatie_tijd_ms\": 1230000,\"prioriteit\": 2}]}");
	}

	public void Send(string topic, string message)
	{
		publisher.SendMoreFrame(topic).SendFrame(message);
		//GD.Print(topic, "", message);
	}

	public override void _ExitTree()
	{
		publisher.Close();
	}

	private string ParseTime(string time)
	{
		TimeSpan timeSpan = TimeSpan.Parse(time);
		int wholeMilliseconds = (int)timeSpan.TotalMilliseconds;
		string milliseconds = wholeMilliseconds.ToString();
		return milliseconds;
	}
}
