using Godot;
using System;

public partial class EmergencyVehicle : Vehicle
{
	public override void _Ready()
	{
		speed = 100;
		roadToUse = "MainRoad";
		isMoving = true;
		spawnChance = 1;
	}
}
