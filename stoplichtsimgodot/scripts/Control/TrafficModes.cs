using Godot;
using StoplichtSimGodot.scripts;
using System;

public partial class TrafficModes : Control
{
	private CheckButton _rustigButton;
	private CheckButton _spitsButton;
	private CheckButton _stressButton;

	private VehicleSpawner _vehicleSpawner;

	public override void _Ready()
	{
		_rustigButton = GetNode<CheckButton>("Rustig");
		_spitsButton = GetNode<CheckButton>("Spits");
		_stressButton = GetNode<CheckButton>("Stress");

		_vehicleSpawner = GetParent().GetNode<VehicleSpawner>("VehicleSpawner");

		_spitsButton.ButtonPressed = true;
		UpdateSpawnDelay(_spitsButton);

		_rustigButton.Toggled += (bool pressed) => OnCheckButtonToggled(_rustigButton, pressed);
		_spitsButton.Toggled += (bool pressed) => OnCheckButtonToggled(_spitsButton, pressed);
		_stressButton.Toggled += (bool pressed) => OnCheckButtonToggled(_stressButton, pressed);
	}

	private void OnCheckButtonToggled(CheckButton toggledButton, bool pressed)
	{
		if (pressed)
		{
			if (toggledButton != _rustigButton) _rustigButton.ButtonPressed = false;
			if (toggledButton != _spitsButton) _spitsButton.ButtonPressed = false;
			if (toggledButton != _stressButton) _stressButton.ButtonPressed = false;

			UpdateSpawnDelay(toggledButton);
		}
		else
		{
			if (!_rustigButton.ButtonPressed && !_spitsButton.ButtonPressed && !_stressButton.ButtonPressed)
			{
				toggledButton.ButtonPressed = true;
			}
		}
	}

	private void UpdateSpawnDelay(CheckButton activeButton)
	{
		if (_vehicleSpawner == null)
		{
			return;
		}

		if (activeButton == _rustigButton)
			_vehicleSpawner.SetSpawnDelay(2.0f);
		else if (activeButton == _spitsButton)
			_vehicleSpawner.SetSpawnDelay(1.3f);
		else if (activeButton == _stressButton)
			_vehicleSpawner.SetSpawnDelay(0.8f);
	}
}
