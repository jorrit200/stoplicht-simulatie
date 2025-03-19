using Godot;
using System;

public partial class Car : Vehicle
{
    Car car = new()
    {
        speed = 50,
        roadToUse = "CarRoad",
        isMoving = true,
        spawnChance = 80
    };
}
