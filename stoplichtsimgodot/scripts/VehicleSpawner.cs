using Godot;
using System;
using System.Collections.Generic;

public partial class VehicleSpawner : Node2D
{
	// Lijst van PackedScenes
	private List<PackedScene> vehicles = new List<PackedScene>();

	// Lijst van PathFollow2D nodes
	private List<PathFollow2D> paths = new List<PathFollow2D>();

	// Referenties naar de verschillende voertuigen
	[Export] public PackedScene Car;
	[Export] public PackedScene Bus;
	[Export] public PackedScene EmergencyVehicle;

	public override void _Ready()
	{
		GD.Print("VehicleSpawner is gestart!");

		// Voeg de voertuigen toe aan de lijst
		vehicles.Add(Car);
		vehicles.Add(Bus);
		vehicles.Add(EmergencyVehicle);

		// Haal de PathFollow2D nodes op (paden)
		paths.Add(GetNode<PathFollow2D>("../Path2D/Path1"));
		paths.Add(GetNode<PathFollow2D>("../Path2D2/Path2"));

		if (vehicles.Count == 0 || paths.Count == 0)
		{
			GD.PrintErr("FOUT: Geen voertuigen of paden beschikbaar!");
			return;
		}

		GD.Print($"Aantal voertuigen: {vehicles.Count}, Aantal paden: {paths.Count}");

		// Start een timer om te spawnen
		Timer spawnTimer = new Timer();
		spawnTimer.WaitTime = 2.0f;  // Tijd tussen spawns
		spawnTimer.Autostart = true;
		spawnTimer.OneShot = false;
		spawnTimer.Timeout += () => {
			GD.Print("Timer triggered! Spawning vehicle...");
			SpawnRandomVehicle();
		};
		AddChild(spawnTimer);
	}

	private void SpawnRandomVehicle()
{
	if (vehicles.Count == 0 || paths.Count == 0)
	{
		GD.PrintErr("FOUT: Geen voertuigen of paden beschikbaar!");
		return;
	}

	// Kies een willekeurig voertuig en pad
	int vehicleIndex = (int)(GD.Randi() % vehicles.Count);
	int pathIndex = (int)(GD.Randi() % paths.Count);

	GD.Print($"Gekozen voertuig index: {vehicleIndex}, Gekozen pad index: {pathIndex}");

	PackedScene vehicleScene = vehicles[vehicleIndex];  // Haal de juiste scene op
	Path2D chosenPath = paths[pathIndex].GetParent<Path2D>();  // Haal het Path2D-object op

	if (vehicleScene == null || chosenPath == null)
	{
		GD.PrintErr("FOUT: Geen voertuig of pad gevonden!");
		return;
	}

	// Maak een nieuw PathFollow2D aan voor dit voertuig
	PathFollow2D pathFollow = new PathFollow2D();
	chosenPath.AddChild(pathFollow);
	pathFollow.Progress = 0.0f;  // Zorg dat het voertuig aan het begin start

	// Maak een nieuw voertuig aan
	Vehicle vehicle = (Vehicle)vehicleScene.Instantiate();
	
	if (vehicle == null)
	{
		GD.PrintErr("FOUT: Kon voertuig niet instantieren!");
		return;
	}

	GD.Print($"Voertuig {vehicle.Name} geïnstantieerd!");

	// Zet de Vehicle specifieke eigenschappen
	vehicle.roadToUse = "Path" + (pathIndex + 1);
	pathFollow.AddChild(vehicle);  // Voeg het voertuig toe aan het PathFollow2D
	vehicle.StartMoving(pathFollow);  // Laat het voertuig bewegen

	GD.Print($"Voertuig {vehicle.Name} succesvol toegevoegd!");
	}
}
