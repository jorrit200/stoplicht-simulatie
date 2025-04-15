using System.Collections.Generic;
using Godot;

namespace StoplichtSimGodot.scripts;

public partial class VehicleSpawner : Node2D
{
	private readonly List<PackedScene> _vehicles = new List<PackedScene>();
	private readonly List<PathFollow2D> _paths = new List<PathFollow2D>();

	[Export] public PackedScene Car;
	[Export] public PackedScene Bus;
	[Export] public PackedScene EmergencyVehicle;

	private readonly HashSet<Path2D> _sharedSpawnPaths = new HashSet<Path2D>();
	private float _sharedCooldown = 0f;
	private float _sharedCooldownTime = 1.5f;

	public override void _Ready()
	{
		GD.Print("VehicleSpawner is gestart!");

		_vehicles.Add(Car);
		_vehicles.Add(Bus);
		_vehicles.Add(EmergencyVehicle);

		// Haal de PathFollow2D nodes op (paden)
		for (var i = 1; i < 13; i++)
		{
			_paths.Add(GetNode<PathFollow2D>($"../Paths/Path2D{i}/Path{i}"));
		}

		_paths.Add(GetNode<PathFollow2D>("../Paths/Path2D2_2/Path2_2"));
		_paths.Add(GetNode<PathFollow2D>("../Paths/Path2D8_2/Path8_2"));

		_sharedSpawnPaths.Add(GetNode<Path2D>("../Paths/Path2D1"));
		_sharedSpawnPaths.Add(GetNode<Path2D>("../Paths/Path2D2"));
		_sharedSpawnPaths.Add(GetNode<Path2D>("../Paths/Path2D2_2"));
		_sharedSpawnPaths.Add(GetNode<Path2D>("../Paths/Path2D3"));

		if (_vehicles.Count == 0 || _paths.Count == 0)
		{
			GD.PrintErr("FOUT: Geen voertuigen of paden beschikbaar!");
			return;
		}

		GD.Print($"Aantal voertuigen: {_vehicles.Count}, Aantal paden: {_paths.Count}");

		// Start een timer om te spawnen
		var spawnTimer = new Timer();
		spawnTimer.WaitTime = 1.3f;
		spawnTimer.Autostart = true;
		spawnTimer.OneShot = false;
		spawnTimer.Timeout += () =>
		{
			SpawnRandomVehicle();
		};
		AddChild(spawnTimer);
	}

	public override void _Process(double delta)
	{
		if (_sharedCooldown > 0f)
			_sharedCooldown -= (float)delta;
	}

	private void SpawnRandomVehicle()
	{
		if (_vehicles.Count == 0 || _paths.Count == 0)
		{
			GD.PrintErr("FOUT: Geen voertuigen of paden beschikbaar!");
			return;
		}

		foreach (var vehicleScene in _vehicles)
		{
			var tempVehicle = vehicleScene.Instantiate<Vehicle>();
			var chance = tempVehicle.SpawnChance;

			var roll = GD.Randf() * 100f;

			if (!(roll <= chance)) continue;
			var pathIndex = (int)(GD.Randi() % _paths.Count);
			var basePath = (Path2D)_paths[pathIndex].GetParent();

			if (_sharedSpawnPaths.Contains(basePath) && _sharedCooldown > 0f)
			{
				return;
			}

			var vehicleFollow = new PathFollow2D
			{
				Loop = false,
				Rotates = true,
				Name = $"PathFollow_{GD.Randi()}"
			};

			basePath.AddChild(vehicleFollow);

			var vehicle = vehicleScene.Instantiate<Vehicle>();
			vehicleFollow.AddChild(vehicle);

			vehicle.StartMoving(vehicleFollow);

			if (_sharedSpawnPaths.Contains(basePath))
			{
				_sharedCooldown = _sharedCooldownTime;
			}

			if (vehicle is not scripts.EmergencyVehicle) continue;
			var maxSound = GetNodeOrNull<AudioStreamPlayer2D>("/root/TrafficSim/AudioStreamPlayer2D");
			if (maxSound != null && !maxSound.Playing)
				maxSound.Play();
		}
	}
}
