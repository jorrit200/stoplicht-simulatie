using System.Collections.Generic;
using System.Text.Json;
using Godot;
using StoplichtSimGodot.dto;

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
		if (message.topic != "stoplichten") return;

		var stoplichtData = JsonSerializer.Deserialize<StoplichtenDto>(message.message);

		foreach (var (key, newState) in stoplichtData)
		{
			var subbedLight = _subscribedLights.GetValueOrDefault(key);
			if (subbedLight is null) continue;
			subbedLight.SetState(newState);
		}
	}

	public override void _Ready()
	{
		subscriber.DoOnMessage(OnMessage);
	}
}
