using System;
using Godot;
using StoplichtSimGodot.scripts.Control;

namespace StoplichtSimGodot.scripts;

public partial class Vehicle : CharacterBody2D
{
	protected float Speed;
	protected string RoadToUse;
	protected bool IsMoving;
	public float SpawnChance;

	private float _originalSpeed;
	private int _overlapCount = 0;
	private int _lightOverlapCount = 0;

	public PathFollow2D _pathFollow;

	public void StartMoving(PathFollow2D followPath)
	{
		_pathFollow = followPath;
		IsMoving = true;
	}

	public override void _Ready()
	{
		_originalSpeed = Speed;
	}

	public override void _Process(double delta)
	{
		if (!IsMoving || _pathFollow == null) return;

		_pathFollow.Progress += Speed * (float)delta;

		if (_pathFollow.ProgressRatio >= 0.99f)
		{
			QueueFree();
		}
	}

	private void _on_voor_sensor_body_entered(CharacterBody2D body)
	{
		if (body is not Vehicle || body == this) return;
		_overlapCount++;

		Speed = 0f;
	}

	private void _on_voor_sensor_body_exited(CharacterBody2D body)
	{
		if (body is not Vehicle || body == this) return;

		_overlapCount = Math.Max(0, _overlapCount - 1);

		if (_overlapCount == 0 && _lightOverlapCount == 0)
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

		_lightOverlapCount++;

		Speed = 0f;
	}

	private void _on_voor_sensor_area_exited(Area2D area)
	{
		if (area is not TrafficLight trafficLight) return;

		var currentPath = _pathFollow?.GetParent<Path2D>();
		if (currentPath == null || !trafficLight.AffectedPaths.Contains(currentPath))
			return;

		_lightOverlapCount = Math.Max(0, _lightOverlapCount - 1);

		if (_lightOverlapCount == 0 && _overlapCount == 0)
		{
			Speed = _originalSpeed;
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		VoorrangsQueueManager.Instance?.Verwijder(this);
	}
}
