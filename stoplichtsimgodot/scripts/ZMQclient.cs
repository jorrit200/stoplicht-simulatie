using System;
using Godot;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;

public partial class ZMQClient : Node
{
	public override void _Ready()
	{
		GD.Print("Verbinding maken met ZeroMQ Server...");
		using (var client = new RequestSocket("tcp://127.0.0.1:5556"))
		{
			client.SendFrame("Hallo, ZeroMQ Server!");
			
			GD.Print("Bericht verzonden, wachten op antwoord...");
			
			// Manueel een timeout instellen (bijv. 1000 ms)
			int maxWaitTime = 1000; // 1000 ms = 1 sec
			int elapsedTime = 0;
			int step = 50; // Check elke 50 ms
			
			string response = null;
			while (elapsedTime < maxWaitTime)
			{
				if (client.TryReceiveFrameString(out response))
				{
					break; // Stop de loop als we een antwoord krijgen
				}
				Thread.Sleep(step);
				elapsedTime += step;
			}

			if (response != null)
			{
				GD.Print("Antwoord van server: " + response);
			}
			else
			{
				GD.PrintErr("Geen antwoord ontvangen van server binnen de tijd.");
			}
		}
	}
}
