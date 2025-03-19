using Godot;
using System;

public partial class EmergencyVehicle : Vehicle
{
    EmergencyVehicle emergencyVehicle = new()
    {
        speed = 100,
        roadToUse = "MainRoad",
        isMoving = true,
        spawnChance = 0.5f
    };
}
