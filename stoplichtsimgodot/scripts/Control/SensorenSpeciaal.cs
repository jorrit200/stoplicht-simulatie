using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using StoplichtSimGodot.dto;
using StoplichtSimGodot.interfaces;

namespace StoplichtSimGodot.scripts;
public partial class SensorenSpeciaal : Node2D
{
	private Dictionary<string, dto.SensorenSpeciaal> _sensoren;
	private string _topicName = "sensoren_speciaal";
	private IMessagePublisher _publisher;

	public override void _Ready()
	{
		_sensoren = new Dictionary<string, dto.SensorenSpeciaal>();

		foreach (var sensor in GetChildren().OfType<Area2D>())
		{
			sensor.BodyEntered += body => OnSensorBodyEntered(sensor, body);
			sensor.BodyExited += body => OnSensorBodyExited(sensor, body);
			_sensoren.Add(sensor.Name, new dto.SensorenSpeciaal { Status = false });
		}
	}

	public void InjectPublisher(IMessagePublisher publisher)
	{
		_publisher = publisher;
	}

	private void OnSensorBodyEntered(Area2D sensor, Node body)
	{
		if (sensor.Name == "brug_wegdek" && body is Vehicle && body is not Boat)
		{
			_sensoren[sensor.Name].Status = true;
		}
		else if (sensor.Name == "brug_water" && body is Boat)
		{
			_sensoren[sensor.Name].Status = true;
		}
		else if (sensor.Name == "brug_file" && body is Vehicle)
		{
			_sensoren[sensor.Name].Status = true;
		}
		PublishStatus();
	}

	private void OnSensorBodyExited(Area2D sensor, Node2D body)
	{
		if (_sensoren.ContainsKey(sensor.Name))
		{
			_sensoren[sensor.Name].Status = false;
			PublishStatus();
		}
	}

	private void PublishStatus()
	{
		// Maak een eenvoudige dictionary: { "brug_wegdek": true, ... }
		var statusDict = _sensoren.ToDictionary(
			kvp => kvp.Key,
			kvp => kvp.Value.Status
		);

		// Zet om naar JSON
		string json = System.Text.Json.JsonSerializer.Serialize(statusDict);

		GD.Print(_topicName, json); // Debug

		// Verstuur via publisher
		_publisher!.Send(_topicName, json);
	}
}
