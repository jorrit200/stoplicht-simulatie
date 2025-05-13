using Godot;

namespace StoplichtSimGodot.scripts.Control;

public partial class LightDrawer : Node2D
{

	[Export] private TrafficLight _trafficLight;
	[Export] private float _lightRadius = 2.4f;
	[Export] private float _offset = 5.5f;
	
	public override void _Ready()
	{
		_trafficLight.StateChanged += (_, _) => { QueueRedraw(); };
		base._Ready();
	}

	public override void _Draw()
	{
		

		DrawCircle(Vector2.Up * _offset, _lightRadius, new Color(1f, 0f, 0f, _trafficLight.State == TrafficLightState.Red ? 1f : 0.4f),
			antialiased: _trafficLight.State == TrafficLightState.Red);

		DrawCircle(Vector2.Zero, _lightRadius,
			new Color(0.8f, 0.6f, 0.2f, _trafficLight.State == TrafficLightState.Orange ? 1f : 0.4f),
			antialiased: _trafficLight.State == TrafficLightState.Orange);

		DrawCircle(Vector2.Down * _offset, _lightRadius,
			new Color(0f, 1f, 0f, _trafficLight.State == TrafficLightState.Green ? 1f : 0.4f),
			antialiased: _trafficLight.State == TrafficLightState.Green);

		base._Draw();
	}
}
