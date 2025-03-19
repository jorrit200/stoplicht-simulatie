using Godot;
using System;

public partial class Car : Vehicle
{
	Car car = new()
	{
		speed = 50,
		roadToUse = "MainRoad",
		isMoving = true,
		spawnChance = 80
	};
}
