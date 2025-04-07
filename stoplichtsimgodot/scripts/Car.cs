using Godot;
using System;

public partial class Car : Vehicle
{
	public override void _Ready()
	{
		speed = 50;
		roadToUse = "MainRoad";
		isMoving = true;
		spawnChance = 90;
	}
}
