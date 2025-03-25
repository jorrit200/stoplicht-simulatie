using Godot;
using System;

public partial class Bus : Vehicle
{
	Bus bus = new()
	{
		speed = 50,
		roadToUse = "MainRoad",
		isMoving = true,
		spawnChance = 5
	};
}
