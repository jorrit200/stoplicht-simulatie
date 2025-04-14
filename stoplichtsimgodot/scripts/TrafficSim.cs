using Godot;

namespace StoplichtSimGodot.scripts;

public partial class TrafficSim : Node2D
{
	public override void _Ready()
	{
		GD.Print("TrafficSim Ready");
		var publisher = GetNode<ZmqPublisher>("ZMQPublisher");
		var sensorListener = GetNode<SensorListenerBitch>("Sensors");

		sensorListener.InjectPublisher(publisher);
	}

	//private double _timeAccumulator = 0;

	// public override void _Process(double delta)
	// {
	// 	_timeAccumulator += delta;

	// 	if (_timeAccumulator >= 5.0)
	// 	{
	// 		_timeAccumulator = 0;

	// 		var stoplicht = GetNode<TrafficLight>("TrafficLightMaster/1_1");
	// 		if (stoplicht.State == TrafficLightState.Green)
	// 		{
	// 			stoplicht.SetState(TrafficLightState.Orange);
	// 			GD.Print("Switching to orange");
	// 		}
	// 		else if (stoplicht.State == TrafficLightState.Orange)
	// 		{
	// 			stoplicht.SetState(TrafficLightState.Red);
	// 			GD.Print("Switching to red");
	// 		}
	// 		else if (stoplicht.State == TrafficLightState.Red)
	// 		{
	// 			stoplicht.SetState(TrafficLightState.Green);
	// 			GD.Print("Switching to green");
	// 		}
	// 	}
	// }
}
