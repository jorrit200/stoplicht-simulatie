namespace StoplichtSimGodot.scripts;

public partial class Car : Vehicle
{
	public Car()
	{
		Speed = 50;
		RoadToUse = "MainRoad";
		IsMoving = true;
		SpawnChance = 75f;
	}
}
