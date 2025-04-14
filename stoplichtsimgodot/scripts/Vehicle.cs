using System;
using Godot;

namespace StoplichtSimGodot.scripts;

public partial class Vehicle : CharacterBody2D
{
    public float Speed;
    public string RoadToUse;
    public bool IsMoving;
    public float SpawnChance;

    private float _originalSpeed;
    private int _overlapCount = 0;

    private int _lightOverlapCount = 0;

    private PathFollow2D _pathFollow;

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
        if (IsMoving && _pathFollow != null)
        {
            _pathFollow.Progress += Speed * (float)delta;

            if (_pathFollow.ProgressRatio >= 0.99f)
            {
                QueueFree();
            }
        }
    }

    private void _on_voor_sensor_body_entered(CharacterBody2D body)
    {
        if (body is Vehicle && body != this)
        {
            _overlapCount++;

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
            _overlapCount = Math.Max(0, _overlapCount - 1);
            if (_overlapCount == 0)
            {
                // Stop eventuele actieve tweens om conflicten te voorkomen
                GetTree().CreateTween().Kill();

                // Maak een nieuwe tween aan om de snelheid te verhogen naar de originele snelheid over 1 seconde
                var tween = CreateTween();
                tween.TweenProperty(this, "speed", _originalSpeed, 1.0f)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.In);
            }
        }
    }

    private void _on_voor_sensor_area_entered(Area2D area)
    {
        if (area is TrafficLight light)
        {
            _lightOverlapCount++;

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
            _lightOverlapCount = Math.Max(0, _lightOverlapCount - 1);
            if (_lightOverlapCount == 0)
            {
                // Stop eventuele actieve tweens om conflicten te voorkomen
                GetTree().CreateTween().Kill();

                // Maak een nieuwe tween aan om de snelheid te verhogen naar de originele snelheid over 1 seconde
                var tween = CreateTween();
                tween.TweenProperty(this, "speed", _originalSpeed, 1.0f)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.In);
            }
        }
    }
}