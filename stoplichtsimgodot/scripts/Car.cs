using Godot;
using System;

public partial class Car : Vehicle
{
    Car car = new Car()
    {
        speed = 50,
        roadToUse = "Road1",
        isMoving = true,
        spawnChance = 50
    };
}
