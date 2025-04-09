using Godot;
using System;

public partial class TrafficSim : Node2D
{
	public override void _Ready()
	{
		var publisher = GetNode<ZMQPublisher>("ZMQPublisher");
		var sensorListener = GetNode<SensorListenerBitch>("Sensors");

		sensorListener.InjectPublisher(publisher);
	}
}
