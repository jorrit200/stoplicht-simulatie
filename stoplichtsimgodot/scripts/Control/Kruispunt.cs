using Godot;

namespace StoplichtSimGodot.scripts;

public partial class Kruispunt : Area2D
{

	private void _on_kruispunt_body_exited(Vehicle body)
	{
		GD.Print("Exited");
		if (body is EmergencyVehicle)
		{
			VoorrangsQueueManager.Instance?.Verwijder(body);
			VoorrangsQueueManager.Instance?.PubliceerQueue();
		}
	}
}
