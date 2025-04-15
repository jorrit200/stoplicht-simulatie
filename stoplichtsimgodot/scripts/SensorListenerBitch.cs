using System.Collections.Generic;
using System.Linq;
using Godot;
using StoplichtSimGodot.dto;
using StoplichtSimGodot.interfaces;

namespace StoplichtSimGodot.scripts;

public partial class SensorListenerBitch : Node
{
	private IMessagePublisher _publisher;
	private SensorStatusData _sensoren = new();
	private string _topicName = "sensoren_rijbaan";

	public void InjectPublisher(IMessagePublisher publisher)
	{
		_publisher = publisher;
	}

	public override void _Ready()
	{

		_sensoren = GetChildren()
			.OfType<Area2D>()
			.Select(sensor =>
			{
				sensor.BodyEntered += body => OnSensorBodyEntered(sensor, body);
				sensor.BodyExited += body => OnSensorBodyExited(sensor, body);
				var sensorenRijbaan = new SensorenRijbaan { Voor = false, Achter = false };
				var id = ParseSensorId(sensor.Name);
				return KeyValuePair.Create(id, sensorenRijbaan);
			})
			.ToDictionary() as SensorStatusData;
	}

	private void OnSensorBodyEntered(Area2D sensor, Node body)
	{
		string sensorName = sensor.Name;
		var id = ParseSensorId(sensor.Name);

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
		var json = System.Text.Json.JsonSerializer.Serialize(_sensoren);
		_publisher!.Send(_topicName, json);
	}

	private void OnSensorBodyExited(Area2D sensor, Node body)
	{
		string sensorName = sensor.Name;
		var id = ParseSensorId(sensor.Name);
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
		var json = System.Text.Json.JsonSerializer.Serialize(_sensoren);
		_publisher!.Send(_topicName, json);
	}

	private static string ParseSensorId(string sensorName)
	{
		if (sensorName.Contains("voor") || sensorName.Contains("achter"))
			return sensorName.Replace("_", ".").Replace(".voor", "").Replace(".achter", "");

		GD.Print($"[ERROR] {sensorName} is not a valid sensor name.");
		return null;
	}
}
