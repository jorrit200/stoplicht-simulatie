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
		spawnTimer.WaitTime = 1; 
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

		// Loop door elk voertuigtype en bepaal of deze moet spawnen
		foreach (PackedScene vehicleScene in vehicles)
		{
			// Maak een tijdelijke instantie om de spawnChance te lezen
			Vehicle tempVehicle = vehicleScene.Instantiate<Vehicle>();
			float chance = tempVehicle.spawnChance;

			float roll = GD.Randf() * 100f;
			GD.Print($"[{vehicleScene.ResourcePath}] spawn roll: {roll} vs chance {chance}");

			if (roll <= chance)
			{
				// Selecteer willekeurig pad
				int pathIndex = (int)(GD.Randi() % paths.Count);
				Path2D basePath = (Path2D)paths[pathIndex].GetParent();

				// Maak een nieuwe PathFollow2D aan
				PathFollow2D vehicleFollow = new PathFollow2D
				{
					Loop = false,
					Rotates = true,
					Name = $"PathFollow_{GD.Randi()}"
				};

				basePath.AddChild(vehicleFollow);

				// Instantieer het voertuig
				Vehicle vehicle = vehicleScene.Instantiate<Vehicle>();
				vehicleFollow.AddChild(vehicle);

				// Start beweging
				vehicle.StartMoving(vehicleFollow);

				GD.Print($"Spawned voertuig {vehicle.Name} op pad {basePath.Name}");
			}
		}
	}
}
