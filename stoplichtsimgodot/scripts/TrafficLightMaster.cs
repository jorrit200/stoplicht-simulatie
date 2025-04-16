using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
using Newtonsoft.Json;
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
		GD.Print("Traffic Light: " + lightName + " is subscribed to the master");
	}

	private void OnMessage((string topic, string message) message)
	{
		if (message.topic != "stoplichten") return;

		try
		{
			var stoplichtData = JsonConvert.DeserializeObject<StoplichtenDto>(message.message);
			foreach (var (key, newState) in stoplichtData)
			{
				GD.Print(key, newState);
				var subbedLight = _subscribedLights.GetValueOrDefault(key);
				subbedLight?.ApplyState(newState);
			}
		}
		catch (Exception e)
		{
			GD.Print("PARSING ERROR: ", e.Message);
		}
		
	}

	public override void _Ready()
	{
		subscriber.DoOnMessage(OnMessage);
	}
}
