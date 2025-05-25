#nullable enable
using System.Runtime.Serialization;
using Godot;

namespace StoplichtSimGodot.scripts.Control;

public partial class TrafficLight : Area2D
{
	[Export]
	public TrafficLightState State
	{
		get => _state;
		set => ApplyState(value);
	}
	
	[Export]
	public Godot.Collections.Array<Path2D> AffectedPaths { get; set; } = [];
	
	[Signal]
	public delegate void StateChangedEventHandler(TrafficLightState oldState, TrafficLightState newState);

	
	private TrafficLightState _state = TrafficLightState.Red;
	private CollisionShape2D? CollisionShape { get; set; }

	
	private void ApplyState(TrafficLightState newState)
	{
		CallDeferred(nameof(SetState), Variant.From(newState));
	}

	private void SetState(TrafficLightState newState)
	{
		if (State != newState)
		{
			GD.Print($"{Name} changed state: {State} -> {newState}");
			EmitSignal(SignalName.StateChanged, Variant.From(State), Variant.From(newState));
		}

		_state = newState;
	}

	public override void _Ready()
	{
		CollisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");

		if (CollisionShape != null)
		{
			StateChanged += SetCollider;
		}

		var master = GetParent<TrafficLightMaster>();
		master.BindTrafficLight(this);
	}

	private void SetCollider(TrafficLightState oldState, TrafficLightState newState)
	{
		CollisionShape!.Disabled = newState == TrafficLightState.Green;
	}
}

//todo: move to a more sensible place
public enum TrafficLightState
{
	[EnumMember(Value = "groen")]
	Green,
	[EnumMember(Value = "oranje")]
	Orange,
	[EnumMember(Value = "rood")]
	Red
}
