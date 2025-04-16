using System.Collections.Generic;
using Godot;

namespace StoplichtSimGodot.scripts;

public partial class TrafficLightMaster : Node
{
	[Export] private ZmqSubscriber subscriber;
	private readonly Dictionary<string, TrafficLight> _subscribedLights = new();

	public void BindTrafficLight(TrafficLight light)
	{
		string lightName = light.Name;

		lightName = lightName.Replace("_", ".");

		_subscribedLights.Add(lightName, light);
	}

	private void OnMessage((string topic, string message) message)
	{
		GD.Print("Master ontvangt dingen");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		subscriber.DoOnMessage(OnMessage);
	}
}
