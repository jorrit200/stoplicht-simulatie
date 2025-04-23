using System.Collections.Generic;
using Godot;

namespace StoplichtSimGodot.scripts;

public partial class VehicleSpawner : Node2D
{
	private readonly List<PackedScene> _vehicles = new List<PackedScene>();
	private readonly List<PackedScene> _boats = new List<PackedScene>();
	private readonly List<PackedScene> _bikes = new List<PackedScene>();

	private readonly List<PathFollow2D> _paths = new List<PathFollow2D>();
	private readonly List<PathFollow2D> _boatPaths = new List<PathFollow2D>();
	private readonly List<PathFollow2D> _bikePaths = new List<PathFollow2D>();

	[Export] public PackedScene Car;
	[Export] public PackedScene Bus;
	[Export] public PackedScene EmergencyVehicle;
	[Export] public PackedScene Boat;
	[Export] public PackedScene Cyclist;

	private readonly HashSet<Path2D> _sharedSpawnPaths = new HashSet<Path2D>();
	private float _sharedCooldown = 0f;
	private float _sharedCooldownTime = 1.5f;

	public override void _Ready()
	{
		GD.Print("VehicleSpawner is gestart!");

		_vehicles.Add(Car);
		_vehicles.Add(Bus);
		_vehicles.Add(EmergencyVehicle);
		_boats.Add(Boat);
		_bikes.Add(Cyclist);

		createPaths();

		var spawnTimer = new Timer();
		spawnTimer.WaitTime = 1.3f;
		spawnTimer.Autostart = true;
		spawnTimer.OneShot = false;
		spawnTimer.Timeout += SpawnRandomRoadVehicle;
		spawnTimer.Timeout += SpawnRandomBoat;
		spawnTimer.Timeout += SpawnRandomBike;
		AddChild(spawnTimer);
	}

	public override void _Process(double delta)
	{
		if (_sharedCooldown > 0f)
			_sharedCooldown -= (float)delta;
	}

	public void createPaths()
	{
		for (var i = 1; i < 13; i++)
		{
			_paths.Add(GetNode<PathFollow2D>($"../Paths/Path2D{i}/Path{i}"));
		}
		_paths.Add(GetNode<PathFollow2D>("../Paths/Path2D2_2/Path2_2"));
		_paths.Add(GetNode<PathFollow2D>("../Paths/Path2D8_2/Path8_2"));

		_boatPaths.Add(GetNode<PathFollow2D>($"../BoatPaths/BoatPath71/Path71"));
		_boatPaths.Add(GetNode<PathFollow2D>($"../BoatPaths/BoatPath72/Path72"));

		for (var i = 1; i < 9; i++)
		{
			_bikePaths.Add(GetNode<PathFollow2D>($"../BikePaths/BikePath{i}/Path{i}"));
		}

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
	}

	private void SpawnRandomRoadVehicle()
	{
		if (_vehicles.Count == 0 || _paths.Count == 0)
		{
			GD.PrintErr("Error: No vehicles or paths available!");
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
			if (maxSound is { Playing: false })
				maxSound.Play();
		}
	}

	private void SpawnRandomBoat()
	{
		if (_boats.Count == 0 || _boatPaths.Count == 0)
		{
			GD.PrintErr("Error: No boats or boatpaths available!");
			return;
		}

		foreach (var boatScene in _boats)
		{
			var tempBoat = boatScene.Instantiate<Vehicle>();
			var chance = tempBoat.SpawnChance;
			var roll = GD.Randf() * 100f;

			if (roll > chance) continue;

			var pathIndex = (int)(GD.Randi() % _boatPaths.Count);
			var basePath = (Path2D)_boatPaths[pathIndex].GetParent();

			var boatFollow = new PathFollow2D
			{
				Loop = false,
				Rotates = true,
				Name = $"BoatFollow_{GD.Randi()}"
			};

			basePath.AddChild(boatFollow);

			var boat = boatScene.Instantiate<Vehicle>();
			boatFollow.AddChild(boat);

			boat.StartMoving(boatFollow);
		}
	}

	private void SpawnRandomBike()
	{
		if (_bikes.Count == 0 || _bikePaths.Count == 0)
		{
			GD.PrintErr("Error: No bikes or bikepaths available!");
			return;
		}

		foreach (var bikeScene in _bikes)
		{
			var tempBike = bikeScene.Instantiate<Vehicle>();
			var chance = tempBike.SpawnChance;
			var roll = GD.Randf() * 100f;

			if (roll > chance) continue;

			var pathIndex = (int)(GD.Randi() % _bikePaths.Count);
			var basePath = (Path2D)_bikePaths[pathIndex].GetParent();

			var bikeFollow = new PathFollow2D
			{
				Loop = false,
				Rotates = true,
				Name = $"BikeFollow_{GD.Randi()}"
			};

			basePath.AddChild(bikeFollow);

			var bike = bikeScene.Instantiate<Vehicle>();
			bikeFollow.AddChild(bike);

			bike.StartMoving(bikeFollow);
		}
	}
}
