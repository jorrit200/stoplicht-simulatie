using Godot;
using System;

public partial class Bus : Vehicle
{
	public Bus()
	{
		speed = 50;
		roadToUse = "MainRoad";
		isMoving = true;
		spawnChance = 50;
	}
}
