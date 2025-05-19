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

	private int _wegdekCounter = 0;
	private int _waterCounter = 0;
	private int _fileCounter = 0;

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
			_wegdekCounter++;
			_sensoren[sensor.Name].Status = true;
		}
		else if (sensor.Name == "brug_water" && body is Boat)
		{
			_waterCounter++;
			_sensoren[sensor.Name].Status = true;
		}
		else if (sensor.Name == "brug_file" && body is Vehicle)
		{
			_fileCounter++;
			_sensoren[sensor.Name].Status = true;
		}
		PublishStatus();
	}

	private void OnSensorBodyExited(Area2D sensor, Node2D body)
	{
		if (sensor.Name == "brug_wegdek" && body is Vehicle && body is not Boat)
		{
			_wegdekCounter = Math.Max(0, _wegdekCounter - 1);
			_sensoren[sensor.Name].Status = _wegdekCounter > 0;
		}
		else if (sensor.Name == "brug_water" && body is Boat)
		{
			_waterCounter = Math.Max(0, _waterCounter - 1);
			_sensoren[sensor.Name].Status = _waterCounter > 0;
		}
		else if (sensor.Name == "brug_file" && body is Vehicle)
		{
			_fileCounter = Math.Max(0, _fileCounter - 1);
			_sensoren[sensor.Name].Status = _fileCounter > 0;
		}

		PublishStatus();
	}

	private void PublishStatus()
	{
		var statusDict = _sensoren.ToDictionary(
			kvp => kvp.Key,
			kvp => kvp.Value.Status
		);

		string json = System.Text.Json.JsonSerializer.Serialize(statusDict);

		_publisher!.Send(_topicName, json);
	}
}
