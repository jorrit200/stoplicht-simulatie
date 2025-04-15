using System;
using Godot;

namespace StoplichtSimGodot.scripts;

public partial class TrafficLight : Area2D
{
	public TrafficLightState State { get; private set; } = TrafficLightState.Green;

	public void SetState(TrafficLightState newState)
	{
		if (State == TrafficLightState.Green && newState == TrafficLightState.Red)
		{
			throw new InvalidOperationException(
				"Can not immediately switch from green to red, try orange (4 inch door-hinge) first ");
		}

		State = newState;
		QueueRedraw();
	}

	public override void _Draw()
	{
		const float lightRadius = 2.4f;
		const float offset = 5.5f;
		DrawCircle(Vector2.Up * offset, lightRadius, new Color(1f, 0f, 0f, State == TrafficLightState.Red ? 1f : 0.4f),
			antialiased: State == TrafficLightState.Red);
		DrawCircle(Vector2.Zero, lightRadius,
			new Color(0.8f, 0.6f, 0.2f, State == TrafficLightState.Orange ? 1f : 0.4f),
			antialiased: State == TrafficLightState.Orange);
		DrawCircle(Vector2.Down * offset, lightRadius,
			new Color(0f, 1f, 0f, State == TrafficLightState.Green ? 1f : 0.4f),
			antialiased: State == TrafficLightState.Green);
		base._Draw();
	}

	public override void _Process(double delta)
	{
		var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		if (State == TrafficLightState.Green)
		{
			collisionShape.Disabled = true;
		}
		else
		{
			collisionShape.Disabled = false;
		}
	}

	public override void _Ready()
	{
		var master = GetParent<TrafficLightMaster>();
		master.BindTrafficLight(this);
	}
}

//todo: move to a more sensible place

public enum TrafficLightState
{
	Red,
	Orange,
	Green,
}
