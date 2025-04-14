using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;

public partial class SensorListenerBitch : Node
{
	private IMessagePublisher _publisher;
	private SensorStatusData _sensoren = new SensorStatusData();

	public void InjectPublisher(IMessagePublisher publisher)
	{
		_publisher = publisher;
	}

	private string topicName = "sensoren_rijbaan";

	public override void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child is Area2D sensor)
			{
				sensor.BodyEntered += (body) => OnSensorBodyEntered(sensor, body);
				sensor.BodyExited += (body) => OnSensorBodyExited(sensor, body);
				Sensoren_Rijbaan sensoren_Rijbaan = new Sensoren_Rijbaan { Voor = false, Achter = false };
				string id = ParseSensorId(child.Name);
				_sensoren[id] = sensoren_Rijbaan;
			}
		}
	}

	private void OnSensorBodyEntered(Area2D sensor, Node body)
	{
		//GD.Print($"[ENTER] {sensor.Name} detected body: {body.Name}");

		string sensorName = sensor.Name;
		string id = ParseSensorId(sensor.Name);

		var currentSensor = _sensoren.GetValueOrDefault(id)!;

		if (sensorName.Contains("voor"))
		{
			currentSensor.Voor = true;
		}
		else if (sensorName.Contains("achter"))
		{
			currentSensor.Achter = true;
		}

		_sensoren[id] = currentSensor;

		//Modify sensor add ID set boolean state and convert sensor to JSON, convert JSON to string
		string json = System.Text.Json.JsonSerializer.Serialize(_sensoren);

		_publisher?.Send(topicName, json);
	}

	private void OnSensorBodyExited(Area2D sensor, Node body)
	{
		//GD.Print($"[EXIT] {sensor.Name} detected body: {body.Name}");

		string sensorName = sensor.Name;
		string id = ParseSensorId(sensor.Name);

		var currentSensor = _sensoren.GetValueOrDefault(id)!;

		if (sensorName.Contains("voor"))
		{
			currentSensor.Voor = false;
		}
		else if (sensorName.Contains("achter"))
		{
			currentSensor.Achter = false;
		}

		_sensoren[id] = currentSensor;

		//Modify sensor add ID set boolean state and convert sensor to JSON, convert JSON to string
		string json = System.Text.Json.JsonSerializer.Serialize(_sensoren);

		_publisher?.Send(topicName, json);

	}

	private string ParseSensorId(string sensorName)
	{
		if (sensorName.Contains("voor") || sensorName.Contains("achter"))
			return sensorName.Replace("_", ".").Replace(".voor", "").Replace(".achter", "");

		GD.Print($"[ERROR] {sensorName} is not a valid sensor name.");
		return null;
	}
}
