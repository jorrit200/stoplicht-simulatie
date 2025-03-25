using Godot;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;

public partial class ZMQServer : Node
{
	private Thread _serverThread;
	private bool _isRunning = true;

	public override void _Ready()
	{
		// Start de server in een aparte thread
		_serverThread = new Thread(ServerLoop);
		_serverThread.Start();
	}

	private void ServerLoop()
	{
		using (var server = new ResponseSocket("@tcp:/127.0.0.1:5556")) // Luistert op poort 5555
		{
			GD.Print("ZeroMQ Server gestart op poort 5556...");
			
			while (_isRunning)
			{
				if (server.TryReceiveFrameString(out string message))
				{
					GD.Print("Ontvangen bericht: " + message);
					server.SendFrame("Bericht ontvangen: " + message);
				}
			}
		}
	}

	public override void _ExitTree()
	{
		_isRunning = false;
		_serverThread.Join();
	}
}
