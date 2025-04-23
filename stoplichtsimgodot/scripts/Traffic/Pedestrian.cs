namespace StoplichtSimGodot.scripts;
using System;
using Godot;

public partial class Pedestrian : Vehicle
{
	public Pedestrian()
	{
		Speed = 6;
		RoadToUse = "PedRoad";
		IsMoving = true;
		SpawnChance = 5;
	}
	private float _originalSpeed;
	private int _overlapCount = 0;

	public override void _Ready()
	{
		_originalSpeed = Speed;
	}

	private void _on_voor_sensor_ped_body_entered(CharacterBody2D body)
	{
		if (body is not Vehicle || body == this || body is Pedestrian || body is Cyclist) return;
		_overlapCount++;

		GetTree().CreateTween().Kill();

		var tween = CreateTween();
		tween.TweenProperty(this, "Speed", 0.0f, 1.0f)
			.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
	}

	private void _on_voor_sensor_ped_body_exited(CharacterBody2D body)
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
}
