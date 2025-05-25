using Godot;

namespace StoplichtSimGodot.scripts;

public partial class Kruispunt : Area2D
{

	private void _on_kruispunt_body_exited(Vehicle body)
	{
		if (body is EmergencyVehicle or Bus)
		{
			VoorrangsQueueManager.Instance?.Verwijder((Vehicle)body);
		}
	}	
}
