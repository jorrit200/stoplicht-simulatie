using Godot;
using System;

public partial class Car : Vehicle
{
	public Car()
	{
		speed = 50;
		roadToUse = "MainRoad";
		isMoving = true;
		spawnChance = 75;
	}
}
