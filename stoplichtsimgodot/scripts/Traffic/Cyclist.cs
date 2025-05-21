using StoplichtSimGodot.scripts.Control;

namespace StoplichtSimGodot.scripts;

using System;
using System.Collections.Generic;
using Godot;

public partial class Cyclist : Vehicle
{
	public Cyclist()
	{
		Speed = 20;
		RoadToUse = "BikeRoad";
		IsMoving = true;
		SpawnChance = 5f;
	}

	private float _originalSpeed;
	private HashSet<CharacterBody2D> _overlapCount = new();
	private HashSet<Area2D> _lightOverlapCount = new();

	public override void _Ready()
	{
		_originalSpeed = Speed;
	}

	private void _on_voor_sensor_cyc_body_entered(CharacterBody2D body)
	{
		if (_lightOverlapCount.Count > 0) return;
		if (body is not Vehicle || body == this || body is Pedestrian || body is Cyclist) return;

		if (_overlapCount.Add(body))
		{
			Speed = 0f;
		}
	}

	private void _on_voor_sensor_cyc_body_exited(CharacterBody2D body)
	{
		if (body is not Vehicle || body == this || body is Pedestrian || body is Cyclist) return;

		_overlapCount.Remove(body);

		if (_overlapCount.Count == 0 && _lightOverlapCount.Count == 0)
		{
			Speed = _originalSpeed;
		}
	}

	private void _on_voor_sensor_area_entered(Area2D area)
	{
		if (area is not TrafficLight trafficLight) return;

		var currentPath = _pathFollow?.GetParent<Path2D>();
		if (currentPath == null || !trafficLight.AffectedPaths.Contains(currentPath))
			return;

		if (_lightOverlapCount.Add(area))
		{
			Speed = 0f;
		}
	}

	private void _on_voor_sensor_area_exited(Area2D area)
	{
		if (area is not TrafficLight trafficLight) return;

		var currentPath = _pathFollow?.GetParent<Path2D>();
		if (currentPath == null || !trafficLight.AffectedPaths.Contains(currentPath))
			return;

		_lightOverlapCount.Remove(area);

		if (_lightOverlapCount.Count == 0 && _overlapCount.Count == 0)
		{
			Speed = _originalSpeed;
		}
	}
}
