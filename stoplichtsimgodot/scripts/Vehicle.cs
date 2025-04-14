using Godot;
using StoplichtSimGodot.scripts;
using System;

public partial class Vehicle : CharacterBody2D
{
	public float speed;
	public string roadToUse;
	public bool isMoving;
	public float spawnChance;

	private float originalSpeed;
	private int overlapCount = 0;

	private int lightOverlapCount = 0;

	private PathFollow2D pathFollow;

	public void StartMoving(PathFollow2D followPath)
	{
		pathFollow = followPath;
		isMoving = true;
	}

	public override void _Ready()
	{
		originalSpeed = speed;
	}

	public override void _Process(double delta)
	{
		if (isMoving && pathFollow != null)
		{
			pathFollow.Progress += speed * (float)delta;

			if (pathFollow.ProgressRatio >= 0.99f)
			{
				QueueFree();
			}
		}
	}

	private void _on_voor_sensor_body_entered(CharacterBody2D body)
	{
		if (body is Vehicle && body != this)
		{
			overlapCount++;

			GetTree().CreateTween().Kill();

			var tween = CreateTween();
			tween.TweenProperty(this, "speed", 0.0f, 1.0f)
			.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
		}
	}

	private void _on_voor_sensor_body_exited(CharacterBody2D body)
	{
		if (body is Vehicle && body != this)
		{
			overlapCount = Math.Max(0, overlapCount - 1);
			if (overlapCount == 0)
			{
				// Stop eventuele actieve tweens om conflicten te voorkomen
				GetTree().CreateTween().Kill();

				// Maak een nieuwe tween aan om de snelheid te verhogen naar de originele snelheid over 1 seconde
				var tween = CreateTween();
				tween.TweenProperty(this, "speed", originalSpeed, 1.0f)
					 .SetTrans(Tween.TransitionType.Sine)
					 .SetEase(Tween.EaseType.In);
			}
		}
	}

	private void _on_voor_sensor_area_entered(Area2D area)
	{
		if (area is TrafficLight light)
		{
			lightOverlapCount++;

			GetTree().CreateTween().Kill();

			var tween = CreateTween();
			tween.TweenProperty(this, "speed", 0.0f, 1.0f)
			.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
		}
	}

	private void _on_voor_sensor_area_exited(Area2D area)
	{
		if (area is TrafficLight light)
		{
			lightOverlapCount = Math.Max(0, lightOverlapCount - 1);
			if (lightOverlapCount == 0)
			{
				// Stop eventuele actieve tweens om conflicten te voorkomen
				GetTree().CreateTween().Kill();

				// Maak een nieuwe tween aan om de snelheid te verhogen naar de originele snelheid over 1 seconde
				var tween = CreateTween();
				tween.TweenProperty(this, "speed", originalSpeed, 1.0f)
					.SetTrans(Tween.TransitionType.Sine)
					.SetEase(Tween.EaseType.In);
			}
		}
	}
}
