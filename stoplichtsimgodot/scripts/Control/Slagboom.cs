using System;
using Godot;

namespace StoplichtSimGodot.scripts.Control;

public partial class Slagboom : Node2D
{
	[Export] private TrafficLight _trafficLight;
	[Export] private float _closingDelay;
	
	private Timer _delayTimer;
	private AnimationPlayer _animationPlayer;


	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_delayTimer = GetNode<Timer>("Timer");
		_trafficLight.StateChanged += OnTrafficLightStateChange;
		_delayTimer.Timeout += () =>
		{
			_animationPlayer.PlayBackwards("slagboom_open");
		};
		base._Ready();
	}

	private void OnTrafficLightStateChange(TrafficLightState oldState, TrafficLightState newState)
	{
		switch (newState)
		{
			case TrafficLightState.Red:
				_delayTimer.WaitTime = _closingDelay;
				_delayTimer.Start();
				break;
			case TrafficLightState.Green:
				_animationPlayer.Play("slagboom_open");
				break;
		}
	}
}
