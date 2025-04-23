using System.Collections.Generic;
using System.Linq;
using Godot;
using StoplichtSimGodot.dto;
using StoplichtSimGodot.interfaces;

namespace StoplichtSimGodot.scripts;

public partial class SensorListenerBitch : Node
{
	private IMessagePublisher _publisher;
	private SensorStatusData _sensoren;
	private string _topicName = "sensoren_rijbaan";

	public void InjectPublisher(IMessagePublisher publisher)
	{
		_publisher = publisher;
	}

	public override void _Ready()
	{
		_sensoren = new SensorStatusData();

		foreach (var sensor in GetChildren().OfType<Area2D>())
		{
			var sensorPosition = sensor.Name.ToString().Contains("voor")
				? SensorPosition.Voor
				: SensorPosition.Achter;
			sensor.BodyEntered += body => OnSensorBodyEntered(sensor, body, sensorPosition);
			sensor.BodyExited += body => OnSensorBodyExited(sensor, body, sensorPosition);
			var sensorenRijbaan = new SensorenRijbaan { Voor = false, Achter = false };
			var id = ParseSensorId(sensor.Name);
			if (sensorPosition != SensorPosition.Voor) continue;
			_sensoren.Add(id, sensorenRijbaan);
		}
	}

	private void OnSensorBodyEntered(Area2D sensor, Node body, SensorPosition position)
	{
		// Word body meegeven omdat er in de toekomst misschien gecontrolleerd moet worden of dit wel een vehicle is?
		var id = ParseSensorId(sensor.Name);

		var currentSensor = _sensoren.GetValueOrDefault(id)!;

		if (position == SensorPosition.Voor)
			currentSensor.Voor = true;
		else
			currentSensor.Achter = true;

		_sensoren[id] = currentSensor;
		var json = System.Text.Json.JsonSerializer.Serialize(_sensoren);
		_publisher!.Send(_topicName, json);
	}

	private void OnSensorBodyExited(Area2D sensor, Node body, SensorPosition position)
	{
		// Word body meegeven omdat er in de toekomst misschien gecontrolleerd moet worden of dit wel een vehicle is?
		var id = ParseSensorId(sensor.Name);
		var currentSensor = _sensoren.GetValueOrDefault(id)!;

		if (position == SensorPosition.Voor)
			currentSensor.Voor = false;
		else
			currentSensor.Achter = false;

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

internal enum SensorPosition
{
	Voor,
	Achter
}
