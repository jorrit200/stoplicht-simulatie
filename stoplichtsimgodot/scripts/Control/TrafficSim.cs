using Godot;

namespace StoplichtSimGodot.scripts;

public partial class TrafficSim : Node2D
{
	public override void _Ready()
	{
		GD.Print("TrafficSim Ready");
		var publisher = GetNode<ZmqPublisher>("ZMQPublisher");
		var sensorListener = GetNode<SensorListenerBitch>("Sensors");
		var sensorSpecial = GetNode<SensorenSpeciaal>("Sensoren_Speciaal");
		var voorrangsQueueManager = GetNode<VoorrangsQueueManager>("VoorrangsQueueManager");

		sensorListener.InjectPublisher(publisher);
		sensorSpecial.InjectPublisher(publisher);
		voorrangsQueueManager.InjectPublisher(publisher);
	}
}
