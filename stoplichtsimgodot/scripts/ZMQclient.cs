using Godot;
using NetMQ;
using NetMQ.Sockets;

public partial class ZMQClient : Node
{
	public override void _Ready()
	{
		GD.Print("Verbinding maken met ZeroMQ Server...");
		using (var client = new RequestSocket(">tcp://localhost:5555"))
		{
			client.SendFrame("Hallo, ZeroMQ Server!");
			
			if (client.TryReceiveFrameString(out string response))
			{
				GD.Print("Antwoord van server: " + response);
			}
		}
	}
}
