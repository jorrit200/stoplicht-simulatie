using System.Collections.Generic;
using Godot;

namespace StoplichtSimGodot.scripts;

public partial class VehicleSpawner : Node2D
{
    // Lijst van PackedScenes
    private readonly List<PackedScene> _vehicles = new List<PackedScene>();

    // Lijst van PathFollow2D nodes
    private readonly List<PathFollow2D> _paths = new List<PathFollow2D>();

    // Referenties naar de verschillende voertuigen
    [Export] public PackedScene Car;
    [Export] public PackedScene Bus;
    [Export] public PackedScene EmergencyVehicle;

    private readonly HashSet<Path2D> _sharedSpawnPaths = new HashSet<Path2D>();
    private float _sharedCooldown = 0f;
    private float _sharedCooldownTime = 1.5f;

    public override void _Ready()
    {
        GD.Print("VehicleSpawner is gestart!");

        // Voeg de voertuigen toe aan de lijst
        _vehicles.Add(Car);
        _vehicles.Add(Bus);
        _vehicles.Add(EmergencyVehicle);

        // Haal de PathFollow2D nodes op (paden)
        for (int i = 1; i < 13; i++)
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
        Timer spawnTimer = new Timer();
        spawnTimer.WaitTime = 1.3f;
        spawnTimer.Autostart = true;
        spawnTimer.OneShot = false;
        spawnTimer.Timeout += () =>
        {
            //GD.Print("Timer triggered! Spawning vehicle...");
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

        // Loop door elk voertuigtype en bepaal of deze moet spawnen
        foreach (PackedScene vehicleScene in _vehicles)
        {
            // Maak een tijdelijke instantie om de spawnChance te lezen
            Vehicle tempVehicle =
                vehicleScene.Instantiate<Vehicle>();
            float chance = tempVehicle.SpawnChance;

            float roll = GD.Randf() * 100f;
            //GD.Print($"[{vehicleScene.ResourcePath}] spawn roll: {roll} vs chance {chance}");

            if (roll <= chance)
            {
                // Selecteer willekeurig pad
                int pathIndex = (int)(GD.Randi() % _paths.Count);
                Path2D basePath = (Path2D)_paths[pathIndex].GetParent();

                if (_sharedSpawnPaths.Contains(basePath) && _sharedCooldown > 0f)
                {
                    //GD.Print($"Cooldown actief voor {basePath.Name}, voertuig wordt niet gespawnd.");
                    return;
                }

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

                if (_sharedSpawnPaths.Contains(basePath))
                {
                    _sharedCooldown = _sharedCooldownTime;
                }

                //GD.Print($"Spawned voertuig {vehicle.Name} op pad {basePath.Name}");

                if (vehicle is EmergencyVehicle)
                {
                    var maxSound = GetNodeOrNull<AudioStreamPlayer2D>("/root/TrafficSim/AudioStreamPlayer2D");
                    if (maxSound != null && !maxSound.Playing)
                        maxSound.Play();
                }
            }
        }
    }
}