using System;
using System.Net.Mime;
using System.Runtime.Serialization;
using Godot;
using Godot.Collections;
using Newtonsoft.Json;

namespace StoplichtSimGodot.scripts.Control;

public partial class BridgeDrawer : Sprite2D
{
	[Export] private TrafficLight _trafficLight;
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private ZmqPublisher _zmqPublisher;

	private BridgeState _bridgeState;

	[Signal]
	public delegate void BridgeStateChangedEventHandler(BridgeState newState);

	[Export]
	public BridgeState BridgeState
	{
		get => _bridgeState;
		set
		{
			if (value == _bridgeState) return;
			_bridgeState = value;
			EmitSignal(SignalName.BridgeStateChanged, Variant.From(value));
		}
	}

	public override void _Ready()
	{
		_trafficLight.StateChanged += (_, newState) =>
		{
			switch (newState)
			{
				case TrafficLightState.Green:
					_animationPlayer.Play("BridgeOpen");
					break;
				case TrafficLightState.Red:
					_animationPlayer.PlayBackwards("BridgeOpen");
					break;
				case TrafficLightState.Orange:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
			}
		};

		BridgeStateChanged += state =>
		{
			_zmqPublisher.Send("sensoren_bruggen",
				$"{{\"81.1\": {{\"state\": \"{state.ToString()}\"}}}}"
			);
		};

		base._Ready();
	}
}

public class BridgeStateContainer
{
	[JsonProperty("state")] public BridgeState State { get; set; }
}

public enum BridgeState
{
	open,
	dicht,
	onbekend
}
