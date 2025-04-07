using System;
using Godot;

namespace StoplichtSimGodot.scripts;

public partial class TrafficLight : Node2D
{
	public TrafficLightState State { get; private set; } = TrafficLightState.Red;
	
	public void SetState(TrafficLightState newState) 
	{
		if (State == TrafficLightState.Green && newState == TrafficLightState.Red)
		{
			throw new InvalidOperationException(
				"Can not immediately switch from green to red, try orange (4 inch door-hinge) first ");
		}
		State = newState;
	}

	public override void _Draw()
	{
		const float lightRadius = 2f;
		const float offset = 5.5f;
		DrawCircle(Vector2.Up * offset, lightRadius, new Color(1f, 0f, 0f, State == TrafficLightState.Red ? 1f : 0.4f), antialiased: State == TrafficLightState.Red);
		DrawCircle(Vector2.Zero, lightRadius, new Color(0.8f, 0.6f, 0.2f, State == TrafficLightState.Orange ? 1f : 0.4f), antialiased: State == TrafficLightState.Orange);
		DrawCircle(Vector2.Down * offset, lightRadius, new Color(0f, 1f, 0f, State == TrafficLightState.Green ? 1f : 0.4f), antialiased: State == TrafficLightState.Green);
		base._Draw();
	}
}


//todo: move to a more sensible place
public enum TrafficLightState {
	Red,
	Orange,
	Green,
}
