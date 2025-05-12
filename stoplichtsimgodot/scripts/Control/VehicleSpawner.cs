using System.Collections.Generic;
using Godot;
using StoplichtSimGodot.interfaces;
using StoplichtSimGodot.dto; // Voor evt. extra struct of DTO
using System.Text.Json;
using System.Linq;

namespace StoplichtSimGodot.scripts;

public partial class VehicleSpawner : Node2D
{
	private readonly List<PackedScene> _vehicles = new List<PackedScene>();
	private readonly List<PackedScene> _boats = new List<PackedScene>();
	private readonly List<PackedScene> _bikes = new List<PackedScene>();
	private readonly List<PackedScene> _peds = new List<PackedScene>();

	private readonly List<PathFollow2D> _paths = new List<PathFollow2D>();
	private readonly List<PathFollow2D> _boatPaths = new List<PathFollow2D>();
	private readonly List<PathFollow2D> _bikePaths = new List<PathFollow2D>();
	private readonly List<PathFollow2D> _pedPaths = new List<PathFollow2D>();

	[Export] public PackedScene Car;
	[Export] public PackedScene Bus;
	[Export] public PackedScene EmergencyVehicle;
	[Export] public PackedScene Boat;
	[Export] public PackedScene Cyclist;
	[Export] public PackedScene Pedestrian;

	private readonly HashSet<Path2D> _sharedSpawnPaths = new HashSet<Path2D>();
	private float _sharedCooldown = 0f;
	private float _sharedCooldownTime = 1.5f;
	private const float SpawnBlockZoneThreshold = 0.05f; // eerste 5% van pad

	public override void _Ready()
	{
		GD.Print("VehicleSpawner is gestart!");

		_vehicles.Add(Car);
		_vehicles.Add(Bus);
		_vehicles.Add(EmergencyVehicle);
		_boats.Add(Boat);
		_bikes.Add(Cyclist);
		_peds.Add(Pedestrian);

		createPaths();

		var spawnTimer = new Timer();
		spawnTimer.WaitTime = 1.3f;
		spawnTimer.Autostart = true;
		spawnTimer.OneShot = false;
		spawnTimer.Timeout += SpawnRandomRoadVehicle;
		spawnTimer.Timeout += SpawnRandomBoat;
		spawnTimer.Timeout += SpawnRandomBike;
		spawnTimer.Timeout += SpawnRandomPedestrian;
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
			_paths.Add(GetNode<PathFollow2D>($"../CarPaths/Path2D{i}/Path{i}"));
		}
		_paths.Add(GetNode<PathFollow2D>("../CarPaths/Path2D2_2/Path2_2"));
		_paths.Add(GetNode<PathFollow2D>("../CarPaths/Path2D8_2/Path8_2"));

		_boatPaths.Add(GetNode<PathFollow2D>($"../BoatPaths/BoatPath71/Path71"));
		_boatPaths.Add(GetNode<PathFollow2D>($"../BoatPaths/BoatPath72/Path72"));

		for (var i = 1; i < 9; i++)
		{
			_bikePaths.Add(GetNode<PathFollow2D>($"../BikePaths/BikePath{i}/Path{i}"));
		}

		for (var i = 1; i < 9; i++)
		{
			_pedPaths.Add(GetNode<PathFollow2D>($"../PedPaths/PedPath{i}/Path{i}"));
		}

		_sharedSpawnPaths.Add(GetNode<Path2D>("../CarPaths/Path2D1"));
		_sharedSpawnPaths.Add(GetNode<Path2D>("../CarPaths/Path2D2"));
		_sharedSpawnPaths.Add(GetNode<Path2D>("../CarPaths/Path2D2_2"));
		_sharedSpawnPaths.Add(GetNode<Path2D>("../CarPaths/Path2D3"));

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

			if (roll > chance) continue;

			var pathIndex = (int)(GD.Randi() % _paths.Count);
			var basePath = (Path2D)_paths[pathIndex].GetParent();

			if (_sharedSpawnPaths.Contains(basePath) && _sharedCooldown > 0f)
				return;

			if (TrySpawnVehicleOnPath(vehicleScene, basePath))
			{
				if (_sharedSpawnPaths.Contains(basePath))
					_sharedCooldown = _sharedCooldownTime;
				break;
			}
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

			if (TrySpawnVehicleOnPath(boatScene, basePath))
				break;
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

			if (TrySpawnVehicleOnPath(bikeScene, basePath))
				break;
		}
	}


	private void SpawnRandomPedestrian()
	{
		if (_peds.Count == 0 || _pedPaths.Count == 0)
		{
			GD.PrintErr("Error: No pedestrians or pedpaths available!");
			return;
		}

		foreach (var pedScene in _peds)
		{
			var tempPed = pedScene.Instantiate<Vehicle>();
			var chance = tempPed.SpawnChance;
			var roll = GD.Randf() * 100f;

			if (roll > chance) continue;

			var pathIndex = (int)(GD.Randi() % _pedPaths.Count);
			var basePath = (Path2D)_pedPaths[pathIndex].GetParent();

			if (TrySpawnVehicleOnPath(pedScene, basePath))
				break;
		}
	}

	private bool TrySpawnVehicleOnPath(PackedScene vehicleScene, Path2D basePath)
	{
		if (vehicleScene == null || basePath == null)
			return false;

		// Check of beginstuk al bezet is
		bool isBlocked = basePath.GetChildren()
			.OfType<PathFollow2D>()
			.Any(pf => pf.GetChildCount() > 0 &&
					   pf.GetChild(0) is Vehicle &&
					   pf.ProgressRatio < SpawnBlockZoneThreshold);

		if (isBlocked)
			return false;

		// Maak nieuwe PathFollow2D aan
		var vehicleFollow = new PathFollow2D
		{
			Loop = false,
			Rotates = true,
			Name = $"PathFollow_{GD.Randi()}"
		};

		basePath.AddChild(vehicleFollow); // <- eerst toevoegen aan de scene tree
		vehicleFollow.ProgressRatio = 0.0f; // <- daarna pas ProgressRatio zetten

		// Instantieer voertuig
		var vehicle = vehicleScene.Instantiate<Vehicle>();
		vehicleFollow.AddChild(vehicle);

		// Start beweging
		vehicle.StartMoving(vehicleFollow);

		// Voor voertuigtypes met prioriteit
		int? prioriteit = null;
		if (vehicle is scripts.EmergencyVehicle)
		{
			prioriteit = 1;
			var maxSound = GetNodeOrNull<AudioStreamPlayer2D>("/root/TrafficSim/AudioStreamPlayer2D");
			if (maxSound is { Playing: false })
				maxSound.Play();
		}
		else if (vehicle is scripts.Bus)
		{
			prioriteit = 2;
		}

		// Voeg toe aan queue
		if (prioriteit.HasValue && VoorrangsQueueManager.Instance != null)
		{
			var baanNaam = basePath.Name;
			VoorrangsQueueManager.Instance.VoegToe(vehicle, baanNaam, prioriteit.Value);
		}

		return true;
	}
}
