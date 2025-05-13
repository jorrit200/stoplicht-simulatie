using StoplichtSimGodot.scripts.Control;

namespace StoplichtSimGodot.scripts;
using System;
using Godot;

public partial class Cyclist : Vehicle
{
	public Cyclist()
	{
		Speed = 20;
		RoadToUse = "BikeRoad";
		IsMoving = true;
		SpawnChance = 5;
	}

	private float _originalSpeed;
	private int _overlapCount = 0;
	private int _lightOverlapCount = 0;

	public override void _Ready()
	{
		_originalSpeed = Speed;
	}

	private void _on_voor_sensor_cyc_body_entered(CharacterBody2D body)
	{
		if (_lightOverlapCount > 0) return;
		if (body is not Vehicle || body == this || body is Pedestrian || body is Cyclist) return;
		_overlapCount++;

		GetTree().CreateTween().Kill();

		var tween = CreateTween();
		tween.TweenProperty(this, "Speed", 0.0f, 1.0f)
			.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
	}

	private void _on_voor_sensor_cyc_body_exited(CharacterBody2D body)
	{
		if (body is not Vehicle || body == this || body is Pedestrian || body is Cyclist) return;
		_overlapCount = Math.Max(0, _overlapCount - 1);
		if (_overlapCount != 0) return;
		GetTree().CreateTween().Kill(); // Stop eventuele actieve tweens om conflicten te voorkomen

		// Maak een nieuwe tween aan om de snelheid te verhogen naar de originele snelheid over 1 seconde
		var tween = CreateTween();
		tween.TweenProperty(this, "Speed", _originalSpeed, 1.0f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In);
	}
	
	private void _on_voor_sensor_area_entered(Area2D area)
{
	if (area is not TrafficLight trafficLight) return;

	var currentPath = _pathFollow?.GetParent<Path2D>();
	if (currentPath == null || !trafficLight.AffectedPaths.Contains(currentPath))
		return;

	_lightOverlapCount++;

	GetTree().CreateTween().Kill();

	var tween = CreateTween();
	tween.TweenProperty(this, "Speed", 0.0f, 1.0f)
		.SetTrans(Tween.TransitionType.Sine)
		.SetEase(Tween.EaseType.Out);
}

private void _on_voor_sensor_area_exited(Area2D area)
{
	if (area is not TrafficLight trafficLight) return;

	var currentPath = _pathFollow?.GetParent<Path2D>();
	if (currentPath == null || !trafficLight.AffectedPaths.Contains(currentPath))
		return;

	_lightOverlapCount = Math.Max(0, _lightOverlapCount - 1);
	if (_lightOverlapCount != 0) return;

	GetTree().CreateTween().Kill();

	var tween = CreateTween();
	tween.TweenProperty(this, "Speed", _originalSpeed, 1.0f)
		.SetTrans(Tween.TransitionType.Sine)
		.SetEase(Tween.EaseType.In);
}
}
